using ClassLibrary.Entities;
using InfraLibrary;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace Tests.InfraLibrary
{
    public class RepositoryShould
    {
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

        public RepositoryShould()
        {
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("theDb")
                .Options;
        }

        [Fact]
        public async Task BeAbleToRecordAProduct()
        {
            var product = new Product("sku", "description");

            using (var context = new ApplicationDbContext(_contextOptions))
            {
                var repository = new Repository<Product>(context);
                await repository.CreateAsync(product);
            }

            using (var context = new ApplicationDbContext(_contextOptions))
            {
                Assert.Equal(1, await context.Set<Product>().CountAsync());
                Assert.NotNull(await context.Set<Product>().FirstOrDefaultAsync(w => w.Id == product.Id));
            }
        }
    }
}
