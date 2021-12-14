using ClassLibrary.Shared.Interfaces;
using System.Threading.Tasks;

namespace InfraLibrary
{
    public class Repository<T> : IRepository<T> where T: class, IEntity
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> CreateAsync(T item)
        {
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(T item)
        {
            _context.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<T> UpsertAsync(T item)
        {
            var result = item;
            if (await ExistsAsync(item)) await UpdateAsync(item);
            else result = await CreateAsync(item);
            return result;
        }

        private async Task<bool> ExistsAsync(T item)
        {
            return (await _context.Set<T>().FindAsync(item.Id)) != null;
        }
    }
}
