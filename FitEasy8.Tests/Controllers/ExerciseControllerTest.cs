using FitEasy8.Controllers;
using FitEasy8.DAL;
using FitEasy8.Models;
using FitEasy8.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FitEasy8.Tests.Controllers
{
    class ExerciseControllerTest
    {
        [TestMethod]
        public void Index(int? id)
        {
            // Arrange
           // //ExerciseController controller = new ExerciseController();
           // var exercises = new List<Exercise>
           // {
           //     new Exercise
           //     {
           //         ExerciseID = 098,
           //     BodyPartId = 22,
           //     Title = "Pull Up",
           //     ImageUrl = "http://marcelstotalfitness.com/wp-content/uploads/2014/02/unassisted-pullup.jpg",
           //     Description = " The Pull-up is performed by hanging from a chin-up bar above head height with the palms facing forward (supinated) and pulling the body up so the chin reaches or passes the bar. The pull-up is a compound exercise that also involves the biceps, forearms, traps, and the rear deltoids.  ",
           //     VideoUrl = "https://www.youtube.com/watch?v=eGo4IYlbE5g",
           //     Rating = Rating.A,
           //     Type = FitEasy8.Models.Type.Strength
           //     },
           //     new Exercise
           //     {
           //         ExerciseID = 7898,
           //     BodyPartId = 22,
           //     Title = "Chin Up",
           //     ImageUrl = "http://marcelstotalfitness.com/wp-content/uploads/2014/02/unassisted-pullup.jpg",
           //     Description = " ",
           //     VideoUrl = "",
           //     Rating = Rating.A,
           //     Type = FitEasy8.Models.Type.Strength
           //     },
           //     new Exercise
           //     {
           //         ExerciseID = 87898,
           //     BodyPartId = 22,
           //     Title = "Push Up",
           //     ImageUrl = "",
           //     Description = " ",
           //     VideoUrl = "",
           //     Rating = Rating.A,
           //     Type = FitEasy8.Models.Type.Strength
           //     }

           // };
           
           //// var db = new Mock<FitEasyContext>();
           //// db.Setup(e => e.Query()).Returns(exercises.AsQueryable());
           //// var controller = new ExerciseController(db.Object);
           //// //Act
           //// var result = controller.Index() as ViewResult;
           //// var model = result.Model as ExercisePlanIndexData;
           ////// Assert
           //// Assert.IsNotNull(result);
           //// Assert.AreEqual(3, model.Exercises.Count());

           // // Act
           //// ViewResult result = controller.Index() as ViewResult;

           // // Assert
           // Assert.IsNotNull(result);
        }
    }
}
