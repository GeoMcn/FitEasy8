using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace FitEasy8.Models
{
    public enum TargetAim { Other = 0, GainMuscle = 1, LoseWeight = 2 }
    public class User : IdentityUser
    {

        public int MyExercisePlanId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        [Display(Name = "Height (MTRS)")]
        public double? Height { get; set; }

        [Display(Name = "Weight (KG)")]

        public double? Weight { get; set; }

        public double? BMI { get; set; }
        public TargetAim? TargetAim { get; set; }
        //public bool? RememberMe { get; set; }

        public Byte[] Image { get; set; }

        public string ImagePath { get; set; }
        //public string ThumbPath { get;set; }

        public int? PlansCompleted { get; set; }

        public int? ExercisesCompleted { get; set; }

        public int? Count { get; set; }


        //public HttpPostedFileBase ImageURL { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        public ICollection<ExercisePlan> ExercisePlans { get; set; }

        public User()
        {
            this.ExercisePlans = new List<ExercisePlan>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

    }

    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }
        public AppRole(string name) : base(name) { }
    }
}