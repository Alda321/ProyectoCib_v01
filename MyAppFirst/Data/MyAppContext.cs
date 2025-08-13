using Microsoft.EntityFrameworkCore;
using MyAppFirst.Models;

namespace MyAppFirst.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
    }
}
