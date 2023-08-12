using AdminPanel.Model;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Data;
public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}
