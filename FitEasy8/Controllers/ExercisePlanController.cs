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
using FitEasy8.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;
using System.Web.Helpers;

// Contains Controllers for both ExercisePlan and MyExercisePlan
namespace FitEasy8.Controllers
{
    public class ExercisePlanController : Controller
    {
        private FitEasyContext db = new FitEasyContext();


        private UserManager<User> manager;

        public ExercisePlanController()
        {
            db = new FitEasyContext();
            manager = new UserManager<User>(new UserStore<User>(db));
        }


        // Due to the amount of Lines, I have split this section into regions to make it easier to read.

        #region ExercisePlan IndexPage

        // GET: ExercisePlan
        public ActionResult Index2()
        {
            var viewModel = new ExercisePlanIndexData();

            viewModel.ExercisePlans = db.ExercisePlans
                //.Contains(i => i.UserID)
                .Include(i => i.Exercises.Select(c => c.BodyParts))
                .OrderBy(i => i.Title);



            return View(viewModel);
        }

        #endregion

        #region MyExercisePlan Index

        public async Task<ActionResult> Index()
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var viewModel = new ExercisePlanIndexData();

            viewModel.MyExercisePlans = db.MyExercisePlans
                .Where(i => i.UserId == currentUser.Id)
                .Include(i => i.Exercises.Select(c => c.BodyParts))
                .OrderBy(i => i.Title);


         


            double? count = 0;
            foreach (var plan in db.MyExercisePlans)
            {
                if (plan.UserId == currentUser.Id)
                {
                    count++;

                    ViewBag.Count2 = count;
                }
            }

