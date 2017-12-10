using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FitEasy8.Models
{
    public class ChosenBodyPart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChosenBodyPartID { get; set; }
        public string UserId { get; set; }
        public int OtherBodyPartID { get; set; }
        public int ChosenExerciseID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }


        public ICollection<ChosenExercise> Exercises { get; set; }

        public ChosenBodyPart()
        {
            this.Exercises = new List<ChosenExercise>();
        }
    }
}