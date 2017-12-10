using FitEasy8.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FitEasy8.Models
{
    public enum Difficulty : int { easy = 0, medium = 1, hard = 2, extreme = 3 }
    public class MyExercisePlan
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MyExercisePlanID { get; set; }
        public string UserId { get; set; }
        public int ExercisePlanID { get; set; }

        [Required]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDone { get; set; }
        public int? Count { get; set; }
        public int? IsComplete { get; set; }
        public string Reps { get; set; }
        //public string ISDone
        //{
        //    get
        //    {
        //        return (bool)this.IsDone ? "Yes" : "NO";
        //    }
        //}
        [Required]
        public Difficulty Difficulty { get; set; }


        public virtual User User { get; set; }
        public ICollection<Exercise> Exercises { get; set; }
        public ICollection<User> Users { get; set; }

        public MyExercisePlan()
        {
            this.Exercises = new List<Exercise>();
            this.Users = new List<User>();
        }



    }
}

