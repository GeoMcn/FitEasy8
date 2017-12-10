using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FitEasy8.Models;

namespace FitEasy8.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            BodyPart bodyPart = new BodyPart

            {
                BodyPartID = 999, Title = "Quadriceps", ImageUrl = "https://thumbs.dreamstime.com/t/man-quadriceps-pain-over-white-background-85870447.jpg", Description = " the quadriceps, quadriceps extensor, or quads, is a large muscle group that includes the four prevailing muscles on the front of the thigh. "
            };

            Console.WriteLine(bodyPart.Title.ToString());
            Console.ReadLine();
        }
    }
}
