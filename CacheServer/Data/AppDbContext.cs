using Microsoft.EntityFrameworkCore;

namespace CacheServer.Data;

public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Data Source=DESKTOP-0RGINKQ\\SQLEXPRESS;Initial Catalog=CacheTask_DB;Integrated Security=True;TrustServerCertificate=True;");
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
    public DbSet<Product> Products { get; set; }
}