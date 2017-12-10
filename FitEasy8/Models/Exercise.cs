using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FitEasy8.Models
{
    public enum Rating { A, B, C, D, E, F }
    public enum Type { Aerobic, Strength, Flexibility, Reflexes }

    public class Exercise
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExerciseID { get; set; }
        public int BodyPartId { get; set; }

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

        public virtual BodyPart BodyPart { get; set; }
        public virtual ICollection<BodyPart> BodyParts { get; set; }
        public ICollection<ExercisePlan> ExercisePlans { get; set; }

        public ICollection<MyExercisePlan> MyExercisePlans { get; set; }

        public Exercise()
        {
            this.ExercisePlans = new List<ExercisePlan>();
            this.BodyParts = new List<BodyPart>();
            this.MyExercisePlans = new List<MyExercisePlan>();
        }

    }
}