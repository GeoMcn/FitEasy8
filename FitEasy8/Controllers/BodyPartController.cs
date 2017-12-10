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

namespace FitEasy8.Controllers
{
    public class BodyPartController : Controller
    {
        private FitEasyContext db = new FitEasyContext();

        // GET: BodyPart
        public ActionResult Index()
        {
            return View(db.BodyParts.ToList());
        }

        // GET: BodyPart/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BodyPart bodyPart = db.BodyParts.Find(id);
            if (bodyPart == null)
            {
                return HttpNotFound();
            }
            return View(bodyPart);
        }

        // GET: BodyPart/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BodyPart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BodyPartID,Title,Description,Location,Image")] BodyPart bodyPart)
        {
            if (ModelState.IsValid)
            {
                db.BodyParts.Add(bodyPart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bodyPart);
        }

        // GET: BodyPart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BodyPart bodyPart = db.BodyParts.Find(id);
            if (bodyPart == null)
            {
                return HttpNotFound();
            }
            return View(bodyPart);
        }

        // POST: BodyPart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BodyPartID,Title,Description,Location,Image")] BodyPart bodyPart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bodyPart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bodyPart);
        }

        // GET: BodyPart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BodyPart bodyPart = db.BodyParts.Find(id);
            if (bodyPart == null)
            {
                return HttpNotFound();
            }
            return View(bodyPart);
        }

        // POST: BodyPart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BodyPart bodyPart = db.BodyParts.Find(id);
            db.BodyParts.Remove(bodyPart);
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
