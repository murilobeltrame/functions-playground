using System.Threading.Tasks;

namespace ClassLibrary.Shared.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        public Task<T> CreateAsync(T item);
        public Task UpdateAsync(T item);
        public Task<T> UpsertAsync(T item);
    }
}
