using FitEasy8.DAL;
using FitEasy8.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitEasy8.ViewModels
{

    public class DetailViewModels
    {
    }

    public class ExercisePlanVM
    {
        public int ExercisePlanID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Difficulty Difficulty { get; set; }
        public IEnumerable<ExerciseVM> Exercises { get; set; }


    }

    public class MyExercisePlanVM
    {
        public FitEasyContext db = new FitEasyContext();
        public int PlanVMID { get; set; }
        public int ExercisePlanID { get; set; }
        public int MyExercisePlanID { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Count { get; set; }
        public bool? IsDone { get; set; }
        public Difficulty Difficulty { get; set; }
        public string Reps { get; set; }

        public IEnumerable<ExerciseVM> Exercises { get; set; }

    }



    public class ExerciseVM
    {
        public int ExerciseID { get; set; }
        public int BodyPartID { get; set; }
        public int CEBodyPartID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Byte[] Image { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public Rating? Rating { get; set; }
        public bool? IsDone { get; set; }
        public Models.Type? Type { get; set; }
        public virtual BodyPart BodyPart { get; set; }

        public IEnumerable<BodyPartVM> BodyParts { get; set; }
    }

    public class UserVM
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public double? BMI { get; set; }
        public TargetAim? TargetAim { get; set; }
        public Byte[] Image { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IEnumerable<ExercisePlanVM> ExercisePlans { get; set; }
    }

    public class BodyPartVM
    {
        public int BodyPartID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<ExerciseVM> Exercises { get; set; }
    }
}