using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FitEasy8.Models
{
    public class Achievement
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AchievementId { get; set; }
        public string UserId { get; set; }
        public string ExercisePlanName { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public virtual User User { get; set; }
    }
}