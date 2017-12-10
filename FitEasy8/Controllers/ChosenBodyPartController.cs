using FitEasy8.DAL;
using FitEasy8.Models;
using FitEasy8.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitEasy8.Controllers
{
    public class ChosenBodyPartController : Controller
    {
        private FitEasyContext db = new FitEasyContext();


        private UserManager<User> manager;

        public ChosenBodyPartController()
        {
            db = new FitEasyContext();
            manager = new UserManager<User>(new UserStore<User>(db));
        }
        // GET: ChosenBodyPart
        public async System.Threading.Tasks.Task<ActionResult> Index(int? id)
        {


            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            //var ChosenExercise = db.ChosenExercises.Where(p => p.ExercisePlanID == id && p.UserId == currentUser.Id).FirstOrDefault();

            var viewModel = new ChosenBodyPartIndexData();


            viewModel.ChosenBodyParts = db.ChosenBodyParts
                .Where(i => i.UserId == currentUser.Id)
                .OrderBy(i => i.Title);


            if (id != null)
            {
                ViewBag.ChosenBodyPartID = id.Value;

            }



            return View(viewModel);
        }

        // GET: ChosenBodyPart/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ChosenBodyPart/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChosenBodyPart/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ChosenBodyPart/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChosenBodyPart/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ChosenBodyPart/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChosenBodyPart/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
