using System.Data.Entity;
using Test.Data.Model;

namespace Test.Data.DbContext
{
    public class ApplicationDbContext : System.Data.Entity.DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnectionString")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }

        public DbSet<User> Users { get; set; }
    }
}
