using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Collections.Generic;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetAll();
        IList<T> GetAll(Expression<Func<T, bool>> filter = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                bool disableTracking = true,
                                                bool ignoreQueryFilters = false);

        Task<IList<T>> GetAllAsync();
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                bool disableTracking = true,
                                                bool ignoreQueryFilters = false);

        T GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                bool disableTracking = true,
                                                bool ignoreQueryFilters = false);

        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                bool disableTracking = true,
                                                bool ignoreQueryFilters = false);


        T GetSingleOrDefault(Expression<Func<T, bool>> filter = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                bool disableTracking = true,
                                                bool ignoreQueryFilters = false);

        Task<T> GetSingleOrDefaultAsync(Expression<Func<T, bool>> filter = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                bool disableTracking = true,
                                                bool ignoreQueryFilters = false);

        Task<IPagedList<T>> GetPagedListAsync(Expression<Func<T, bool>> filter = null,
                                                 Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                 Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                 int pageIndex = 1,
                                                 int pageSize = 10,
                                                 bool disableTracking = true,
                                                 bool ignoreQueryFilters = false);

        Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                             Expression<Func<T, bool>> filter = null,
                                                             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                             Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                             int pageIndex = 0,
                                                             int pageSize = 20,
                                                             bool disableTracking = true,
                                                             CancellationToken cancellationToken = default(CancellationToken),
                                                             bool ignoreQueryFilters = false) where TResult : class;

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        Task<int> CountAsync(Expression<Func<T, bool>> filter);
        Task<TEntity> MaxAsync<TEntity>(Expression<Func<T, bool>>? filter = null, Expression<Func<T, TEntity>> selector = null);
        Task<TEntity> MinAsync<TEntity>(Expression<Func<T, bool>>? filter = null, Expression<Func<T, TEntity>> selector = null);
        Task<decimal> SumAsync(Expression<Func<T, bool>>? filter = null, Expression<Func<T, decimal>> selector = null);

        void Add(T entity);
        Task AddAsync(T entity);

        Task InsertAsync(params T[] entities);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
