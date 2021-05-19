using APICrudBasica.Models;
using Microsoft.EntityFrameworkCore;

namespace APICrudBasica.Data
{

    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> opt) : base(opt)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

    }

}