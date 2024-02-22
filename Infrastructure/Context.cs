using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Context : DbContext
    {
        public Context()
        {

        }

        public Context(string connectionString) : base(GetOptions(connectionString)) { }

        public Context(DbContextOptions<Context> options) : base(options) { }

        public static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.UseSqlServer("conn string");

            // For using localDb
            //optionsBuilder.UseSqlServer("conn string");
            // also optional to add this to make the migrations assembly to "ASP NET template"
            //, b => b.MigrationsAssembly("ASP NET Template");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Security> Securities { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Status> Status { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasColumnName("id")
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Security>()
                .HasOne(s => s.User);

            // Inventory items
            modelBuilder.Entity<InventoryItem>()
                .Property(u => u.Id)
                .HasColumnName("id")
                .IsRequired();

            modelBuilder.Entity<InventoryItem>()
                .HasOne(i => i.Location)
                .WithMany(l => l.InventoryItems);

            modelBuilder.Entity<InventoryItem>()
                .HasOne(i => i.Status)
                .WithMany(s => s.InventoryItems);

            // Locations
            modelBuilder.Entity<Location>()
                .Property(u => u.Id)
                .HasColumnName("id")
                .IsRequired();

            modelBuilder.Entity<Location>()
                .HasIndex(l => l.Name)
                .IsUnique();

            modelBuilder.Entity<Location>()
                .HasData(
                new Location()
                {
                    Id = 1,
                    Name = "Warehouse",
                    Description = "Power cellar warehouse, where the lost property stock is dropped off",
                    Address = "30 Musgrave Rd, Coopers Plains QLD 4108"
                });

            // Status
            modelBuilder.Entity<Status>()
                .Property(u => u.Id)
                .HasColumnName("id")
                .IsRequired();

            modelBuilder.Entity<Status>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<Status>()
                .HasData(
                    new Status() { Id = 1, Name = "Awaiting Inspection", ColourCode = "#DC3545" },
                    new Status() { Id = 2, Name = "Good to go", ColourCode = "#5CB85C" },
                    new Status() { Id = 3, Name = "Pending transfer approval", ColourCode = "#FFD700" },
                    new Status() { Id = 4, Name = "Salvageable components", ColourCode = "#FFD700" },
                    new Status() { Id = 5, Name = "Closed", ColourCode = "#808080" }
            );
        }
    }
}