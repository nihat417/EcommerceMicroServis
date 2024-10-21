using Ecommerce.SharedLibrary.Response;
using System.Linq.Expressions;

namespace Ecommerce.SharedLibrary.Interface
{
    public interface IGenericInterface<T> where T : class
    {
        Task<ApiResponse> CreateAsync(T entity);
        Task<ApiResponse> UpdateAsync(T entity);
        Task<ApiResponse> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> FindByIdAsync(Expression<Func<T, bool>> predicate);
    }
}
