using FitEasy8.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FitEasy8.Tests.Controllers
{
    class ExercisePlanTest
    {

        [TestMethod]
        public void Index()
        {
            // Arrange
            ExercisePlanController controller = new ExercisePlanController();

            // Act
            System.Threading.Tasks.Task<ActionResult> result = controller.Index() as System.Threading.Tasks.Task<ActionResult>;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
