using FitEasy8.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace FitEasy8.DAL
{
    public class FitEasyContext : IdentityDbContext<User>
    {

        public FitEasyContext() : base("FitEasyContext")
        {
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ChosenExercise> ChosenExercises { get; set; }
        public DbSet<ExercisePlan> ExercisePlans { get; set; }
        public DbSet<MyExercisePlan> MyExercisePlans { get; set; }
        public DbSet<BodyPart> BodyParts { get; set; }
        public DbSet<ChosenBodyPart> ChosenBodyParts { get; set; }
        public DbSet<BMI> BMI { get; set; }
        public DbSet<Achievement> Achievements { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();



            modelBuilder.Entity<ExercisePlan>()
                .HasMany(c => c.Exercises).WithMany(i => i.ExercisePlans)
                .Map(t => t.MapLeftKey("ExercisePlanID")
                    .MapRightKey("ExerciseID")
                    .ToTable("WorkOutPlanExercises"));

            modelBuilder.Entity<Exercise>()
                .HasMany(c => c.BodyParts)
                .WithMany(i => i.Exercises)
                .Map(t => t.MapLeftKey("ExerciseID")
                    .MapRightKey("BodyPartID")
                    .ToTable("ExerciseBodyParts"));

            modelBuilder.Entity<User>()
               .HasMany(c => c.ExercisePlans)
               .WithMany(i => i.Users)
               .Map(t => t.MapLeftKey("UserID")
                    .MapRightKey("ExercisePlanID")
                    .ToTable("UserExercisePlans"))
            ;
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<User>()
                .ToTable("Users");


            //modelBuilder.Entity<User>()
            //   .HasOptional(c => c.MyExercisePlan)
            //   .WithRequired(i => i.User);

            base.OnModelCreating(modelBuilder);
        }

        public static FitEasyContext Create()
        {
            return new FitEasyContext();
        }




        public DbQuery<T> Query<T>() where T : class
        {
            return Set<T>().AsNoTracking();
        }
    }
}