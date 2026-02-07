
using EntityModel.Model;
using EntityModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace BusinessLayer.IRepository
{
    /// <summary>
    /// Generic repository interface for common CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get entity by ID
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Get all entities
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Find entities matching a predicate
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get single entity matching a predicate
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Check if any entity matches the predicate
        /// </summary>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Count entities matching a predicate
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Add a new entity
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Add multiple entities
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update an entity
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Update multiple entities
        /// </summary>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Soft delete an entity (sets IsDeleted to true)
        /// </summary>
        Task<bool> SoftDeleteAsync(int id);

        /// <summary>
        /// Hard delete an entity (permanently removes from database)
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Hard delete multiple entities
        /// </summary>
        void DeleteRange(IEnumerable<T> entities);
    }
}
