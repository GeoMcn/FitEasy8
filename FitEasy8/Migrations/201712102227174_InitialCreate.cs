namespace FitEasy8.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Achievement",
                c => new
                    {
                        AchievementId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ExercisePlanName = c.String(),
                        Description = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AchievementId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.ExercisePlan",
                c => new
                    {
                        ExercisePlanID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(maxLength: 200),
                        AddedOn = c.DateTime(),
                        UpdatedOn = c.DateTime(),
                        Difficulty = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ExercisePlanID);
            
            CreateTable(
                "dbo.Exercise",
                c => new
                    {
                        ExerciseID = c.Int(nullable: false, identity: true),
                        BodyPartId = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        Image = c.Binary(),
                        ImageUrl = c.String(),
                        VideoUrl = c.String(),
                        Rating = c.Int(),
                        Type = c.Int(),
                        IsDone = c.Boolean(),
                        BodyPart_BodyPartID = c.Int(),
                    })
                .PrimaryKey(t => t.ExerciseID)
                .ForeignKey("dbo.BodyPart", t => t.BodyPart_BodyPartID)
                .Index(t => t.BodyPart_BodyPartID);
            
            CreateTable(
                "dbo.BodyPart",
                c => new
                    {
                        BodyPartID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.BodyPartID);
            
            CreateTable(
                "dbo.MyExercisePlan",
                c => new
                    {
                        MyExercisePlanID = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ExercisePlanID = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Description = c.String(maxLength: 200),
                        AddedOn = c.DateTime(),
                        UpdatedOn = c.DateTime(),
                        IsDone = c.Boolean(nullable: false),
                        Count = c.Int(),
                        IsComplete = c.Int(),
                        Reps = c.String(),
                        Difficulty = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MyExercisePlanID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.RoleId)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.BMI",
                c => new
                    {
                        BmiId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Weight = c.Double(),
                        Height = c.Double(),
                        Result = c.Double(),
                        AddedOn = c.DateTime(nullable: false),
                        Verdict = c.String(),
                    })
                .PrimaryKey(t => t.BmiId);
            
            CreateTable(
                "dbo.ChosenBodyPart",
                c => new
                    {
                        ChosenBodyPartID = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        OtherBodyPartID = c.Int(nullable: false),
                        ChosenExerciseID = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.ChosenBodyPartID);
            
            CreateTable(
                "dbo.ChosenExercise",
                c => new
                    {
                        ChosenExerciseID = c.Int(nullable: false, identity: true),
                        ExercisePlanID = c.Int(nullable: false),
                        MyExercisePlanID = c.Int(),
                        UserId = c.String(),
                        CEBodyPartID = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        Image = c.Binary(),
                        ImageUrl = c.String(),
                        VideoUrl = c.String(),
                        Rating = c.Int(),
                        Type = c.Int(),
                        IsDone = c.Boolean(),
                        Complete = c.Int(),
                        Count = c.Int(),
                        BodyPart_BodyPartID = c.Int(),
                        ChosenBodyPart_ChosenBodyPartID = c.Int(),
                    })
                .PrimaryKey(t => t.ChosenExerciseID)
                .ForeignKey("dbo.BodyPart", t => t.BodyPart_BodyPartID)
                .ForeignKey("dbo.ExercisePlan", t => t.ExercisePlanID, cascadeDelete: true)
                .ForeignKey("dbo.ChosenBodyPart", t => t.ChosenBodyPart_ChosenBodyPartID)
                .Index(t => t.ExercisePlanID)
                .Index(t => t.BodyPart_BodyPartID)
                .Index(t => t.ChosenBodyPart_ChosenBodyPartID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ExerciseBodyParts",
                c => new
                    {
                        ExerciseID = c.Int(nullable: false),
                        BodyPartID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ExerciseID, t.BodyPartID })
                .ForeignKey("dbo.Exercise", t => t.ExerciseID, cascadeDelete: true)
                .ForeignKey("dbo.BodyPart", t => t.BodyPartID, cascadeDelete: true)
                .Index(t => t.ExerciseID)
                .Index(t => t.BodyPartID);
            
            CreateTable(
                "dbo.MyExercisePlanExercise",
                c => new
                    {
                        MyExercisePlan_MyExercisePlanID = c.Int(nullable: false),
                        Exercise_ExerciseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MyExercisePlan_MyExercisePlanID, t.Exercise_ExerciseID })
                .ForeignKey("dbo.MyExercisePlan", t => t.MyExercisePlan_MyExercisePlanID, cascadeDelete: true)
                .ForeignKey("dbo.Exercise", t => t.Exercise_ExerciseID, cascadeDelete: true)
                .Index(t => t.MyExercisePlan_MyExercisePlanID)
                .Index(t => t.Exercise_ExerciseID);
            
            CreateTable(
                "dbo.WorkOutPlanExercises",
                c => new
                    {
                        ExercisePlanID = c.Int(nullable: false),
                        ExerciseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ExercisePlanID, t.ExerciseID })
                .ForeignKey("dbo.ExercisePlan", t => t.ExercisePlanID, cascadeDelete: true)
                .ForeignKey("dbo.Exercise", t => t.ExerciseID, cascadeDelete: true)
                .Index(t => t.ExercisePlanID)
                .Index(t => t.ExerciseID);
            
            CreateTable(
                "dbo.UserExercisePlans",
                c => new
                    {
                        UserID = c.String(nullable: false, maxLength: 128),
                        ExercisePlanID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.ExercisePlanID })
                .ForeignKey("dbo.AspNetUsers", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.ExercisePlan", t => t.ExercisePlanID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.ExercisePlanID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MyExercisePlan_MyExercisePlanID = c.Int(),
                        MyExercisePlanId = c.Int(nullable: false),
                        LastName = c.String(),
                        FirstName = c.String(),
                        Height = c.Double(),
                        Weight = c.Double(),
                        BMI = c.Double(),
                        TargetAim = c.Int(),
                        Image = c.Binary(),
                        ImagePath = c.String(),
                        PlansCompleted = c.Int(),
                        ExercisesCompleted = c.Int(),
                        Count = c.Int(),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .ForeignKey("dbo.MyExercisePlan", t => t.MyExercisePlan_MyExercisePlanID)
                .Index(t => t.Id)
                .Index(t => t.MyExercisePlan_MyExercisePlanID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "MyExercisePlan_MyExercisePlanID", "dbo.MyExercisePlan");
            DropForeignKey("dbo.AspNetUsers", "Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserRoles", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserLogins", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserClaims", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ChosenExercise", "ChosenBodyPart_ChosenBodyPartID", "dbo.ChosenBodyPart");
            DropForeignKey("dbo.ChosenExercise", "ExercisePlanID", "dbo.ExercisePlan");
            DropForeignKey("dbo.ChosenExercise", "BodyPart_BodyPartID", "dbo.BodyPart");
            DropForeignKey("dbo.Achievement", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserExercisePlans", "ExercisePlanID", "dbo.ExercisePlan");
            DropForeignKey("dbo.UserExercisePlans", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkOutPlanExercises", "ExerciseID", "dbo.Exercise");
            DropForeignKey("dbo.WorkOutPlanExercises", "ExercisePlanID", "dbo.ExercisePlan");
            DropForeignKey("dbo.MyExercisePlan", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MyExercisePlanExercise", "Exercise_ExerciseID", "dbo.Exercise");
            DropForeignKey("dbo.MyExercisePlanExercise", "MyExercisePlan_MyExercisePlanID", "dbo.MyExercisePlan");
            DropForeignKey("dbo.ExerciseBodyParts", "BodyPartID", "dbo.BodyPart");
            DropForeignKey("dbo.ExerciseBodyParts", "ExerciseID", "dbo.Exercise");
            DropForeignKey("dbo.Exercise", "BodyPart_BodyPartID", "dbo.BodyPart");
            DropIndex("dbo.AspNetUsers", new[] { "MyExercisePlan_MyExercisePlanID" });
            DropIndex("dbo.AspNetUsers", new[] { "Id" });
            DropIndex("dbo.UserExercisePlans", new[] { "ExercisePlanID" });
            DropIndex("dbo.UserExercisePlans", new[] { "UserID" });
            DropIndex("dbo.WorkOutPlanExercises", new[] { "ExerciseID" });
            DropIndex("dbo.WorkOutPlanExercises", new[] { "ExercisePlanID" });
            DropIndex("dbo.MyExercisePlanExercise", new[] { "Exercise_ExerciseID" });
            DropIndex("dbo.MyExercisePlanExercise", new[] { "MyExercisePlan_MyExercisePlanID" });
            DropIndex("dbo.ExerciseBodyParts", new[] { "BodyPartID" });
            DropIndex("dbo.ExerciseBodyParts", new[] { "ExerciseID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ChosenExercise", new[] { "ChosenBodyPart_ChosenBodyPartID" });
            DropIndex("dbo.ChosenExercise", new[] { "BodyPart_BodyPartID" });
            DropIndex("dbo.ChosenExercise", new[] { "ExercisePlanID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "IdentityUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "IdentityUser_Id" });
            DropIndex("dbo.MyExercisePlan", new[] { "UserId" });
            DropIndex("dbo.Exercise", new[] { "BodyPart_BodyPartID" });
            DropIndex("dbo.AspNetUserClaims", new[] { "IdentityUser_Id" });
            DropIndex("dbo.Users", "UserNameIndex");
            DropIndex("dbo.Achievement", new[] { "UserId" });
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserExercisePlans");
            DropTable("dbo.WorkOutPlanExercises");
            DropTable("dbo.MyExercisePlanExercise");
            DropTable("dbo.ExerciseBodyParts");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ChosenExercise");
            DropTable("dbo.ChosenBodyPart");
            DropTable("dbo.BMI");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.MyExercisePlan");
            DropTable("dbo.BodyPart");
            DropTable("dbo.Exercise");
            DropTable("dbo.ExercisePlan");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.Achievement");
        }
    }
}
