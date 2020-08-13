using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointOfSale.WebAPI.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task SaveAsync();
        bool HasChanges();
        void Add(T model);
        void Update(T model);
        void Remove(T model);
    }
}
