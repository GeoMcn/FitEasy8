using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitEasy8.ViewModels
{
    public class AssignedExerciseData
    {
        public int ExerciseID { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }
        public bool? IsDone { get; set; }


    }

    public class AssignedExercisePlanData
    {
        public int ExercisePlanID { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }


    }

    public class AssignedBodyPartData
    {
        public int BodyPartID { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }


    }
}