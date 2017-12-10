using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FitEasy8.DAL;
using FitEasy8.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using FitEasy8.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using Microsoft.AspNet.Identity.Owin;
using FitEasy8.ViewModels;

namespace FitEasy8.Controllers
{
    public class UserController : Controller
    {
        private FitEasyContext db = new FitEasyContext();


        private UserManager<User> manager;

        public UserController()
        {
            db = new FitEasyContext();
            manager = new UserManager<User>(new UserStore<User>(db));
        }


        public async Task<ActionResult> UserProfile(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            UserProfile user = new UserProfile()
            {
                Id = currentUser.Id,
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Password = currentUser.Password,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Height = currentUser.Height,
                Weight = currentUser.Weight,
                ExercisePlans = currentUser.ExercisePlans,
                BMI = currentUser.BMI,
                Image = currentUser.Image,
                TargetAim = currentUser.TargetAim


            };


            double? countAchievements = 0;
            foreach (var achievement in db.Achievements)
            {

                if (achievement.UserId == currentUser.Id)
                {
                    countAchievements++;

                }
                ViewBag.Count3 = countAchievements.Value;
            }

            double? count = 0;
            foreach (var plan in db.MyExercisePlans)
            {
                if (plan.UserId == currentUser.Id)
                {
                    count++;

                    ViewBag.Count2 = count;
                }
            }

            double? isDone = 0;
            foreach (var plan in db.MyExercisePlans)
            {
                if (plan.UserId == currentUser.Id && plan.IsDone == true)
                {
                    isDone++;
                }
            }

            if (count == null)
            {
                count = 0;
            }

            if (count >= 1 && isDone == count)
            {
                ViewBag.message = "Wow!! You have completed all of your ExercisePlans! You should attempt them again for roughly 6 weeks before moving to a more difficult setting or creating a new plan.";
            }




            decimal? totalPlansCompleted = (from users in db.Users
                                            where users.Id == currentUser.Id
                                            select (int?)users.Count * users.PlansCompleted.Value).Sum();

            ViewBag.OverallPlansCompleted = totalPlansCompleted.ToString();

            decimal? totalExercisesCompleted = (from users in db.Users
                                                where users.Id == currentUser.Id
                                                select (int?)users.Count * users.ExercisesCompleted.Value).Sum();

            ViewBag.OverallExercisesCompleted = totalExercisesCompleted.ToString();


            double? exerciseCount = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id)
                {
                    exerciseCount++;

                }
            }

