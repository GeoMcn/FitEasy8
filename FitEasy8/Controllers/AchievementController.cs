using FitEasy8.DAL;
using FitEasy8.Models;
using FitEasy8.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FitEasy8.Controllers
{
    public class AchievementController : Controller
    {
        private FitEasyContext db = new FitEasyContext();


        private UserManager<User> manager;

        public AchievementController()
        {
            db = new FitEasyContext();
            manager = new UserManager<User>(new UserStore<User>(db));
        }
        // GET: Achievement
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var viewModel = new AchievementIndexData();

            viewModel.Achievements = db.Achievements
                .Where(i => i.UserId == currentUser.Id)
                .OrderBy(i => i.Date);


            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Achievement achievement = db.Achievements.Find(id);
            if (achievement == null)
            {
                return HttpNotFound();
            }
            return View(achievement);
        }

        // POST: Exercise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Achievement achievement = db.Achievements.Find(id);
            db.Achievements.Remove(achievement);
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
