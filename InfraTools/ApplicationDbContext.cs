using Microsoft.EntityFrameworkCore;

namespace InfraTools
{
    public class ApplicationDbContext : InfraLibrary.ApplicationDbContext {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
    }
}
