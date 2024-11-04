using Microsoft.EntityFrameworkCore;

namespace MyPlanner.Data
{
    //Arv av DbContext klass som används för databaskoppling
    public class TodoDataContext : DbContext
    {
        //get set för listan av todos i databasen
        public DbSet<Todo> Todos { get; set; }

        //Konfigurationsegenskaper
        protected readonly IConfiguration Configuration;

        //Konstruktor
        public TodoDataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //Konfigurerar typ av databas
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Entity framework core använder metod UseSqlite för att hämta TodoDB i appsettings.json
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("TodoDB"));
        }

        //Skapar upp modellen som kommer bygga upp tabellen Todos
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

