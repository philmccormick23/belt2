using Microsoft.EntityFrameworkCore;
 
namespace BeltExam.Models
{
    public class YourContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public YourContext(DbContextOptions<YourContext> options) : base(options) { }
        public DbSet<User> User { get; set; }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }
        
    }
}