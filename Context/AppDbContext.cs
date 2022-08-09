using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Context
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Equipments> equipments { get; set; }
    public DbSet<Instruments> instruments { get; set; }
    public DbSet<Components> components { get; set; }
    public DbSet<Reagents> reagents { get; set; }
    public DbSet<Consumables> consumables { get; set; }
    public DbSet<Users> users { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //  modelBuilder.Entity<Users>(entity => { entity.HasIndex(e => e.email).IsUnique(); });
    //}

  }
}
