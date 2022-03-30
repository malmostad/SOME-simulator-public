using System.Threading.Tasks;

namespace SomeSimulator.Interfaces
{
    public interface ICrudInterface<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T item);
        Task<T> UpdateAsync(T item);
        Task DeleteAsync(int id);
    }
}