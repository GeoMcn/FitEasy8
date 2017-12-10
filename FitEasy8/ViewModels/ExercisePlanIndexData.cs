using FitEasy8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitEasy8.ViewModels
{
    public class ExercisePlanIndexData
    {
        public IEnumerable<ExercisePlan> ExercisePlans { get; set; }
        public IEnumerable<Exercise> Exercises { get; set; }
        public IEnumerable<MyExercisePlan> MyExercisePlans { get; set; }

    }

    public class UserIndexData
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<ExercisePlan> ExercisePlans { get; set; }
        public IEnumerable<MyExercisePlan> MyExercisePlans { get; set; }
        public IEnumerable<Exercise> Exercises { get; set; }
    }

    public class ExerciseIndexData
    {
        public IEnumerable<User> BodyParts { get; set; }
        public IEnumerable<ExercisePlan> Exercises { get; set; }
    }
    public class BMIIndexData
    {
        public IEnumerable<BMI> BMIs { get; set; }
    }
    public class AchievementIndexData
    {
        public IEnumerable<Achievement> Achievements { get; set; }
    }
    public class ChosenExerciseIndexData
    {
        public IEnumerable<ChosenExercise> ChosenExercises { get; set; }
    }
    public class ChosenBodyPartIndexData
    {
        public IEnumerable<ChosenBodyPart> ChosenBodyParts { get; set; }
    }
}

