using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FitEasy8.Models
{
    public class BodyPart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BodyPartID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }


        public ICollection<Exercise> Exercises { get; set; }

        public BodyPart()
        {
            this.Exercises = new List<Exercise>();
        }
    }
}