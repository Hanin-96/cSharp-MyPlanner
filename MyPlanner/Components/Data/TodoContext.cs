using Microsoft.EntityFrameworkCore;

namespace MyPlanner.Data
{
    public class TodoDataContext : DbContext
    {

        protected readonly IConfiguration Configuration;

        //Constructor
        public TodoDataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("TodoDB"));
        }
        public DbSet<Todo> Todos { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>()
                .ToTable("Todos");

            modelBuilder.Entity<Todo>()
                .HasData(
                //Testdata till databasen
                    new Todo
                    {
                        Id = 1,
                        Title = "Programmering i C#",
                        Description = "Fortsätta skapa todolista med Blazor",
                        Category = "Studier",
                        Priority = 1,
                        Status = false,
                        DueDate = new DateTime(2024, 10, 18)
                    },

                    new Todo
                    {
                        Id = 2,
                        Title = "Grupparbete",
                        Description = "Skapa prototyp",
                        Category = "Studier",
                        Priority = 2,
                        Status = false,
                        DueDate = new DateTime(2024, 10, 20)
                    },
                    new Todo
                    {
                        Id = 3,
                        Title = "Test",
                        Description = "Test",
                        Category = "Personligt",
                        Priority = 2,
                        Status = false,
                        DueDate = new DateTime(2024, 10, 20)
                    }
                );
        }
    }
}

