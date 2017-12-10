using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FitEasy8.Models
{


    public class ChosenExercise
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChosenExerciseID { get; set; }
        public int ExercisePlanID { get; set; }
        public int? MyExercisePlanID { get; set; }
        public string UserId { get; set; }
        public int CEBodyPartID { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public Byte[] Image { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        [DisplayFormat(NullDisplayText = "No rating")]
        public Rating? Rating { get; set; }
        public Type? Type { get; set; }
        public bool? IsDone { get; set; }
        public int? Complete { get; set; }

        public int? Count { get; set; }
        public virtual BodyPart BodyPart { get; set; }
        public virtual ExercisePlan ExercisePlan { get; set; }

        public ChosenExercise()
        {
        }

    }
}