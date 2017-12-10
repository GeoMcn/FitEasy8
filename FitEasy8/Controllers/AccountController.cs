using FitEasy8.App_Start;
using FitEasy8.DAL;
using FitEasy8.Models;
using FitEasy8.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System;

namespace FitEasy8.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private FitEasyContext context = new FitEasyContext();


        private UserManager<User> manager;

        public AccountController()
        {
            context = new FitEasyContext();
            manager = new UserManager<User>(new UserStore<User>(context));
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "User");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }


        private void PopulateAssignedExercisePlanData(RegisterViewModel user)
        {

            var allExercisePlans = context.ExercisePlans;
            var userExercisePlans = new HashSet<int>(user.ExercisePlans.Select(c => c.ExercisePlanID));
            var viewModel = new List<AssignedExercisePlanData>();
            foreach (var exercisePlan in allExercisePlans)
            {
                viewModel.Add(new AssignedExercisePlanData
                {
                    ExercisePlanID = exercisePlan.ExercisePlanID,
                    Title = exercisePlan.Title,
                    Assigned = userExercisePlans.Contains(exercisePlan.ExercisePlanID)
                });
            }
            ViewBag.ExercisePlans = viewModel;
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var user = new RegisterViewModel();
            user.ExercisePlans = new List<ExercisePlan>();
            PopulateAssignedExercisePlanData(user);

            var enumData1 = from TargetAim e in Enum.GetValues(typeof(TargetAim))
                            select new
                            {
                                ID = (int)e,
                                Name = e.ToString()
                            };
            ViewBag.EnumList1 = new SelectList(enumData1, "ID", "Name");

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string[] selectedExercisePlans, [Bind(Exclude = "Image")] RegisterViewModel model, HttpPostedFileBase file)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {

                // To convert the user uploaded Photo as Byte Array before save to DB
                byte[] imageData = null;
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase poImgFile = Request.Files["Image"];

                    using (var binary = new BinaryReader(poImgFile.InputStream))
                    {
                        imageData = binary.ReadBytes(poImgFile.ContentLength);
                    }
                }

                var user = new User
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Height = model.Height,
                    Weight = model.Weight,
                    BMI = model.BMI,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    TargetAim = model.TargetAim,
                    ExercisesCompleted = 0,
                    PlansCompleted = 0,
                    Count = 1


                };

                var users = context.Users;

                foreach (var otherUser in users)
                {
                    if (user.UserName == otherUser.UserName)
                    {
                        return RedirectToAction("Register2", "Account");
                    }
                }


                foreach (var otherUser in users)
                {
                    if (user.Email == otherUser.Email)
                    {
                        return RedirectToAction("Email", "Account");
                    }
                }


                user.Image = imageData;
                var result = await UserManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    //Assign Role to user Here   
                    //await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);
                    //Ends Here 


                    if (user.Height != null && user.Weight != null)
                    {
                        var bmi = new BMI
                        {
                            UserId = user.Id,
                            Height = user.Height,
                            Weight = user.Weight,
                            Result = user.Weight.Value / (user.Height.Value * user.Height.Value),
                            AddedOn = System.DateTime.Now,

                        };
                        context.BMI.Add(bmi);
                        if (user.TargetAim == TargetAim.GainMuscle)
                        {


                            if (bmi.Result <= 18.5)
                            {
                                bmi.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                            }
                            else if (bmi.Result <= 24.9)
                            {
                                bmi.Verdict = "you are average weight and seem healthy";
                            }
                            else if (bmi.Result <= 29.9)
                            {
                                bmi.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                            }
                            else if (bmi.Result >= 30)
                            {
                                bmi.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                            }
                            context.SaveChanges();
                        }

                        else
                        {
                            if (bmi.Result <= 18.5)
                            {
                                bmi.Verdict = "you are under average and should attempt to gain more wieght";
                            }
                            else if (bmi.Result <= 24.9)
                            {
                                bmi.Verdict = "you are average weight and seem healthy";
                            }
                            else if (bmi.Result <= 29.9)
                            {
                                bmi.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                            }
                            else if (bmi.Result >= 30)
                            {
                                bmi.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                            }
                            context.SaveChanges();

                        }
                        context.SaveChanges();

                    }

                    #region  BMI data for testing phase sample Charts
                    var bmi2 = new BMI
                    {
                        UserId = user.Id,
                        Height = user.Height,
                        Weight = 87,
                        AddedOn = System.DateTime.Parse("2017-10-11"),

                    };
                    context.BMI.Add(bmi2);
                    context.SaveChanges();

                    if (user.Height == null)
                    {
                        bmi2.Height = 1.70;
                    }

                    context.SaveChanges();

                    bmi2.Result = bmi2.Weight.Value / (bmi2.Height.Value * bmi2.Height.Value);
                    if (user.TargetAim == TargetAim.GainMuscle)
                    {


                        if (bmi2.Result <= 18.5)
                        {
                            bmi2.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                        }
                        else if (bmi2.Result <= 24.9)
                        {
                            bmi2.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi2.Result <= 29.9)
                        {
                            bmi2.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                        }
                        else if (bmi2.Result >= 30)
                        {
                            bmi2.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                        }
                        context.SaveChanges();
                    }

                    else
                    {
                        if (bmi2.Result <= 18.5)
                        {
                            bmi2.Verdict = "you are under average and should attempt to gain more wieght";
                        }
                        else if (bmi2.Result <= 24.9)
                        {
                            bmi2.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi2.Result <= 29.9)
                        {
                            bmi2.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                        }
                        else if (bmi2.Result >= 30)
                        {
                            bmi2.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                        }
                        context.SaveChanges();

                    }
                    context.SaveChanges();

                    var bmi3 = new BMI
                    {
                        UserId = user.Id,
                        Height = user.Height,
                        Weight = 77,
                        AddedOn = System.DateTime.Parse("2017-09-11"),

                    };
                    context.BMI.Add(bmi3);
                    context.SaveChanges();

                    if (user.Height == null)
                    {
                        bmi3.Height = 1.70;
                    }

                    context.SaveChanges();

                    bmi3.Result = bmi3.Weight.Value / (bmi3.Height.Value * bmi3.Height.Value);
                    if (user.TargetAim == TargetAim.GainMuscle)
                    {


                        if (bmi3.Result <= 18.5)
                        {
                            bmi3.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                        }
                        else if (bmi3.Result <= 24.9)
                        {
                            bmi3.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi3.Result <= 29.9)
                        {
                            bmi3.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                        }
                        else if (bmi3.Result >= 30)
                        {
                            bmi3.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                        }
                        context.SaveChanges();
                    }

                    else
                    {
                        if (bmi3.Result <= 18.5)
                        {
                            bmi3.Verdict = "you are under average and should attempt to gain more wieght";
                        }
                        else if (bmi3.Result <= 24.9)
                        {
                            bmi3.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi3.Result <= 29.9)
                        {
                            bmi3.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                        }
                        else if (bmi3.Result >= 30)
                        {
                            bmi3.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                        }
                        context.SaveChanges();

                    }
                    context.SaveChanges();

                    var bmi4 = new BMI
                    {
                        UserId = user.Id,
                        Height = user.Height,
                        Weight = 55,
                        AddedOn = System.DateTime.Parse("2017-08-11"),

                    };

                    context.BMI.Add(bmi4);
                    context.SaveChanges();

                    if (user.Height == null)
                    {
                        bmi4.Height = 1.70;
                    }
                    context.SaveChanges();

                    bmi4.Result = bmi4.Weight.Value / (bmi4.Height.Value * bmi4.Height.Value);
                    if (user.TargetAim == TargetAim.GainMuscle)
                    {


                        if (bmi4.Result <= 18.5)
                        {
                            bmi4.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                        }
                        else if (bmi4.Result <= 24.9)
                        {
                            bmi4.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi4.Result <= 29.9)
                        {
                            bmi4.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                        }
                        else if (bmi4.Result >= 30)
                        {
                            bmi4.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                        }
                        context.SaveChanges();
                    }

                    else
                    {
                        if (bmi4.Result <= 18.5)
                        {
                            bmi4.Verdict = "you are under average and should attempt to gain more wieght";
                        }
                        else if (bmi4.Result <= 24.9)
                        {
                            bmi4.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi4.Result <= 29.9)
                        {
                            bmi4.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                        }
                        else if (bmi4.Result >= 30)
                        {
                            bmi4.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                        }
                        context.SaveChanges();

                    }
                    context.SaveChanges();


                    var bmi5 = new BMI
                    {
                        UserId = user.Id,
                        Height = user.Height,
                        Weight = 65,
                        AddedOn = System.DateTime.Parse("2017-07-11"),

                    };

                    context.BMI.Add(bmi5);
                    context.SaveChanges();

                    if (user.Height == null)
                    {
                        bmi5.Height = 1.70;
                    }
                    context.SaveChanges();

                    bmi5.Result = bmi5.Weight.Value / (bmi5.Height.Value * bmi5.Height.Value);
                    if (user.TargetAim == TargetAim.GainMuscle)
                    {


                        if (bmi5.Result <= 18.5)
                        {
                            bmi5.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                        }
                        else if (bmi5.Result <= 24.9)
                        {
                            bmi5.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi5.Result <= 29.9)
                        {
                            bmi5.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                        }
                        else if (bmi5.Result >= 30)
                        {
                            bmi5.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                        }
                        context.SaveChanges();
                    }

                    else
                    {
                        if (bmi5.Result <= 18.5)
                        {
                            bmi5.Verdict = "you are under average and should attempt to gain more wieght";
                        }
                        else if (bmi5.Result <= 24.9)
                        {
                            bmi5.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi5.Result <= 29.9)
                        {
                            bmi5.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                        }
                        else if (bmi5.Result >= 30)
                        {
                            bmi5.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                        }
                        context.SaveChanges();

                    }
                    context.SaveChanges();
                    #endregion

                    return RedirectToAction("UserProfile", "User");

                }
                AddErrors(result);
                return View();
            }




            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register2()
        {
            var user = new RegisterViewModel();
            user.ExercisePlans = new List<ExercisePlan>();
            PopulateAssignedExercisePlanData(user);

            var enumData1 = from TargetAim e in Enum.GetValues(typeof(TargetAim))
                            select new
                            {
                                ID = (int)e,
                                Name = e.ToString()
                            };
            ViewBag.EnumList1 = new SelectList(enumData1, "ID", "Name");

            return View();
        }
        //
        // POST: /Account/Register2
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register2(string[] selectedExercisePlans, [Bind(Exclude = "Image")] RegisterViewModel model, HttpPostedFileBase file)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {

                // To convert the user uploaded Photo as Byte Array before save to DB
                byte[] imageData = null;
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase poImgFile = Request.Files["Image"];

                    using (var binary = new BinaryReader(poImgFile.InputStream))
                    {
                        imageData = binary.ReadBytes(poImgFile.ContentLength);
                    }
                }

                var user = new User
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Height = model.Height,
                    Weight = model.Weight,
                    BMI = model.BMI,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    TargetAim = model.TargetAim,
                    ExercisesCompleted = 0,
                    PlansCompleted = 0,
                    Count = 1


                };

                var usernames = context.Users;

                foreach (var otherUser in usernames)
                {
                    if (user.UserName == otherUser.UserName)
                    {
                        return RedirectToAction("Register2", "Account");
                    }
                }



                user.Image = imageData;
                var result = await UserManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    //Assign Role to user Here   
                    //await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);
                    //Ends Here 


                    if (user.Height != null && user.Weight != null)
                    {
                        var bmi = new BMI
                        {
                            UserId = user.Id,
                            Height = user.Height,
                            Weight = user.Weight,
                            Result = user.Weight.Value / (user.Height.Value * user.Height.Value),
                            AddedOn = System.DateTime.Now,

                        };
                        context.BMI.Add(bmi);
                        if (user.TargetAim == TargetAim.GainMuscle)
                        {


                            if (bmi.Result <= 18.5)
                            {
                                bmi.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                            }
                            else if (bmi.Result <= 24.9)
                            {
                                bmi.Verdict = "you are average weight and seem healthy";
                            }
                            else if (bmi.Result <= 29.9)
                            {
                                bmi.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                            }
                            else if (bmi.Result >= 30)
                            {
                                bmi.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                            }
                            context.SaveChanges();
                        }

                        else
                        {
                            if (bmi.Result <= 18.5)
                            {
                                bmi.Verdict = "you are under average and should attempt to gain more wieght";
                            }
                            else if (bmi.Result <= 24.9)
                            {
                                bmi.Verdict = "you are average weight and seem healthy";
                            }
                            else if (bmi.Result <= 29.9)
                            {
                                bmi.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                            }
                            else if (bmi.Result >= 30)
                            {
                                bmi.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                            }
                            context.SaveChanges();

                        }
                        context.SaveChanges();

                    }

                    #region  BMI data for testing phase sample Charts
                    var bmi2 = new BMI
                    {
                        UserId = user.Id,
                        Height = user.Height,
                        Weight = 87,
                        AddedOn = System.DateTime.Parse("2017-10-11"),

                    };
                    context.BMI.Add(bmi2);
                    context.SaveChanges();

                    if (user.Height == null)
                    {
                        bmi2.Height = 1.70;
                    }

                    context.SaveChanges();

                    bmi2.Result = bmi2.Weight.Value / (bmi2.Height.Value * bmi2.Height.Value);
                    if (user.TargetAim == TargetAim.GainMuscle)
                    {


                        if (bmi2.Result <= 18.5)
                        {
                            bmi2.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                        }
                        else if (bmi2.Result <= 24.9)
                        {
                            bmi2.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi2.Result <= 29.9)
                        {
                            bmi2.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                        }
                        else if (bmi2.Result >= 30)
                        {
                            bmi2.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                        }
                        context.SaveChanges();
                    }

                    else
                    {
                        if (bmi2.Result <= 18.5)
                        {
                            bmi2.Verdict = "you are under average and should attempt to gain more wieght";
                        }
                        else if (bmi2.Result <= 24.9)
                        {
                            bmi2.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi2.Result <= 29.9)
                        {
                            bmi2.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                        }
                        else if (bmi2.Result >= 30)
                        {
                            bmi2.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                        }
                        context.SaveChanges();

                    }
                    context.SaveChanges();

                    var bmi3 = new BMI
                    {
                        UserId = user.Id,
                        Height = user.Height,
                        Weight = 77,
                        AddedOn = System.DateTime.Parse("2017-09-11"),

                    };
                    context.BMI.Add(bmi3);
                    context.SaveChanges();

                    if (user.Height == null)
                    {
                        bmi3.Height = 1.70;
                    }

                    context.SaveChanges();

                    bmi3.Result = bmi3.Weight.Value / (bmi3.Height.Value * bmi3.Height.Value);
                    if (user.TargetAim == TargetAim.GainMuscle)
                    {


                        if (bmi3.Result <= 18.5)
                        {
                            bmi3.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                        }
                        else if (bmi3.Result <= 24.9)
                        {
                            bmi3.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi3.Result <= 29.9)
                        {
                            bmi3.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                        }
                        else if (bmi3.Result >= 30)
                        {
                            bmi3.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                        }
                        context.SaveChanges();
                    }

                    else
                    {
                        if (bmi3.Result <= 18.5)
                        {
                            bmi3.Verdict = "you are under average and should attempt to gain more wieght";
                        }
                        else if (bmi3.Result <= 24.9)
                        {
                            bmi3.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi3.Result <= 29.9)
                        {
                            bmi3.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                        }
                        else if (bmi3.Result >= 30)
                        {
                            bmi3.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                        }
                        context.SaveChanges();

                    }
                    context.SaveChanges();

                    var bmi4 = new BMI
                    {
                        UserId = user.Id,
                        Height = user.Height,
                        Weight = 55,
                        AddedOn = System.DateTime.Parse("2017-08-11"),

                    };

                    context.BMI.Add(bmi4);
                    context.SaveChanges();

                    if (user.Height == null)
                    {
                        bmi4.Height = 1.70;
                    }
                    context.SaveChanges();

                    bmi4.Result = bmi4.Weight.Value / (bmi4.Height.Value * bmi4.Height.Value);
                    if (user.TargetAim == TargetAim.GainMuscle)
                    {


                        if (bmi4.Result <= 18.5)
                        {
                            bmi4.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                        }
                        else if (bmi4.Result <= 24.9)
                        {
                            bmi4.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi4.Result <= 29.9)
                        {
                            bmi4.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                        }
                        else if (bmi4.Result >= 30)
                        {
                            bmi4.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                        }
                        context.SaveChanges();
                    }

                    else
                    {
                        if (bmi4.Result <= 18.5)
                        {
                            bmi4.Verdict = "you are under average and should attempt to gain more wieght";
                        }
                        else if (bmi4.Result <= 24.9)
                        {
                            bmi4.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi4.Result <= 29.9)
                        {
                            bmi4.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                        }
                        else if (bmi4.Result >= 30)
                        {
                            bmi4.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                        }
                        context.SaveChanges();

                    }
                    context.SaveChanges();


                    var bmi5 = new BMI
                    {
                        UserId = user.Id,
                        Height = user.Height,
                        Weight = 65,
                        AddedOn = System.DateTime.Parse("2017-07-11"),

                    };

                    context.BMI.Add(bmi5);
                    context.SaveChanges();

                    if (user.Height == null)
                    {
                        bmi5.Height = 1.70;
                    }
                    context.SaveChanges();

                    bmi5.Result = bmi5.Weight.Value / (bmi5.Height.Value * bmi5.Height.Value);
                    if (user.TargetAim == TargetAim.GainMuscle)
                    {


                        if (bmi5.Result <= 18.5)
                        {
                            bmi5.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                        }
                        else if (bmi5.Result <= 24.9)
                        {
                            bmi5.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi5.Result <= 29.9)
                        {
                            bmi5.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on after putting in some work!";
                        }
                        else if (bmi5.Result >= 30)
                        {
                            bmi5.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                        }
                        context.SaveChanges();
                    }

                    else
                    {
                        if (bmi5.Result <= 18.5)
                        {
                            bmi5.Verdict = "you are under average and should attempt to gain more wieght";
                        }
                        else if (bmi5.Result <= 24.9)
                        {
                            bmi5.Verdict = "you are average weight and seem healthy";
                        }
                        else if (bmi5.Result <= 29.9)
                        {
                            bmi5.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                        }
                        else if (bmi5.Result >= 30)
                        {
                            bmi5.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                        }
                        context.SaveChanges();

                    }
                    context.SaveChanges();
                    #endregion

                    return RedirectToAction("UserProfile", "User");

                }
                AddErrors(result);
                return View();
            }




            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Email()
        {

            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "User");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }




        #endregion
    }
}