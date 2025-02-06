using Microsoft.EntityFrameworkCore;
using DecisionBackend.Models.Domain;

namespace DecisionBackend.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { 
        
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; } 

        public DbSet<Models.Domain.Task> Tasks { get; set; }

       



    }
}
