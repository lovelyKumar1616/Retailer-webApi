using EntityModel.Model;
using EntityModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IRepository
{
    /// <summary>
    /// Unit of Work interface to manage transactions and repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get repository for a specific entity type
        /// </summary>
        IRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Save all changes made in this unit of work
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Begin a new transaction
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commit the current transaction
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Save changes synchronously (use sparingly)
        /// </summary>
        int SaveChanges();
    }
}
