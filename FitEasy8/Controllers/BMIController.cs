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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;
using System.Web.Helpers;
using System.Threading.Tasks;
using FitEasy8.ViewModels;
using System.IO;

namespace FitEasy8.Controllers
{
    public class BMIController : Controller
    {
        private FitEasyContext db = new FitEasyContext();


        private UserManager<User> manager;

        public BMIController()
        {
            db = new FitEasyContext();
            manager = new UserManager<User>(new UserStore<User>(db));
        }

        // GET: BMI
        public async Task<ActionResult> Index()
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var viewModel = new BMIIndexData();
            viewModel.BMIs = db.BMI
               .Where(i => i.UserId == currentUser.Id)
               .OrderBy(i => i.AddedOn);


            double? total = 0, avg = 0;
            foreach (var bmi in viewModel.BMIs)
            {
                if (bmi.UserId == currentUser.Id)
                {
                    total++;
                    avg += bmi.Weight.Value;
                    double? totalAvg = (avg.Value / total.Value);
                    ViewBag.Average = totalAvg;
                }
            }

            string message;
            var lastBMI = this.db.BMI
                            .Where(c => c.UserId == currentUser.Id)
                            .OrderByDescending(t => t.AddedOn)
                            .FirstOrDefault();
            var previousBMI = this.db.BMI
                            .Where(c => c.UserId == currentUser.Id)
                            .OrderByDescending(t => t.AddedOn)
                            .Skip(1)
                            .FirstOrDefault();

            double? amountLost = previousBMI.Weight - lastBMI.Weight;
            double? amountGained = lastBMI.Weight - previousBMI.Weight;

            if (currentUser.TargetAim == TargetAim.GainMuscle)
            {
                if (lastBMI.Weight < previousBMI.Weight)
                {
                    message = "Going by your last BMI, you have lost " + amountLost.Value + "kg!! You have set your Target Aim to Gain Muscle. You should be doing more Strength based exercises or make sure you are eating enough calories and protein in your diet! ";
                    ViewBag.wMessage = message.ToString();
                }
                else
                {
                    message = "Well done, you have gained " + amountGained.Value + "kg!! Keep up the good work and you will hit your target in no time!!";
                    ViewBag.wMessage = message.ToString();
                }
            }
            else if (currentUser.TargetAim == TargetAim.LoseWeight)
            {
                if (lastBMI.Weight > previousBMI.Weight)
                {
                    message = "Going by your last BMI, you have Gained " + amountGained.Value + "kg!! You have set your Target Aim to lose weight. You should be doing more Aerobic based exercises and make sure you are eating smaller amounts calories and bad carbs and sugars in your diet! ";
                    ViewBag.wMessage = message.ToString();
                }
                else
                {
                    message = "Well done, you have lost " + amountLost.Value + "kg!! Keep up the good work and you will hit your target in no time!!";
                    ViewBag.wMessage = message.ToString();
                }
            }

            return View(viewModel.BMIs);
        }





        // return View(db.BMI.ToList());

        public async Task<ActionResult> Index2(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var viewModel = new BMIIndexData();

            viewModel.BMIs = db.BMI
                .Where(i => i.UserId == currentUser.Id)
                .OrderBy(i => i.AddedOn);

            if (id != null)
            {
                ViewBag.MyExercisePlanID = id.Value;
            }

            return View(viewModel);
        }

