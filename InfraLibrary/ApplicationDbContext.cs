using ClassLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfraLibrary
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
