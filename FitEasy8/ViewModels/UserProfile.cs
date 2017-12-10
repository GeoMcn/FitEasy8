using FitEasy8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitEasy8.ViewModels
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public double? BMI { get; set; }
        public TargetAim? TargetAim { get; set; }
        public int? ExercisesCompleted { get; set; }
        public int? PlansCompleted { get; set; }
        public ICollection<ExercisePlan> ExercisePlans { get; set; }
        public Byte[] Image { get; set; }

    }
}