            double? count1 = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id)
                {
                    count1++;

                }
            }

            string smessage;
            int? strength = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Strength)
                {
                    strength++;
                }
                if (strength <= 2 && count1 >= 10)
                {
                    smessage = "We notice only 1/5th or lower of your exercise types are Strength based. Perhpas you should add in a few more Strength based exericses to your future plans";

                    ViewBag.SMessage = smessage.ToString();
                }
                else
                {
                    smessage = "";
                    ViewBag.SMessage = smessage.ToString();
                }
            };

            string amessage;
            int? aerobic = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Aerobic)
                {
                    aerobic++;
                }
                if (aerobic <= 2 && count1 >= 10)
                {
                    amessage = "We notice only 1/5th of your exercise types are Aerobic based. Aerobic exercises are the most important for keeping your heart healthy. Perhpas you should add in a few more Aerobic based exericses to your future plans";

                    ViewBag.aMessage = amessage.ToString();
                }
                else
                {
                    amessage = "";
                    ViewBag.aMessage = amessage.ToString();
                }
            };

            string fmessage;
            int? flexibility = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Flexibility)
                {
                    flexibility++;
                }
                if (flexibility <= 2 && count1 >= 10)
                {
                    fmessage = "We notice only 1/5th of your exercise types are Flexibilty based. Exercises based around Flexibility are a great way to keep your muscles loose and young. Perhpas you should add in a few more Flex based exericses to your future plans";

                    ViewBag.fMessage = fmessage.ToString();
                }
                else
                {
                    fmessage = "";
                    ViewBag.fMessage = fmessage.ToString();
                }
            };

            int? reflexes = 0;
            string rmessage;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Reflexes)
                {
                    reflexes++;
                }
                if (reflexes <= 2 && count1 >= 10)
                {
                    rmessage = "We notice only 1/5th of your exercise types are Reflex based. Relex based exercises keep your mind sharp and your hand-eye coordination skill sharper.  Perhpas you should add in a few more Reflex based exericses to your future plans";

                    ViewBag.rMessage = rmessage.ToString();
                }
                else
                {
                    rmessage = "";
                    ViewBag.rMessage = rmessage.ToString();
                }
            };

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

            ViewBag.getCount = (isDone.Value / count.Value) * (100);
            ViewBag.Count = count.Value;
            ViewBag.isComplete = isDone.Value;


            return View(viewModel);
        }


        #endregion


        #region ExercisePlan Details
        // GET: ExercisePlan/Details/5
        public ActionResult Details(int? id)
        {

            var exercisePlan = db.ExercisePlans.Include(i => i.Exercises).Where(p => p.ExercisePlanID == id).FirstOrDefault();
            // note you may need to add .Include("SpecificationsTable") in the above
            if (exercisePlan == null)
            {
                return new HttpNotFoundResult();
            }
            ExercisePlanVM model = new ExercisePlanVM()
            {
                Title = exercisePlan.Title,
                Description = exercisePlan.Description,
                Exercises = exercisePlan.Exercises.Select(s => new ExerciseVM()
                {
                    Title = s.Title,
                    Description = s.Description,
                    Type = s.Type,
                    BodyPart = s.BodyPart
                })
            };
            return View(model);
        }
        #endregion

        #region MyExercisePlan Details
        // GET: ExercisePlan/Details/5
        public ActionResult Details2(int? id)
        {

            var myExercisePlan = db.MyExercisePlans.Include(i => i.Exercises).Where(p => p.MyExercisePlanID == id).FirstOrDefault();

            if (myExercisePlan == null)
            {
                return new HttpNotFoundResult();
            }
            MyExercisePlanVM model = new MyExercisePlanVM()
            {
                MyExercisePlanID = myExercisePlan.MyExercisePlanID,
                Title = myExercisePlan.Title,
                Description = myExercisePlan.Description,
                IsDone = myExercisePlan.IsDone,
                Difficulty = myExercisePlan.Difficulty,
                Reps = myExercisePlan.Reps,
                //Exercises = myExercisePlan.Exercises.Select(s => new ExerciseVM()
                //{
                //    Title = s.Title,
                //    Description = s.Description,
                //    Type = s.Type,
                //    BodyPart = s.BodyPart
                //})
            };

            ViewBag.myExercises = db.ChosenExercises.Where(p => p.ExercisePlanID == id);
            return View(model);
        }
        #endregion

        #region  Both Plans Create

        // GET: ExercisePlan/Create
        public ActionResult Create()
        {
            var exercisePlan = new ExercisePlan();
            exercisePlan.Exercises = new List<Exercise>();
            PopulateAssignedExerciseData(exercisePlan);
            var enumData = from Difficulty e in Enum.GetValues(typeof(Difficulty))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };
            ViewBag.EnumList = new SelectList(enumData, "ID", "Name");


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ExercisePlanID,Title,Description,Difficulty ")] ExercisePlan exercisePlan, string[] selectedExercises, MyExercisePlan myExercisePlan)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (selectedExercises != null)
            {
                myExercisePlan.Exercises = new List<Exercise>();
                exercisePlan.Exercises = new List<Exercise>();
                foreach (var exercise in selectedExercises)
                {
                    var exerciseToAdd = db.Exercises.Find(int.Parse(exercise));
                    exercisePlan.Exercises.Add(exerciseToAdd);
                    myExercisePlan.Exercises.Add(exerciseToAdd);

                    ChosenExercise chosenExercise = new ChosenExercise()
                    {
                        UserId = currentUser.Id,
                        ExercisePlanID = myExercisePlan.ExercisePlanID,
                        MyExercisePlanID = myExercisePlan.MyExercisePlanID,
                        Title = exerciseToAdd.Title,
                        Description = exerciseToAdd.Description,
                        Type = exerciseToAdd.Type,
                        Image = exerciseToAdd.Image,
                        ImageUrl = exerciseToAdd.ImageUrl,
                        VideoUrl = exerciseToAdd.VideoUrl,
                        Rating = exerciseToAdd.Rating,
                        IsDone = false,
                        Complete = 0,
                        Count = 1,
                        CEBodyPartID = exerciseToAdd.BodyPartId

                    };
                    db.ChosenExercises.Add(chosenExercise);



                    var bodyPart = db.BodyParts.Where(bp => bp.BodyPartID == chosenExercise.CEBodyPartID).FirstOrDefault();

                    foreach (var bp in db.BodyParts)
                    {
                        if (bp.BodyPartID == exerciseToAdd.BodyPartId)
                        {
                            ChosenBodyPart chosenBodyPart = new ChosenBodyPart()
                            {

                                ChosenExerciseID = chosenExercise.ChosenExerciseID,
                                UserId = currentUser.Id,
                                Title = bp.Title,
                                Description = bp.Description,
                                ImageUrl = bp.ImageUrl



                            };

                            db.ChosenBodyParts.Add(chosenBodyPart);
                            



                        }

                        //else
                        //{
                        //    ChosenBodyPart chosenBodyPart = new ChosenBodyPart()
                        //    {

                        //        OtherBodyPartID = chosenExercise.CEBodyPartID,
                        //        ChosenExerciseID = chosenExercise.ChosenExerciseID,
                        //        MyExercisePlanID = myExercisePlan.MyExercisePlanID,
                        //        UserId = currentUser.Id,
                        //        Title = bp.Title,
                        //        Description = bp.Description,
                        //        ImageUrl = bp.ImageUrl



                        //    };

                        //    db.ChosenBodyParts.Add(chosenBodyPart);

                        //}


                    }

                }
            }

            if (ModelState.IsValid)
            {

                myExercisePlan.UserId = currentUser.Id;
                myExercisePlan.ExercisePlanID = exercisePlan.ExercisePlanID;
                myExercisePlan.Title = exercisePlan.Title;
                myExercisePlan.Description = exercisePlan.Description;
                myExercisePlan.AddedOn = exercisePlan.AddedOn;
                myExercisePlan.UpdatedOn = exercisePlan.UpdatedOn;
                myExercisePlan.Users = exercisePlan.Users;
                myExercisePlan.AddedOn = DateTime.Now;
                myExercisePlan.IsComplete = 0;
                myExercisePlan.Count = 1;
                myExercisePlan.Difficulty = exercisePlan.Difficulty;

                if (myExercisePlan.Difficulty == Difficulty.easy)
                {
                    myExercisePlan.Reps = "For weights and strength : 3X10 of whatever weight is comfortable. For Aerobic : 5 sets of 3 minutes, taking intervals between sets.  ";
                }
                else if (myExercisePlan.Difficulty == Difficulty.medium)
                {
                    myExercisePlan.Reps = "For weights and strength : 5X5. Add extra weight so that by the end of the reps you would not be able to do one more, make sure not to go beyond that point either.. For Aerobic : 3 sets of 8 minutes, taking intervals between sets.  ";
                }
                else if (myExercisePlan.Difficulty == Difficulty.hard)
                {
                    myExercisePlan.Reps = "For weights and strength : 5X3. The weight should now be at a point where you are really straining yourself to get the 3 reps per set. For Aerobic : 2 sets of 16 minutes, taking intervals between sets.  ";
                }
                else if (myExercisePlan.Difficulty == Difficulty.extreme)
                {
                    myExercisePlan.Reps = "For weights and strength : 6X2. The weight of each rep should be barely do-able. Your veins are popping, your guns are pumping!For Aerobic : 2 set of 45 minutes, taking intervals between sets.  ";
                }


                db.ExercisePlans.Add(exercisePlan);
                db.SaveChanges();
                db.MyExercisePlans.Add(myExercisePlan);
                //currentUser.ExercisePlans.Add(exercisePlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateAssignedExerciseData(exercisePlan);

            return View(exercisePlan);
        }

        #endregion

        #region ExercisePlan Edits


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


        // GET: ExercisePlan/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExercisePlan exercisePlan = db.ExercisePlans
                .Include(i => i.Exercises)
                .Where(i => i.ExercisePlanID == id)
                .Single();
            PopulateAssignedExerciseData(exercisePlan);
            if (exercisePlan == null)
            {
                return HttpNotFound();
            }
            return View(exercisePlan);

        }

        // POST: ExercisePlan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedExercises)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var exercisePlanToUpdate = db.ExercisePlans
               .Include(i => i.Exercises)
               .Where(i => i.ExercisePlanID == id)
               .Single();

            if (TryUpdateModel(exercisePlanToUpdate, "",
               new string[] { "Title", "Description" }))
            {
                try
                {


                    UpdateWorkOutPlanExercises(selectedExercises, exercisePlanToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedExerciseData(exercisePlanToUpdate);
            return View(exercisePlanToUpdate);
        }


        private void UpdateWorkOutPlanExercises(string[] selectedExercises, ExercisePlan exercisePlanToUpdate)
        {
            if (selectedExercises == null)
            {
                exercisePlanToUpdate.Exercises = new List<Exercise>();
                return;
            }

            var selectedExercisesHS = new HashSet<string>(selectedExercises);
            var exercisePlanExercises = new HashSet<int>
                (exercisePlanToUpdate.Exercises.Select(c => c.ExerciseID));
            foreach (var exercise in db.Exercises)
            {
                if (selectedExercisesHS.Contains(exercise.ExerciseID.ToString()))
                {
                    if (!exercisePlanExercises.Contains(exercise.ExerciseID))
                    {
                        exercisePlanToUpdate.Exercises.Add(exercise);
                    }
                }
                else
                {
                    if (exercisePlanExercises.Contains(exercise.ExerciseID))
                    {
                        exercisePlanToUpdate.Exercises.Remove(exercise);
                    }
                }
            }
        }

        #endregion

        #region    MyExercisePlan Edits


        private void PopulateAssignedExerciseData2(MyExercisePlan myExercisePlan)
        {
            var allExercises = db.Exercises;
            var myExercisePlanExercises = new HashSet<int>(myExercisePlan.Exercises.Select(c => c.ExerciseID));
            var viewModel = new List<AssignedExerciseData>();
            foreach (var exercise in allExercises)
            {
                viewModel.Add(new AssignedExerciseData
                {
                    ExerciseID = exercise.ExerciseID,
                    Title = exercise.Title,
                    Assigned = myExercisePlanExercises.Contains(exercise.ExerciseID)
                });
            }
            ViewBag.Exercises = viewModel;
        }




        // GET: ExercisePlan/Edit/5
        public ActionResult Edit2(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyExercisePlan myExercisePlan = db.MyExercisePlans
                .Include(i => i.Exercises)
                .Where(i => i.MyExercisePlanID == id)
                .Single();
            PopulateAssignedExerciseData2(myExercisePlan);
            if (myExercisePlan == null)
            {
                return HttpNotFound();
            }
            var enumData = from Difficulty e in Enum.GetValues(typeof(Difficulty))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };
            ViewBag.EnumList = new SelectList(enumData, "ID", "Name");
            return View(myExercisePlan);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2(int? id, string[] selectedExercises)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var myExercisePlanToUpdate = db.MyExercisePlans
               .Include(i => i.Exercises)
               .Where(i => i.MyExercisePlanID == id)
               .Single();


            if (TryUpdateModel(myExercisePlanToUpdate, "",
               new string[] { "Title", "Description" }))
            {
                try
                {

                    myExercisePlanToUpdate.IsDone = false;
                    UpdateWorkOutPlanExercises2(selectedExercises, myExercisePlanToUpdate);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedExerciseData2(myExercisePlanToUpdate);
            return View(myExercisePlanToUpdate);
        }



        private void UpdateWorkOutPlanExercises2(string[] selectedExercises, MyExercisePlan myExercisePlanToUpdate)
        {
            if (selectedExercises == null)
            {
                myExercisePlanToUpdate.Exercises = new List<Exercise>();
                return;
            }

            var selectedExercisesHS = new HashSet<string>(selectedExercises);
            var myExercisePlanExercises = new HashSet<int>
                (myExercisePlanToUpdate.Exercises.Select(c => c.ExerciseID));
            foreach (var exercise in db.Exercises)
            {
                if (selectedExercisesHS.Contains(exercise.ExerciseID.ToString()))
                {
                    if (!myExercisePlanExercises.Contains(exercise.ExerciseID))
                    {
                        myExercisePlanToUpdate.Exercises.Add(exercise);
                    }
                }
                else
                {
                    if (myExercisePlanExercises.Contains(exercise.ExerciseID))
                    {
                        myExercisePlanToUpdate.Exercises.Remove(exercise);
                    }
                }
            }
        }
        #endregion

        #region ExercisePlan Delete
        // GET: ExercisePlan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExercisePlan exercisePlan = db.ExercisePlans.Find(id);
            if (exercisePlan == null)
            {
                return HttpNotFound();
            }
            return View(exercisePlan);
        }

        // POST: ExercisePlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExercisePlan exercisePlan = db.ExercisePlans.Find(id);
            db.ExercisePlans.Remove(exercisePlan);
            db.SaveChanges();
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
        #endregion

        #region MyExercisePlanDelete
        public ActionResult Delete2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyExercisePlan myExercisePlan = db.MyExercisePlans.Find(id);
            if (myExercisePlan == null)
            {
                return HttpNotFound();
            }
            return View(myExercisePlan);
        }

        // POST: ExercisePlan/Delete/5
        [HttpPost, ActionName("Delete2")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed2(int id)
        {
            MyExercisePlan myExercisePlan = db.MyExercisePlans.Find(id);
            db.MyExercisePlans.Remove(myExercisePlan);
            db.SaveChanges();

            var exercises = db.ChosenExercises.Where(e => e.ExercisePlanID == id);
            foreach (var exercise in exercises)
            {
                db.ChosenExercises.Remove(exercise);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        #endregion

        private void PopulateAssignedExerciseData3(MyExercisePlan myExercisePlan)
        {
            var allExercises = db.Exercises;
            var myExercisePlanExercises = new HashSet<int>(myExercisePlan.Exercises.Select(c => c.ExerciseID));
            var viewModel = new List<AssignedExerciseData>();
            foreach (var exercise in allExercises)
            {
                viewModel.Add(new AssignedExerciseData
                {
                    ExerciseID = exercise.ExerciseID,
                    Title = exercise.Title,
                    Assigned = myExercisePlanExercises.Contains(exercise.ExerciseID)
                });
            }
            ViewBag.Exercises = viewModel;
        }


        public async Task<ActionResult> AddToPlan(int id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            // Retrieve the plan from the database
            var addedExercisePlan = db.ExercisePlans
                .Single(exercisePlan => exercisePlan.ExercisePlanID == id);

            MyExercisePlan myExercisePlan = new MyExercisePlan()
            {
                ExercisePlanID = addedExercisePlan.ExercisePlanID,
                UserId = currentUser.Id,
                Title = addedExercisePlan.Title,
                Description = addedExercisePlan.Description,
                Exercises = addedExercisePlan.Exercises,
                AddedOn = DateTime.Now,
                IsComplete = 0,
                IsDone = false,
                Difficulty = addedExercisePlan.Difficulty



            };

            db.MyExercisePlans.Add(myExercisePlan);
            db.SaveChanges();

            myExercisePlan.Exercises = new List<Exercise>();
            foreach (var exercise in addedExercisePlan.Exercises)
            {
                myExercisePlan.Exercises.Add(exercise);
            }

            db.SaveChanges();

            var exercises = this.db.ChosenExercises
                            .Where(c => c.ExercisePlanID == id);
            foreach (var exercise in exercises)
            {
                ChosenExercise myExercise = new ChosenExercise()
                {
                    ExercisePlanID = myExercisePlan.MyExercisePlanID,
                    MyExercisePlanID = myExercisePlan.MyExercisePlanID,
                    UserId = currentUser.Id,
                    Title = exercise.Title,
                    Description = exercise.Description,
                    IsDone = false,
                    CEBodyPartID = exercise.CEBodyPartID,
                    Type = exercise.Type,
                    Image = exercise.Image,
                    ImageUrl = exercise.ImageUrl,
                    VideoUrl = exercise.VideoUrl,
                    Rating = exercise.Rating,
                    Complete = 0,
                    Count = 1


                };
                db.ChosenExercises.Add(myExercise);
                db.SaveChanges();
            }


            return RedirectToAction("Index");
        }


        #region MyExercisePlan Done?

        public async Task<ActionResult> Done(int id, Achievement achievement)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            // Retrieve the plan from the database
            var myExercisePlan = db.MyExercisePlans
                .Single(exercisePlan => exercisePlan.MyExercisePlanID == id);

            if (myExercisePlan.IsDone == false)
            {


                myExercisePlan.IsDone = true;
                db.SaveChanges();
                myExercisePlan.IsComplete++;
                db.SaveChanges();
                currentUser.PlansCompleted++;
                db.SaveChanges();



                if (myExercisePlan.IsComplete == 5)
                {
                    achievement = new Achievement { UserId = currentUser.Id, ExercisePlanName = myExercisePlan.Title, Date = DateTime.Now, Description = "Congratulations, you have completed your Exercise Plan 5 times! Keep up the good work!" };
                    db.Achievements.Add(achievement);
                    db.SaveChanges();
                }
                if (myExercisePlan.IsComplete == 10)
                {
                    achievement = new Achievement { UserId = currentUser.Id, ExercisePlanName = myExercisePlan.Title, Date = DateTime.Now, Description = "Congratulations, you have completed your Exercise Plan 10 times! You are really putting in some effort. Here have this online voucher on us! Use the code provided on WWW.SportingClothingShop.COM to get 15% off any clothing! Voucher Code : X2342J21" };
                    db.Achievements.Add(achievement);
                    db.SaveChanges();
                }
                if (myExercisePlan.IsComplete == 20)
                {
                    achievement = new Achievement { UserId = currentUser.Id, ExercisePlanName = myExercisePlan.Title, Date = DateTime.Now, Description = "Congratulations, you have completed your Exercise Plan 20! You must be working your ass off. Here have this online voucher code! Use the code provided on WWW.SportingClothesShop.COM to get 25% off any clothing or footwear! Voucher Code : P1324gre908. " };
                    db.Achievements.Add(achievement);
                    db.SaveChanges();
                }

                var chosenExercises = db.ChosenExercises
                    .Where(exercise => exercise.ExercisePlanID == myExercisePlan.MyExercisePlanID);

                foreach (var exercise in chosenExercises)
                {
                    exercise.IsDone = true;
                   
                    exercise.Complete++;
                    currentUser.ExercisesCompleted++;
                }
                db.SaveChanges();
            }

            else
            {

                myExercisePlan.IsDone = false;
                db.SaveChanges();

                var chosenExercises = db.ChosenExercises
                    .Where(exercise => exercise.ExercisePlanID == myExercisePlan.MyExercisePlanID);

                foreach (var exercise in chosenExercises)
                {
                    exercise.IsDone = false;
                    
                }
                db.SaveChanges();
            }


            return RedirectToAction("Index");
        }



        public async Task<ActionResult> ExerciseDone(int id, Achievement achievement)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            // Retrieve the plan from the database
            var chosenExercise = db.ChosenExercises
                .Single(exercise => exercise.ChosenExerciseID == id);

            chosenExercise.IsDone = true;
            currentUser.ExercisesCompleted++;
            // chosenExercise.Complete++;
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


            return RedirectToAction("Details2", new { id = chosenExercise.ExercisePlanID });
        }

        public async Task<ActionResult> ExerciseNotDone(int id)
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


            return RedirectToAction("Details2", new { id = chosenExercise.ExercisePlanID });
        }
        #endregion

        #region  Difficulty Setting
        public async Task<ActionResult> Harder(int id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            // Retrieve the plan from the database
            var myExercisePlan = db.MyExercisePlans
                .Single(exercisePlan => exercisePlan.MyExercisePlanID == id);
            if (myExercisePlan.Difficulty == Difficulty.easy)
            {
                myExercisePlan.Difficulty = Difficulty.medium;
                db.SaveChanges();
            }

            else if (myExercisePlan.Difficulty == Difficulty.medium)
            {
                myExercisePlan.Difficulty = Difficulty.hard;
                db.SaveChanges();
            }

            else if (myExercisePlan.Difficulty == Difficulty.hard)
            {
                myExercisePlan.Difficulty = Difficulty.extreme;
                db.SaveChanges();
            }

            if (myExercisePlan.Difficulty == Difficulty.easy)
            {
                myExercisePlan.Reps = "For weights and strength : 3X10 of whatever weight is comfortable. For Aerobic : 5 sets of 3 minutes, taking intervals between sets.  ";
            }
            else if (myExercisePlan.Difficulty == Difficulty.medium)
            {
                myExercisePlan.Reps = "For weights and strength : 5X5. Add extra weight so that by the end of the reps you would not be able to do one more, make sure not to go beyond that point either.. For Aerobic : 3 sets of 8 minutes, taking intervals between sets.  ";
            }
            else if (myExercisePlan.Difficulty == Difficulty.hard)
            {
                myExercisePlan.Reps = "For weights and strength : 5X3. The weight should now be at a point where you are really straining yourself to get the 3 reps per set. For Aerobic : 2 sets of 16 minutes, taking intervals between sets.  ";
            }
            else if (myExercisePlan.Difficulty == Difficulty.extreme)
            {
                myExercisePlan.Reps = "For weights and strength : 6X2. The weight of each rep should be barely do-able. Your veins are popping, your guns are pumping!For Aerobic : 2 set of 45 minutes, taking intervals between sets.  ";
            }

            db.SaveChanges();

            return RedirectToAction("Details2", new { id = myExercisePlan.MyExercisePlanID });

        }

        public async Task<ActionResult> Easier(int id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            // Retrieve the plan from the database
            var myExercisePlan = db.MyExercisePlans
                .Single(exercisePlan => exercisePlan.MyExercisePlanID == id);
            if (myExercisePlan.Difficulty == Difficulty.extreme)
            {
                myExercisePlan.Difficulty = Difficulty.hard;
                db.SaveChanges();
            }

            else if (myExercisePlan.Difficulty == Difficulty.medium)
            {
                myExercisePlan.Difficulty = Difficulty.easy;
                db.SaveChanges();
            }

            else if (myExercisePlan.Difficulty == Difficulty.hard)
            {
                myExercisePlan.Difficulty = Difficulty.medium;
                db.SaveChanges();
            }


            if (myExercisePlan.Difficulty == Difficulty.easy)
            {
                myExercisePlan.Reps = "For weights and strength : 3X10 of whatever weight is comfortable. For Aerobic : 5 sets of 3 minutes, taking intervals between sets.  ";
            }
            else if (myExercisePlan.Difficulty == Difficulty.medium)
            {
                myExercisePlan.Reps = "For weights and strength : 5X5. Add extra weight so that by the end of the reps you would not be able to do one more, make sure not to go beyond that point either.. For Aerobic : 3 sets of 8 minutes, taking intervals between sets.  ";
            }
            else if (myExercisePlan.Difficulty == Difficulty.hard)
            {
                myExercisePlan.Reps = "For weights and strength : 5X3. The weight should now be at a point where you are really straining yourself to get the 3 reps per set. For Aerobic : 2 sets of 16 minutes, taking intervals between sets.  ";
            }
            else if (myExercisePlan.Difficulty == Difficulty.extreme)
            {
                myExercisePlan.Reps = "For weights and strength : 4X2. The weight of each rep should be barely do-able. Your veins are popping, your guns are pumping!For Aerobic : 2 set of 45 minutes, taking intervals between sets.  ";
            }

            db.SaveChanges();
            return RedirectToAction("Details2", new { id = myExercisePlan.MyExercisePlanID });
        }
        #endregion

        #region Charts

        public async System.Threading.Tasks.Task<ActionResult> CharterColumn()

        {


            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());



            int? strength = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Strength)
                {
                    strength++;
                }
            };
            int? aerobic = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Aerobic)
                {
                    aerobic++;
                }
            };

            int? flexibility = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Flexibility)
                {
                    flexibility++;
                }

            };
            int? reflexes = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Reflexes)
                {
                    reflexes++;
                }
            };



            string[] xValue = new string[] { "Strength", "Aerobic", "Flexibility", "Reflexes" };

            int[] yValue = new int[] { strength.Value, aerobic.Value, flexibility.Value, reflexes.Value };

            new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

              .AddTitle("Types of Exercises")

             .AddSeries("Default", chartType: "column", xValue: xValue, yValues: yValue)

                   .Write("bmp");

            return null;

        }


        public async System.Threading.Tasks.Task<ActionResult> ChartPie()

        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            int? isDone = 0;
            foreach (var plan in db.MyExercisePlans)
            {
                if (plan.UserId == currentUser.Id && plan.IsDone == true)
                {
                    isDone++;
                }
            };
            int? notDone = 0;
            foreach (var plan in db.MyExercisePlans)
            {
                if (plan.UserId == currentUser.Id && plan.IsDone == false)
                {
                    notDone++;
                }
            };

            if (isDone >= 1 && notDone >= 1)
            {
                string[] xValue = new string[] { "Complete : " + isDone.Value, "Not Complete : " + notDone.Value };
                int[] yValue = new int[] { isDone.Value, notDone.Value };
                new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

                .AddTitle("Is Complete / Not Complete")

               .AddSeries("Default", chartType: "Pie", xValue: xValue, yValues: yValue)

                      .Write("bmp");
                return null;
            }
            else if (isDone >= 1 && notDone == 0)
            {
                string[] xValue = new string[] { "Complete : " + isDone.Value };
                int[] yValue = new int[] { isDone.Value };
                new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

                .AddTitle("IsComplete/NotComplete")

               .AddSeries("Default", chartType: "Pie", xValue: xValue, yValues: yValue)

                      .Write("bmp");
                return null;
            }
            else if (isDone == 0 && notDone >= 1)
            {
                string[] xValue = new string[] { "Not Complete : " + notDone.Value };
                int[] yValue = new int[] { notDone.Value };
                new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

                .AddTitle("IsComplete/NotComplete")

               .AddSeries("Default", chartType: "Pie", xValue: xValue, yValues: yValue)

                      .Write("bmp");
                return null;
            }


            return null;

        }

        public async System.Threading.Tasks.Task<ActionResult> ChartPieType()

        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            double? count1 = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id)
                {
                    count1++;

                }
            }

            int? strength = 0;
            double? spercent = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Strength)
                {
                    strength++;
                }

                spercent = (strength.Value / count1.Value) * (100);


            };

            int? aerobic = 0;
            double? apercent = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Aerobic)
                {
                    aerobic++;
                }


                apercent = (aerobic.Value / count1.Value) * (100);

            };

            int? flexibility = 0;
            double? fpercent = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Flexibility)
                {
                    flexibility++;
                }


                fpercent = (flexibility.Value / count1.Value) * (100);

            };

            int? reflexes = 0;
            double? rpercent = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Reflexes)
                {
                    reflexes++;
                }

                rpercent = (reflexes.Value / count1.Value) * (100);

            };


            string[] xValue = new string[] { "Strength : " + spercent.Value, "Aerobic : " + apercent.Value, "Flex" + fpercent.Value, "Reflex : " + rpercent.Value };
            double[] yValue = new double[] { spercent.Value, apercent.Value, fpercent.Value, rpercent.Value };
            new Chart(width: 700, height: 500, theme: ChartTheme.Vanilla)

            .AddTitle("Type %'s")

           .AddSeries("Default", chartType: "Pie", xValue: xValue, yValues: yValue)

                  .Write("bmp");

            return null;



        }

        #endregion

        #region Statistics and Suggestions

        public async Task<ActionResult> Stats()
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            double? count1 = 0;
            foreach (var exercise in db.ChosenExercises)
            {
                if (exercise.UserId == currentUser.Id)
                {
                    count1++;

                }
            }

            string smessage;
            int? strength = 0;
            double? spercent;

            //results based on TargetAim
            #region TargetAim == Other Suggestions
            if (currentUser.TargetAim == TargetAim.Other)
            {

                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Strength)
                    {
                        strength++;
                    }

                    spercent = (strength.Value / count1.Value) * (100);
                    if (spercent <= 20) //strength <= 2 && count1 >= 10
                    {
                        smessage = "We noticed only " + spercent + "% of your exercise types are Strength based. Perhpas you should add  a few more Strength based exercises to your future plans";

                        ViewBag.SMessage = smessage.ToString();
                    }

                    else if (spercent >= 75)
                    {
                        smessage = "We noticed " + spercent + "% of your exercise types are Strength based. Unless you are purely trying to gain muscle, you should add a few different based exercises to your future plans";

                        ViewBag.SMessage = smessage.ToString();
                    }

                    else
                    {
                        smessage = "";
                        ViewBag.SMessage = smessage.ToString();
                    }
                };

                string amessage;
                int? aerobic = 0;
                double? apercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Aerobic)
                    {
                        aerobic++;
                    }


                    apercent = (aerobic.Value / count1.Value) * (100);
                    if (apercent <= 20)
                    {
                        amessage = "We noticed only " + apercent + "% of your exercise types are Aerobic based. Aerobic exercises are the most important for keeping your heart healthy. Perhpas you should add in a few more Aerobic based exericses to your future plans";

                        ViewBag.aMessage = amessage.ToString();
                    }
                    else if (apercent >= 75)
                    {
                        amessage = "We noticed " + apercent + "% of your exercise types are Aerobic based. Unless you are purely trying to lose muscle or weight, you should add a few different based exercises to your future plans";

                        ViewBag.aMessage = amessage.ToString();
                    }
                    else
                    {
                        amessage = "";
                        ViewBag.aMessage = amessage.ToString();
                    }
                };

                string fmessage;
                int? flexibility = 0;
                double? fpercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Flexibility)
                    {
                        flexibility++;
                    }


                    fpercent = (flexibility.Value / count1.Value) * (100);
                    if (fpercent <= 20)
                    {
                        fmessage = "We noticed only " + fpercent + "% of your exercise types are Flexibilty based. Exercises based around Flexibility are a great way to keep your muscles loose and young. Perhpas you should add in a few more Flex based exericses to your future plans";

                        ViewBag.fMessage = fmessage.ToString();
                    }
                    else if (fpercent >= 75)
                    {
                        fmessage = "We noticed " + fpercent + "% of your exercise types are Flexibility based. If you have no interest in gaining muscle, you should add a few different Reflex or Aerobic based exercises to your future plans";

                        ViewBag.fMessage = fmessage.ToString();
                    }
                    else
                    {
                        fmessage = "";
                        ViewBag.fMessage = fmessage.ToString();
                    }
                };

                int? reflexes = 0;
                string rmessage;
                double? rpercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Reflexes)
                    {
                        reflexes++;
                    }

                    rpercent = (reflexes.Value / count1.Value) * (100);
                    if (rpercent <= 20)
                    {
                        rmessage = "We noticed only " + rpercent + "% of your exercise types are Reflex based. Reflex based exercises keep your mind sharp and your hand-eye coordination skill sharper.  Perhpas you should add in a few more Reflex based exericses to your future plans";

                        ViewBag.rMessage = rmessage.ToString();
                    }
                    else if (rpercent >= 75)
                    {
                        rmessage = "We noticed " + rpercent + "% of your exercise types are Reflex based. Unless you have no interest in trying to gain muscle, you should add a few different Flexibility or Aerobic based exercises to your future plans";

                        ViewBag.rMessage = rmessage.ToString();
                    }
                    else
                    {
                        rmessage = "";
                        ViewBag.rMessage = rmessage.ToString();
                    }
                };

                double? isDonepercent = 0;
                string isdmessage;
                double? isDone = 0;
                double? count2 = 0;
                foreach (var plan in db.MyExercisePlans)
                {
                    if (plan.UserId == currentUser.Id)
                    {
                        count2++;
                    }

                    if (plan.UserId == currentUser.Id && plan.IsDone == true)
                    {
                        isDone++;
                    }

                    isDonepercent = (isDone.Value / count2.Value) * (100);
                    if (isDonepercent <= 65)
                    {
                        isdmessage = "You have only completed " + isDonepercent.Value + "% of your plans. Pick up the pace and Keeping going!";

                        ViewBag.isdMessage = isdmessage.ToString();
                    }
                    else if (isDonepercent <= 85)
                    {
                        isdmessage = "You have completed " + isDonepercent.Value + "% of your plans. You are doing some good work! Keeping going!";

                        ViewBag.isdMessage = isdmessage.ToString();
                    }

                    else
                    {
                        isdmessage = "Well done! All Exercise Plans are Complete";
                        ViewBag.isdMessage = isdmessage.ToString();
                    }
                };



                return View();

            }
            #endregion

            #region TargetAim == GainMuscle Suggestions
            else if (currentUser.TargetAim == TargetAim.GainMuscle)
            {

                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Strength)
                    {
                        strength++;
                    }

                    spercent = (strength.Value / count1.Value) * (100);
                    if (spercent <= 45) //strength <= 2 && count1 >= 10
                    {
                        smessage = "We noticed only " + spercent + "% of your exercise types are Strength based. If you are trying to Gain Muscle Strength based exercises are the most important. You should add more Strength based exercises to your future plans";

                        ViewBag.SMessage = smessage.ToString();
                    }

                    else if (spercent >= 80)
                    {
                        smessage = "We noticed " + spercent + "% of your exercise types are Strength based. As you have set your Target Aim to gain muscle, You will need a lot of strength based exercises which you do. Make sure you don't neglect other based exercises as They are important for Healthy hearts, loosening muscles and keeping you sharp.";

                        ViewBag.SMessage = smessage.ToString();
                    }

                    else
                    {
                        smessage = "";
                        ViewBag.SMessage = smessage.ToString();
                    }
                };

                string amessage;
                int? aerobic = 0;
                double? apercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Aerobic)
                    {
                        aerobic++;
                    }


                    apercent = (aerobic.Value / count1.Value) * (100);
                    if (apercent <= 20)
                    {
                        amessage = "We noticed only " + apercent + "% of your exercise types are Aerobic based. Aerobic exercises are the most important for keeping your heart healthy. Perhpas you should add in a few more Aerobic based exericses to your future plans";

                        ViewBag.aMessage = amessage.ToString();
                    }
                    else if (apercent >= 35)
                    {
                        amessage = apercent + "% of your exercise types are Aerobic based.You have set your Target Aim to Gain Muscle. Aerobic exercises are generally used to lose weight. Make sure you have enough Strength based exercises in your future plans. ";
                        ViewBag.aMessage = amessage.ToString();
                    }
                    else
                    {
                        amessage = "";
                        ViewBag.aMessage = amessage.ToString();
                    }
                };

                string fmessage;
                int? flexibility = 0;
                double? fpercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Flexibility)
                    {
                        flexibility++;
                    }


                    fpercent = (flexibility.Value / count1.Value) * (100);
                    if (fpercent <= 5)
                    {
                        fmessage = "We noticed only " + fpercent + "% of your exercise types are Flexibilty based. Exercises based around Flexibility are a great way to keep your muscles loose and young. Perhpas you should add in a few more Flex based exericses to your future plans";

                        ViewBag.fMessage = fmessage.ToString();
                    }
                    else if (fpercent >= 50)
                    {
                        fmessage = "We noticed " + fpercent + "% of your exercise types are Flexibility based. We suggest you lose a few in order to make room for more strength based exercises. You do not have to drop all Flex based exercises but in order to gain muscle you need hefty percent of strength based exercises.";
                        ViewBag.fMessage = fmessage.ToString();
                    }
                    else
                    {
                        fmessage = "";
                        ViewBag.fMessage = fmessage.ToString();
                    }
                };

                int? reflexes = 0;
                string rmessage;
                double? rpercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Reflexes)
                    {
                        reflexes++;
                    }

                    rpercent = (reflexes.Value / count1.Value) * (100);
                    if (rpercent <= 5)
                    {
                        rmessage = "We noticed only " + rpercent + "% of your exercise types are Reflex based. Reflex based exercises keep your mind sharp and your hand-eye coordination skill sharper.  Perhpas you should add in a few more Reflex based exericses to your future plans";

                        ViewBag.rMessage = rmessage.ToString();
                    }
                    else if (rpercent >= 45)
                    {
                        rmessage = "We noticed " + rpercent + "% of your exercise types are Reflex based. Make sure you have the right balance betwen exercise types to best suit your aim of gaining muscle";

                        ViewBag.rMessage = rmessage.ToString();
                    }
                    else
                    {
                        rmessage = "";
                        ViewBag.rMessage = rmessage.ToString();
                    }
                };

                double? isDonepercent = 0;
                string isdmessage;
                double? isDone = 0;
                double? count2 = 0;
                foreach (var plan in db.MyExercisePlans)
                {
                    if (plan.UserId == currentUser.Id)
                    {
                        count2++;
                    }

                    if (plan.UserId == currentUser.Id && plan.IsDone == true)
                    {
                        isDone++;
                    }

                    isDonepercent = (isDone.Value / count2.Value) * (100);
                    if (isDonepercent <= 65)
                    {
                        isdmessage = "You have only completed " + isDonepercent.Value + "% of your plans. Pick up the pace and Keeping going!";

                        ViewBag.isdMessage = isdmessage.ToString();
                    }
                    else if (isDonepercent <= 85)
                    {
                        isdmessage = "You have completed " + isDonepercent.Value + "% of your plans. You are doing some good work! Keeping going!";

                        ViewBag.isdMessage = isdmessage.ToString();
                    }

                    else
                    {
                        isdmessage = "Well done! All Exercise Plans are Complete";
                        ViewBag.isdMessage = isdmessage.ToString();
                    }
                };



                return View();

            }



            #endregion

            #region TargetAim == LoseWeight Suggestions

            else
            {

                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Strength)
                    {
                        strength++;
                    }

                    spercent = (strength.Value / count1.Value) * (100);
                    if (spercent <= 10) //strength <= 2 && count1 >= 10
                    {
                        smessage = "We noticed only " + spercent + "% of your exercise types are Strength based. Aerobic based exercises are the best way for you to lose weight but you also need a healthy balance of strength and aerobic. You should add a few more Strength based exercises in future.";

                        ViewBag.SMessage = smessage.ToString();
                    }

                    else if (spercent >= 50)
                    {
                        smessage = "We noticed " + spercent + "% of your exercise types are Strength based. This will ultimately help in the loss of fat and the gain of muscle. You may not lose weight but it will be healthy weight you are gaining. If you purely want to lose weight try adding more aerobic based exercises. ";

                        ViewBag.SMessage = smessage.ToString();
                    }

                    else
                    {
                        smessage = "";
                        ViewBag.SMessage = smessage.ToString();
                    }
                };

                string amessage;
                int? aerobic = 0;
                double? apercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Aerobic)
                    {
                        aerobic++;
                    }


                    apercent = (aerobic.Value / count1.Value) * (100);
                    if (apercent <= 45)
                    {
                        amessage = "We noticed only " + apercent + "% of your exercise types are Aerobic based. Aerobic exercises are the most important for keeping your heart healthy. You need to add more Aerobic based exercises in order to lose weight.";

                        ViewBag.aMessage = amessage.ToString();
                    }
                    else if (apercent >= 85)
                    {
                        amessage = apercent + "% of your exercise types are Aerobic based.You have set your Target Aim to Lose Weight. Although Aerobic exercises are the best way to lose weight, Make sure you have a healthy mix of exercise types and aren't neglecting any";
                        ViewBag.aMessage = amessage.ToString();
                    }
                    else
                    {
                        amessage = "";
                        ViewBag.aMessage = amessage.ToString();
                    }
                };

                string fmessage;
                int? flexibility = 0;
                double? fpercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Flexibility)
                    {
                        flexibility++;
                    }


                    fpercent = (flexibility.Value / count1.Value) * (100);
                    if (fpercent <= 5)
                    {
                        fmessage = "We noticed only " + fpercent + "% of your exercise types are Flexibilty based. Exercises based around Flexibility are a great way to keep your muscles loose and young. Perhpas you should add in a few more Flex based exericses to your future plans";

                        ViewBag.fMessage = fmessage.ToString();
                    }
                    else if (fpercent >= 50)
                    {
                        fmessage = "We noticed " + fpercent + "% of your exercise types are Flexibility based. We suggest you lose a few in order to make room for more Aerobic based exercises. You do not have to drop all Flex based exercises but in order to lose weight you need hefty percent of aerobic and strength based exercises.";
                        ViewBag.fMessage = fmessage.ToString();
                    }
                    else
                    {
                        fmessage = "";
                        ViewBag.fMessage = fmessage.ToString();
                    }
                };

                int? reflexes = 0;
                string rmessage;
                double? rpercent;
                foreach (var exercise in db.ChosenExercises)
                {
                    if (exercise.UserId == currentUser.Id && exercise.Type == Models.Type.Reflexes)
                    {
                        reflexes++;
                    }

                    rpercent = (reflexes.Value / count1.Value) * (100);
                    if (rpercent <= 5)
                    {
                        rmessage = "We noticed only " + rpercent + "% of your exercise types are Reflex based. Reflex based exercises keep your mind sharp and your hand-eye coordination skill sharper.  Perhpas you should add in a few more Reflex based exericses to your future plans";

                        ViewBag.rMessage = rmessage.ToString();
                    }
                    else if (rpercent >= 45)
                    {
                        rmessage = "We noticed " + rpercent + "% of your exercise types are Reflex based. Make sure you have the right balance betwen exercise types to best suit your aim of losing Weight";

                        ViewBag.rMessage = rmessage.ToString();
                    }
                    else
                    {
                        rmessage = "";
                        ViewBag.rMessage = rmessage.ToString();
                    }
                };

                double? isDonepercent = 0;
                string isdmessage;
                double? isDone = 0;
                double? count2 = 0;
                foreach (var plan in db.MyExercisePlans)
                {
                    if (plan.UserId == currentUser.Id)
                    {
                        count2++;
                    }

                    if (plan.UserId == currentUser.Id && plan.IsDone == true)
                    {
                        isDone++;
                    }

                    isDonepercent = (isDone.Value / count2.Value) * (100);
                    if (isDonepercent <= 65)
                    {
                        isdmessage = "You have only completed " + isDonepercent.Value + "% of your plans. Pick up the pace and Keeping going!";

                        ViewBag.isdMessage = isdmessage.ToString();
                    }
                    else if (isDonepercent <= 85)
                    {
                        isdmessage = "You have completed " + isDonepercent.Value + "% of your plans. You are doing some good work! Keeping going!";

                        ViewBag.isdMessage = isdmessage.ToString();
                    }

                    else
                    {
                        isdmessage = "Well done! All Exercise Plans are Complete";
                        ViewBag.isdMessage = isdmessage.ToString();
                    }
                };



                return View();

            }

            #endregion


        }
        #endregion
    }

}



