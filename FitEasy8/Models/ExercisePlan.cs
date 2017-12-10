using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitEasy8.Models
{


    public class ExercisePlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExercisePlanID { get; set; }

        [Required]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        [Required]
        public Difficulty Difficulty { get; set; }

        public ICollection<Exercise> Exercises { get; set; }
        public ICollection<User> Users { get; set; }

        public ExercisePlan()
        {
            this.Exercises = new List<Exercise>();
            this.Users = new List<User>();
        }

    }
}