        // GET: BMI/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BMI bMI = db.BMI.Find(id);
            if (bMI == null)
            {
                return HttpNotFound();
            }
            return View(bMI);
        }

        // GET: BMI/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BMI/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create([Bind(Include = "")] BMI bMI)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            bMI.AddedOn = DateTime.Now;
            bMI.UserId = currentUser.Id;
            if (bMI.Height == null)
            {
                bMI.Height = currentUser.Height;
            }
            if (bMI.Weight == null)
            {
                bMI.Weight = currentUser.Weight;

            }
            var weight = db.Users
                    .Single(u => u.Id == bMI.UserId);
            if (weight.Weight == null)
            {
                bMI.Weight = 1;
            }

            bMI.Result = (bMI.Weight.Value) / (bMI.Height.Value * bMI.Height.Value);

            if (currentUser.TargetAim == TargetAim.GainMuscle)
            {

                if (bMI.Result <= 18.5)
                {
                    bMI.Verdict = "you are under average and should attempt to gain more weight, eat loads of carbs and calories. Don't forget Protein!";
                }
                else if (bMI.Result <= 24.9)
                {
                    bMI.Verdict = "you are average weight and seem healthy, include more protein and calories in your diet to continue to gain weight";
                }
                else if (bMI.Result <= 29.9)
                {
                    bMI.Verdict = "You are going above the average weight now. I hope that's the extra muscle you have been piling on and not loosey-goosey flab handles!";
                }
                else if (bMI.Result >= 30)
                {
                    bMI.Verdict = "You're well above average now! If i come across you on the street I hope you are looking like a wall with arms as strong as rock instead of a couch potato with a plump belly!";
                }
            }

            else
            {
                if (bMI.Result <= 18.5)
                {
                    bMI.Verdict = "you are under average and should attempt to gain more wieght";
                }
                else if (bMI.Result <= 24.9)
                {
                    bMI.Verdict = "you are average weight and seem healthy";
                }
                else if (bMI.Result <= 29.9)
                {
                    bMI.Verdict = "You are going above average now, either you have gained extra muscle which in that case good for you, or, you have eaten a few too many pies or sneaky McDonalds. Get back to worl!";
                }
                else if (bMI.Result >= 30)
                {
                    bMI.Verdict = "Seriously time to get back to work and pump some steam. You need to lose weight unless it is pure steel you are rocking!";
                }

            }

            if (ModelState.IsValid)
            {
                db.BMI.Add(bMI);
                db.SaveChanges();
                currentUser.Height = bMI.Height;
                currentUser.Weight = bMI.Weight;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(bMI);
        }


        // GET: BMI/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BMI bMI = db.BMI.Find(id);
            if (bMI == null)
            {
                return HttpNotFound();
            }
            return View(bMI);
        }

        // POST: BMI/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BmiId,UserId,Weight,Height,Result,Verdict")] BMI bMI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bMI).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bMI);
        }

        // GET: BMI/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BMI bMI = db.BMI.Find(id);
            if (bMI == null)
            {
                return HttpNotFound();
            }
            return View(bMI);
        }

        // GET: BMI/Stats/5
        public async Task<ActionResult> Stats()
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            string message;
            var lastBMI = this.db.BMI
                            .Where(c => c.UserId == currentUser.Id)
                            .OrderByDescending(t => t.AddedOn)
                            .FirstOrDefault();
            var previousBMI = this.db.BMI
                            .Where(c => c.UserId == currentUser.Id)
                            .OrderByDescending(t => t.AddedOn)
                            .Skip(1)
                            .FirstOrDefault();

            double? amountLost = previousBMI.Weight - lastBMI.Weight;
            double? amountGained = lastBMI.Weight - previousBMI.Weight;

            if (currentUser.TargetAim == TargetAim.GainMuscle)
            {
                if (lastBMI.Weight < previousBMI.Weight)
                {
                    message = "Going by your last BMI, you have lost " + amountLost.Value + "kg!! You have set your Target Aim to Gain Muscle. You should be doing more Strength based exercises or make sure you are eating enough calories and protein in your diet! ";
                    ViewBag.wMessage = message.ToString();
                }
                else
                {
                    message = "Well done, you have gained " + amountGained.Value + "kg!! Keep up the good work and you will hit your target in no time!!";
                    ViewBag.wMessage = message.ToString();
                }
            }
            else if (currentUser.TargetAim == TargetAim.LoseWeight)
            {
                if (lastBMI.Weight > previousBMI.Weight)
                {
                    message = "Going by your last BMI, you have Gained " + amountGained.Value + "kg!! You have set your Target Aim to lose weight. You should be doing more Aerobic based exercises and make sure you are eating smaller amounts calories and bad carbs and sugars in your diet! ";
                    ViewBag.wMessage = message.ToString();
                }
                else
                {
                    message = "Well done, you have lost " + amountLost.Value + "kg!! Keep up the good work and you will hit your target in no time!!";
                    ViewBag.wMessage = message.ToString();
                }
            }
            return View();
        }

        // POST: BMI/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            BMI bMI = db.BMI.Find(id);
            db.BMI.Remove(bMI);
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

        public async System.Threading.Tasks.Task<ActionResult> CharterColumn()

        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            ArrayList xValue = new ArrayList();

            ArrayList yValue = new ArrayList();

            var results = (from c in db.BMI where c.UserId == currentUser.Id select c);

            results.ToList().ForEach(rs => xValue.Add(rs.AddedOn.Date));

            results.ToList().ForEach(rs => yValue.Add(rs.Weight));

            new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

            .AddTitle("Weight(kg) History")

           .AddSeries("Default", chartType: "column", xValue: xValue, yValues: yValue)

                  .Write("bmp");

            return null;

        }

        public async System.Threading.Tasks.Task<ActionResult> ChartPie()

        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            ArrayList xValue = new ArrayList();

            ArrayList yValue = new ArrayList();

            var results = (from c in db.BMI where c.UserId == currentUser.Id select c);

            results.ToList().ForEach(rs => xValue.Add(rs.AddedOn.Date.Date.ToString()));

            results.ToList().ForEach(rs => yValue.Add(rs.Weight));

            new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla)

            .AddTitle("Weight(kg) History")

           .AddSeries("Default", chartType: "Pie", xValue: xValue, yValues: yValue)

                  .Write("bmp");

            return null;

        }




    }
}
