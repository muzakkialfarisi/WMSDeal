
using WMS.DataAccess.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;

namespace WMS.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(AppDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public IList<T> GetAll()
        {
            return dbSet.ToList();
        }

        public IList<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
            bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
            bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
                                         Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                         Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                         bool disableTracking = true,
                                         bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).FirstOrDefault();
            }
            else
            {
                return query.FirstOrDefault();
            }
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).FirstOrDefaultAsync();
            }
            else
            {
                return await query.FirstOrDefaultAsync();
            }
        }

        public T GetSingleOrDefault(Expression<Func<T, bool>> filter = null,
                                         Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                         Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                         bool disableTracking = true,
                                         bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).SingleOrDefault();
            }
            else
            {
                return query.SingleOrDefault();
            }
        }

        public async Task<T> GetSingleOrDefaultAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).SingleOrDefaultAsync();
            }
            else
            {
                return await query.SingleOrDefaultAsync();
            }
        }

        public async Task<IPagedList<T>> GetPagedListAsync(Expression<Func<T, bool>> filter = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                int pageIndex = 0,
                                                int pageSize = 10,
                                                bool disableTracking = true,
                                                bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToPagedListAsync(pageIndex, pageSize);
            }
            else
            {
                return await query.ToPagedListAsync(pageIndex, pageSize);
            }
        }

        public async Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                                    Expression<Func<T, bool>> filter = null,
                                                                    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                                    Func<IQueryable<T>, IIncludableQueryable<T, object>> includeProperties = null,
                                                                    int pageIndex = 0,
                                                                    int pageSize = 10,
                                                                    bool disableTracking = true,
                                                                    CancellationToken cancellationToken = default(CancellationToken),
                                                                    bool ignoreQueryFilters = false)
            where TResult : class
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
            else
            {
                return await query.Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return await query.AnyAsync();
        }

        public async Task<TEntity> MaxAsync<TEntity>(Expression<Func<T, bool>>? filter = null, Expression<Func<T, TEntity>> selector = null)
        {
            if (filter == null)
            {
                return await dbSet.MaxAsync(selector);
            }
            else
            {
                return await dbSet.Where(filter).MaxAsync(selector);
            }
        }

        public async Task<TEntity> MinAsync<TEntity>(Expression<Func<T, bool>>? filter = null, Expression<Func<T, TEntity>> selector = null)
        {
            if (filter == null)
            {
                return await dbSet.MinAsync(selector);
            }
            else
            {
                return await dbSet.Where(filter).MinAsync(selector);
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return await query.CountAsync();
        }

        public async Task<decimal> SumAsync(Expression<Func<T, bool>>? filter = null, Expression<Func<T, decimal>> selector = null)
        {
            if (filter == null)
            {
                return await dbSet.SumAsync(selector);
            }
            else
            {
                return await dbSet.Where(filter).SumAsync(selector);
            }
        }

        void IRepository<T>.Add(T entity)
        {
            dbSet.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public virtual Task InsertAsync(params T[] entities) => dbSet.AddRangeAsync(entities);

        void IRepository<T>.Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        void IRepository<T>.RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

    }
}
