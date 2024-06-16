using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        public DbSet<KeyBinding> KeyBindings {  get; set; }
        
    }
}
