using FleetManagementApp.Data.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManagementApp.Data.Repositories
{
    /// <summary>
    /// Abstract base generic repository encapsulating SqlHelper access and entity mapping.
    /// </summary>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly SqlHelper Db;
        protected Repository(SqlHelper db)
        {
            Db = db ?? throw new ArgumentNullException(nameof(db));
        }
        /// <summary>
        /// Defines how to map an ADO.NET SqlDataReader row to entity type T.
        /// Derived repositories implement this without requiring reflection.
        /// </summary>
        protected abstract T MapRecord(SqlDataReader reader);
        public abstract Task<T> GetByIdAsync(int id);
        public abstract Task<IReadOnlyList<T>> GetAllAsync();
        public abstract Task<int> AddAsync(T entity);
        public abstract Task<bool> UpdateAsync(T entity);
        public abstract Task<bool> DeleteAsync(int id);
    }
}
