using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FitEasy8.Models
{
    public class BMI
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int? BmiId { get; set; }
        public string UserId { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public double? Result { get; set; }
        public DateTime AddedOn { get; set; }

        public string Verdict { get; set; }
    }
}