            double? exerciseIsDone = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.IsDone == true)
                {
                    exerciseIsDone++;
                }
            }


            if (exerciseCount == null)
            {
                exerciseCount = 0;
            }

            if (exerciseCount >= 1 && exerciseIsDone == exerciseCount)
            {
                ViewBag.message = "Wow!! You have completed all of your Exercises! Well Done! You should create a new exercise plan.";

            }

            ViewBag.getExerciseCount = (exerciseIsDone.Value / exerciseCount.Value) * (100);
            ViewBag.exerciseCount = exerciseCount.Value;
            ViewBag.exerciseIsComplete = exerciseIsDone.Value;
            ViewBag.getCount = (isDone.Value / count.Value) * (100);
            ViewBag.Count = count.Value;
            ViewBag.isComplete = isDone.Value;

            return View(user);
        }



        public FileContentResult UserPhotos()
        {
            if (User.Identity.IsAuthenticated)
            {
                String userId = User.Identity.GetUserId();

                // to get the user details to load user Image
                var bdUsers = HttpContext.GetOwinContext().Get<FitEasyContext>();
                var userImage = bdUsers.Users.Where(x => x.Id == userId).FirstOrDefault();
                return new FileContentResult(userImage.Image, "image/jpeg");
            }
            else
            {
                string fileName = HttpContext.Server.MapPath(@"~/Images/noImg.png");

                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return File(imageData, "image/png");

            }
        }

        public FileContentResult Photo()
        {

            {
                string fileName = HttpContext.Server.MapPath(@"~/Images/fiteasypic2.png");

                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return File(imageData, "image/png");

            }
        }



        // GET: User
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var users = from s in db.Users
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(s => s.LastName);
                    break;
                case "Email":
                    users = users.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                default:
                    users = users.OrderBy(s => s.LastName);
                    break;
            }
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET: User/Details/5
        public ActionResult Details(string id)
        {

            var user = db.Users.Include(i => i.ExercisePlans).Where(p => p.Id == id).FirstOrDefault();
            // note you may need to add .Include("SpecificationsTable") in the above
            if (user == null)
            {
                return new HttpNotFoundResult();
            }
            UserVM model = new UserVM()
            {
                LastName = user.LastName,
                FirstName = user.FirstName,
                Height = user.Height,
                Password = user.Password,
                BMI = user.BMI,
                Image = user.Image,
                Id = user.Id,
                Weight = user.Weight,
                Username = user.UserName,
                Email = user.Email,
                TargetAim = user.TargetAim,
                //MyExercisePlan = user.MyExercisePlan,

                ExercisePlans = user.ExercisePlans.Select(s => new ExercisePlanVM()
                {
                    Title = s.Title,
                    Description = s.Description
                })
            };
            return View(model);
        }


        private void PopulateAssignedExercisePlanData(User user)
        {
            var allExercisePlans = db.ExercisePlans;
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

        // GET: User/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users
                .Include(i => i.ExercisePlans)
                .Where(i => i.Id == id)
                .Single();
            PopulateAssignedExercisePlanData(user);
            if (user == null)
            {
                return HttpNotFound();
            }


            var enumData1 = from TargetAim e in Enum.GetValues(typeof(TargetAim))
                            select new
                            {
                                ID = (int)e,
                                Name = e.ToString()
                            };
            ViewBag.EnumList1 = new SelectList(enumData1, "ID", "Name");

            return View(user);

        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(string id, string[] selectedExercisePlans, HttpPostedFileBase file)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userToUpdate = db.Users
               .Include(i => i.ExercisePlans)
               .Where(i => i.Id == id)
               .Single();

            byte[] imageData2 = null;
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase poImgFile = Request.Files["Image"];

                using (var binary = new BinaryReader(poImgFile.InputStream))
                {
                    imageData2 = binary.ReadBytes(poImgFile.ContentLength);
                }
            }
            else
            {
                userToUpdate.Image = currentUser.Image;
                userToUpdate.ImagePath = currentUser.ImagePath;
            }

            if (TryUpdateModel(userToUpdate, "",
               new string[] { "LastName", "FirstName", "Email", "TargetAim" }))
            {
                try
                {

                    UpdateUserExercisePlans(selectedExercisePlans, userToUpdate);
                    userToUpdate.Image = imageData2;
                    db.SaveChanges();

                    return RedirectToAction("UserProfile", "User");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedExercisePlanData(userToUpdate);
            return View(userToUpdate);
        }

        private void UpdateUserExercisePlans(string[] selectedExercisePlans, User userToUpdate)
        {
            if (selectedExercisePlans == null)
            {
                userToUpdate.ExercisePlans = new List<ExercisePlan>();
                return;
            }

            var selectedExercisePlansHS = new HashSet<string>(selectedExercisePlans);
            var userExercisePlans = new HashSet<int>
                (userToUpdate.ExercisePlans.Select(c => c.ExercisePlanID));
            foreach (var exercisePlan in db.ExercisePlans)
            {
                if (selectedExercisePlansHS.Contains(exercisePlan.ExercisePlanID.ToString()))
                {
                    if (!userExercisePlans.Contains(exercisePlan.ExercisePlanID))
                    {
                        userToUpdate.ExercisePlans.Add(exercisePlan);
                    }
                }
                else
                {
                    if (userExercisePlans.Contains(exercisePlan.ExercisePlanID))
                    {
                        userToUpdate.ExercisePlans.Remove(exercisePlan);
                    }
                }
            }
        }


        // GET: User/Delete/5
        public ActionResult Delete(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id) // Delete Confirmed
        {
            try
            {
                User user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        private void PopulateAssignedExerciseData(ExercisePlan exercisePlan)
        {
            var allExercises = db.Exercises;
            var exercisePlanExercises = new HashSet<int>(exercisePlan.Exercises.Select(c => c.ExerciseID));
            var viewModel = new List<AssignedExerciseData>();
            foreach (var exercise in allExercises)
            {
                viewModel.Add(new AssignedExerciseData
                {
                    ExerciseID = exercise.ExerciseID,
                    Title = exercise.Title,
                    Assigned = exercisePlanExercises.Contains(exercise.ExerciseID)
                });
            }
            ViewBag.Exercises = viewModel;
        }
    }
}
