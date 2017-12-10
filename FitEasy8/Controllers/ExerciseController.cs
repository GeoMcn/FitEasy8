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
using System.Data.Entity.Infrastructure;
using FitEasy8.ViewModels;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FitEasy8.Controllers
{
    public class ExerciseController : Controller
    {
        private FitEasyContext db = new FitEasyContext();


        private UserManager<User> manager;

        public ExerciseController()
        {
            db = new FitEasyContext();
            manager = new UserManager<User>(new UserStore<User>(db));
        }



        // GET: Exercise
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var exercises = from s in db.Exercises
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                exercises = exercises.Where(s => s.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    exercises = exercises.OrderByDescending(s => s.Title);
                    break;

                default:
                    exercises = exercises.OrderBy(s => s.Title);
                    break;

            }
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(exercises.ToPagedList(pageNumber, pageSize));
        }
        public async System.Threading.Tasks.Task<ActionResult> SuggestedExercises(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var exercises = from s in db.Exercises
                            select s;

            if(currentUser.TargetAim == TargetAim.GainMuscle)
            {
                exercises = exercises.Where(s => s.Type == Models.Type.Strength);
            }
            else if (currentUser.TargetAim == TargetAim.LoseWeight)
            {
                exercises = exercises.Where(s => s.Type == Models.Type.Aerobic);
            }
            
            
            if (!String.IsNullOrEmpty(searchString))
            {
                exercises = exercises.Where(s => s.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    exercises = exercises.OrderByDescending(s => s.Title);
                    break;

                default:
                    exercises = exercises.OrderBy(s => s.Title);
                    break;

            }

            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(exercises.ToPagedList(pageNumber, pageSize));
        }

        // GET: Exercise/Details/5
        public ActionResult Details(int? id)
        {

            var exercise = db.Exercises.Include(i => i.BodyParts).Where(p => p.ExerciseID == id).FirstOrDefault();
            // note you may need to add .Include("SpecificationsTable") in the above
            if (exercise == null)
            {
                return new HttpNotFoundResult();
            }
            ExerciseVM model = new ExerciseVM()
            {
                Title = exercise.Title,
                Description = exercise.Description,
                Image = exercise.Image,
                Rating = exercise.Rating,
                Type = exercise.Type,
                VideoUrl = exercise.VideoUrl,
                BodyPart = db.BodyParts.Where(p => p.BodyPartID == exercise.BodyPartId).FirstOrDefault(),

                BodyParts = exercise.BodyParts.Select(s => new BodyPartVM()
                {
                    Title = s.Title,
                    Description = s.Description
                })
            };
            ViewBag.imageUrl = exercise.ImageUrl;
            ViewBag.Video = exercise.VideoUrl;
            return View(model);


        }

        // GET: Exercise/Create
        public ActionResult Create()
        {
            var exercise = new Exercise();
            exercise.BodyParts = new List<BodyPart>();
            PopulateAssignedBodyPartData(exercise);
            return View();

        }

        // POST: Exercise/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExerciseID,Title,BodyPart,Description,VideoUrl,Rating,Type")] string[] selectedBodyParts, [Bind(Exclude = "Image")] Exercise exercise, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (selectedBodyParts != null)
                    {
                        exercise.BodyParts = new List<BodyPart>();
                        foreach (var bodyPart in selectedBodyParts)
                        {
                            var bodyPartToAdd = db.BodyParts.Find(int.Parse(bodyPart));
                            exercise.BodyParts.Add(bodyPartToAdd);
                        }
                    }


                    if (ModelState.IsValid)
                    {
                        byte[] imageData = null;
                        if (Request.Files.Count > 0)
                        {
                            HttpPostedFileBase poImgFile = Request.Files["Image"];

                            using (var binary = new BinaryReader(poImgFile.InputStream))
                            {
                                imageData = binary.ReadBytes(poImgFile.ContentLength);
                            }
                        }
                        //string fileName = Path.GetFileNameWithoutExtension(exercise.ImageURL.FileName);
                        //string extension = Path.GetExtension(exercise.ImageURL.FileName);
                        //fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        //exercise.ImagePath = "~/Images/" + fileName;
                        //fileName = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        //exercise.ImageURL.SaveAs(fileName);

                        db.Exercises.Add(exercise);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }

                PopulateAssignedBodyPartData(exercise);
                return View(exercise);


            }

            return View(exercise);
        }


        private void PopulateAssignedBodyPartData(Exercise exercise)
        {
            var allBodyParts = db.BodyParts;
            var exerciseBodyParts = new HashSet<int>(exercise.BodyParts.Select(c => c.BodyPartID));
            var viewModel = new List<AssignedBodyPartData>();
            foreach (var bodyPart in allBodyParts)
            {
                viewModel.Add(new AssignedBodyPartData
                {
                    BodyPartID = bodyPart.BodyPartID,
                    Title = bodyPart.Title,
                    Assigned = exerciseBodyParts.Contains(bodyPart.BodyPartID)
                });
            }
            ViewBag.BodyParts = viewModel;
        }

        public FileContentResult ExercisePhotos()
        {
            if (User.Identity.IsAuthenticated)
            {
                String userId = User.Identity.GetUserId();

                if (userId == null)
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


        // GET: Exercise/Edit/5
        public ActionResult Edit(int? id)
        {





            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exercise exercise = db.Exercises
                .Include(i => i.BodyParts)
                .Where(i => i.ExerciseID == id)
                .Single();
            PopulateAssignedBodyPartData(exercise);
            if (exercise == null)
            {
                return HttpNotFound();
            }
            return View(exercise);




        }

        // POST: Exercise/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedBodyParts)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var exerciseToUpdate = db.Exercises
               .Include(i => i.BodyParts)
               .Where(i => i.ExerciseID == id)
               .Single();

            if (TryUpdateModel(exerciseToUpdate, "",
               new string[] { "Title", "Description", "Image", "BodyPart", "VideoUrl", "Rating", "Type" }))
            {
                try
                {


                    UpdateExerciseBodyParts(selectedBodyParts, exerciseToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedBodyPartData(exerciseToUpdate);
            return View(exerciseToUpdate);

        }



        private void UpdateExerciseBodyParts(string[] selectedBodyParts, Exercise exerciseToUpdate)
        {
            if (selectedBodyParts == null)
            {
                exerciseToUpdate.BodyParts = new List<BodyPart>();
                return;
            }

            var selectedBodyPartsHS = new HashSet<string>(selectedBodyParts);
            var exerciseBodyParts = new HashSet<int>
                (exerciseToUpdate.BodyParts.Select(c => c.BodyPartID));
            foreach (var bodyPart in db.BodyParts)
            {
                if (selectedBodyPartsHS.Contains(bodyPart.BodyPartID.ToString()))
                {
                    if (!exerciseBodyParts.Contains(bodyPart.BodyPartID))
                    {
                        exerciseToUpdate.BodyParts.Add(bodyPart);
                    }
                }
                else
                {
                    if (exerciseBodyParts.Contains(bodyPart.BodyPartID))
                    {
                        exerciseToUpdate.BodyParts.Remove(bodyPart);
                    }
                }
            }
        }





        // GET: Exercise/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exercise exercise = db.Exercises.Find(id);
            if (exercise == null)
            {
                return HttpNotFound();
            }
            return View(exercise);
        }

        // POST: Exercise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Exercise exercise = db.Exercises.Find(id);
            db.Exercises.Remove(exercise);
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
    }
}
