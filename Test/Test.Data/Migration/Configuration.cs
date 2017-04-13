namespace Lagun.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Test.Data.DbContext;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);


            context.Users.Add(new Test.Data.Model.User
            {
                FirstName = "Naveed",
                MiddleName = "",
                LastName = "Jamil",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "admin",
                LastModifiedOn = DateTime.UtcNow,
                IsDeleted = false
            });

            context.Users.Add(new Test.Data.Model.User
            {
                FirstName = "Riaz",
                MiddleName = "",
                LastName = "Uddin",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "admin",
                LastModifiedOn = DateTime.UtcNow,
                IsDeleted = false
            });

            context.Users.Add(new Test.Data.Model.User
            {
                FirstName = "Faisal",
                MiddleName = "",
                LastName = "Khanani",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "admin",
                LastModifiedOn = DateTime.UtcNow,
                IsDeleted = false
            });

        }
    }
}
