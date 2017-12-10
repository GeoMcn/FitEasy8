using FitEasy8.DAL;
using FitEasy8.Models;
using FitEasy8.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FitEasy8.Controllers
{
    public class ChosenExerciseController : Controller
    {

        private FitEasyContext db = new FitEasyContext();


        private UserManager<User> manager;

        public ChosenExerciseController()
        {
            db = new FitEasyContext();
            manager = new UserManager<User>(new UserStore<User>(db));
        }


        // GET: ChosenExercise
        public async System.Threading.Tasks.Task<ActionResult> Index(int? id)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            var viewModel = new ChosenExerciseIndexData();


            viewModel.ChosenExercises = db.ChosenExercises
                .Where(i => i.UserId == currentUser.Id && i.ExercisePlanID == id)
                .OrderBy(i => i.Title);



            if (id != null)
            {
                ViewBag.MyExercisePlanID = id.Value;

            }


            double? count = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.ExercisePlanID == id)
                {
                    count++;

                    ViewBag.Count2 = count;
                }
            }

            double? isDone = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.IsDone == true && exercise.ExercisePlanID == id)
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
                ViewBag.message = "Wow!! You have completed all of your Exercises! Well Done! Try sticking with your Exercise Plan for at least 6 weeks before raising the difficulty or creating a new Plan.";

            }



            ViewBag.getCount = (isDone.Value / count.Value) * (100);
            ViewBag.Count = count.Value;
            ViewBag.isComplete = isDone.Value;



            return View(viewModel);
        }

        public async System.Threading.Tasks.Task<ActionResult> Index2(int? id)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            var viewModel = new ChosenExerciseIndexData();


            viewModel.ChosenExercises = db.ChosenExercises
                .Where(i => i.UserId == currentUser.Id && i.MyExercisePlanID == id)
                .OrderBy(i => i.Title);


            if (id != null)
            {
                ViewBag.MyExercisePlanID = id.Value;

            }


            double? count = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.ExercisePlanID == id)
                {
                    count++;

                    ViewBag.Count2 = count;
                }
            }

            double? isDone = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.IsDone == true && exercise.ExercisePlanID == id)
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
                ViewBag.message = "Wow!! You have completed all of your Exercises! Well Done! You should set your Exercise Plan to Done.";

            }



            ViewBag.getCount = (isDone.Value / count.Value) * (100);
            ViewBag.Count = count.Value;
            ViewBag.isComplete = isDone.Value;



            return View(viewModel);
        }



        public async Task<ActionResult> Done(int id, Achievement achievement)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            // Retrieve the plan from the database
            var chosenExercise = db.ChosenExercises
                .Single(exercise => exercise.ChosenExerciseID == id);

            chosenExercise.IsDone = true;
            chosenExercise.Complete++;
            currentUser.ExercisesCompleted++;
            db.SaveChanges();

            var myExercisePlan2 = db.MyExercisePlans
               .Single(exercisePlan => exercisePlan.MyExercisePlanID == chosenExercise.ExercisePlanID);
            double? exerciseCount = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.ExercisePlanID == chosenExercise.ExercisePlanID)
                {
                    exerciseCount++;

                    ViewBag.exerciseCount2 = exerciseCount;
                }
            }
            double? exerciseIsDone = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.ExercisePlanID == chosenExercise.ExercisePlanID && exercise.IsDone == true)
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
                ViewBag.exerciseMessage = "Wow!! You have completed all of your Exercise! ";
                myExercisePlan2.IsDone = true;
                myExercisePlan2.IsComplete++;
                currentUser.PlansCompleted++;
                db.SaveChanges();
            }

            else if (exerciseCount >= 1 && exerciseIsDone != exerciseCount)
            {

                myExercisePlan2.IsDone = false;
                db.SaveChanges();
            }


            if (myExercisePlan2.IsComplete == 5)
            {
                achievement = new Achievement { UserId = currentUser.Id, ExercisePlanName = myExercisePlan2.Title, Date = DateTime.Now, Description = "Congratulations, you have completed your Exercise Plan 5 times! Keep up the good work!" };
                db.Achievements.Add(achievement);
                db.SaveChanges();
            }
            if (myExercisePlan2.IsComplete == 10)
            {
                achievement = new Achievement { UserId = currentUser.Id, ExercisePlanName = myExercisePlan2.Title, Date = DateTime.Now, Description = "Congratulations, you have completed your Exercise Plan 10 times! You are really putting in some effort. Here have this online voucher on us! Use the code provided on WWW.SportingClothingShop.COM to get 15% off any clothing! Voucher Code : X2342J21" };
                db.Achievements.Add(achievement);
                db.SaveChanges();
            }
            if (myExercisePlan2.IsComplete == 20)
            {
                achievement = new Achievement { UserId = currentUser.Id, ExercisePlanName = myExercisePlan2.Title, Date = DateTime.Now, Description = "Congratulations, you have completed your Exercise Plan 20! You must be working your ass off. Here have this online voucher code! Use the code provided on WWW.SportingClothesShop.COM to get 25% off any clothing or footwear! Voucher Code : P1324gre908. " };
                db.Achievements.Add(achievement);
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { id = chosenExercise.ExercisePlanID });
        }


        public async Task<ActionResult> NotDone(int id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var chosenExercise = db.ChosenExercises
                .Single(exercise => exercise.ChosenExerciseID == id);

            chosenExercise.IsDone = false;
            db.SaveChanges();

            var myExercisePlan2 = db.MyExercisePlans
                .Single(exercisePlan => exercisePlan.MyExercisePlanID == chosenExercise.ExercisePlanID);
            double? exerciseCount = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.ExercisePlanID == chosenExercise.ExercisePlanID)
                {
                    exerciseCount++;

                }
            }
            double? exerciseIsDone = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.ExercisePlanID == chosenExercise.ExercisePlanID && exercise.IsDone == true)
                {
                    exerciseIsDone++;
                }
            }

            if (exerciseCount == null)
            {
                exerciseCount = 0;
            }

            if (exerciseCount >= 1 && exerciseIsDone != exerciseCount)
            {

                myExercisePlan2.IsDone = false;
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = chosenExercise.ExercisePlanID });
        }



        // GET: ChosenExercise/Details/5
        public ActionResult Details(int id)
        {

            var exercise = db.ChosenExercises.Where(p => p.ChosenExerciseID == id).FirstOrDefault();

            if (exercise == null)
            {
                return new HttpNotFoundResult();
            }
            ExerciseVM viewmodel = new ExerciseVM()
            {
                Title = exercise.Title,
                Description = exercise.Description,
                Image = exercise.Image,
                ImageUrl = exercise.ImageUrl,
                Rating = exercise.Rating,
                Type = exercise.Type,
                VideoUrl = exercise.VideoUrl,
                CEBodyPartID = exercise.CEBodyPartID
                ////BodyPart = db.BodyParts.Where(p => p.BodyPartID == exercise.BodyPartId).FirstOrDefault(),

                ////BodyParts = exercise.BodyParts.Select(s => new BodyPartVM()
                ////{
                ////    Title = s.Title,
                ////    Description = s.Description
                ////})
            };

            ViewBag.bodyPartss = db.ChosenBodyParts.Where(p => p.ChosenExerciseID == id);
            //BodyPart bodyPart2 = db.BodyParts.Where(p => p.BodyPartID == bodypart.BodyPartID).FirstOrDefault();
            //ViewBag.bodyPart = bodypart.ToString();
            return View(viewmodel);

        }

        // GET: ChosenExercise/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChosenExercise/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ChosenExercise/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChosenExercise/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ChosenExercise/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChosenExercise/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //public async System.Threading.Tasks.Task<ActionResult> CharterColumn()

        //{

        //    var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
        //    var chosenExercises = db.ChosenExercises
        //        .Where(i => i.UserId == currentUser.Id);
        //    var myExercisePlan = db.MyExercisePlans.Where(p => p.MyExercisePlanID == chosenExercises.ID).FirstOrDefault();


        //    int? strength = 0;
        //    foreach (var exercise in db.ChosenExercises)
        //    {
        //        if (exercise.UserId == currentUser.Id && exercise.ExercisePlanID == myExercisePlan.MyExercisePlanID && exercise.Type == Models.Type.Strength)
        //        {
        //            strength++;
        //        }
        //    };
        //    int? aerobic = 0;
        //    foreach (var exercise in db.ChosenExercises)
        //    {
        //        if (exercise.UserId == currentUser.Id && exercise.ExercisePlanID == myExercisePlan.MyExercisePlanID && exercise.Type == Models.Type.Aerobic)
        //        {
        //            aerobic++;
        //        }
        //    };
        //    int? flexibility = 0;
        //    foreach (var exercise in db.ChosenExercises)
        //    {
        //        if (exercise.UserId == currentUser.Id && exercise.ExercisePlanID == myExercisePlan.MyExercisePlanID && exercise.Type == Models.Type.Flexibility)
        //        {
        //            flexibility++;
        //        }
        //    };
        //    int? reflexes = 0;
        //    foreach (var exercise in db.ChosenExercises)
        //    {
        //        if (exercise.UserId == currentUser.Id && exercise.ExercisePlanID == myExercisePlan.MyExercisePlanID && exercise.Type == Models.Type.Reflexes)
        //        {
        //            reflexes++;
        //        }
        //    };



        //    string[] xValue = new string[] { "Strength", "Aerobic", "Flexibility", "Reflexes" };

        //    int[] yValue = new int[] { strength.Value, aerobic.Value, flexibility.Value, reflexes.Value };

        //    new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

        //      .AddTitle("Types of Exercises")

        //     .AddSeries("Default", chartType: "column", xValue: xValue, yValues: yValue)

        //           .Write("bmp");

        //    return null;

        //}

        //public async System.Threading.Tasks.Task<ActionResult> CharterColumn()

        //{

        //    var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
        //    var myExercisePlan = db.MyExercisePlans.Where(p => p.MyExercisePlanID == id).FirstOrDefault();

        //    int? strength = 0;
        //    foreach (var exercise in db.ChosenExercises)
        //    {
        //        if (exercise.ExercisePlanID == myExercisePlan.MyExercisePlanId && exercise.Type == Models.Type.Strength)
        //        {
        //            strength++;
        //        }
        //    };
        //    int? aerobic = 0;
        //    foreach (var exercise in db.ChosenExercises)
        //    {
        //        if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Aerobic)
        //        {
        //            aerobic++;
        //        }
        //    };
        //    int? flexibility = 0;
        //    foreach (var exercise in db.ChosenExercises)
        //    {
        //        if (exercise.Type == Models.Type.Flexibility)
        //        {
        //            flexibility++;
        //        }
        //    };
        //    int? reflexes = 0;
        //    foreach (var exercise in db.ChosenExercises)
        //    {
        //        if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Reflexes)
        //        {
        //            reflexes++;
        //        }
        //    };



        //    string[] xValue = new string[] { "Strength", "Aerobic", "Flexibility", "Reflexes" };

        //    int[] yValue = new int[] { strength.Value, aerobic.Value, flexibility.Value, reflexes.Value };

        //    new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

        //      .AddTitle("Types of Exercises")

        //     .AddSeries("Default", chartType: "column", xValue: xValue, yValues: yValue)

        //           .Write("bmp");

        //    return null;

        //}

    }
}
