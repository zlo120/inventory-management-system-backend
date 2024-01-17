using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Context : DbContext
    {
        public Context(string connectionString) : base(GetOptions(connectionString)) { }

        public static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.UseSqlServer("server=127.0.0.1;uid=root;pwd=Wasd123!;database=ims-db");

            // For using localDb
            //optionsBuilder.UseSqlServer("conn string");
            // also optional to add this to make the migrations assembly to "ASP NET template"
            //, b => b.MigrationsAssembly("ASP NET Template");
        }
    }
}