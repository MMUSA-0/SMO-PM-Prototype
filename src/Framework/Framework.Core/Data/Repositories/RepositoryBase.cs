using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Framework.Core.Data.Repositories
{
    public class RepositoryBase<TContext, TEntity> : IRepositoryBase<TContext, TEntity>
    where TContext : IBaseDbContext
    where TEntity : class
    {
        protected TContext DbContext { get; }

        protected DbSet<TEntity> DbSet { get; }


        public RepositoryBase(TContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<TEntity>();
        }


        public virtual IQueryable<TEntity> Table => DbSet;

        public virtual IQueryable<TEntity> TableNoTracking => DbSet.AsNoTracking();

        public TEntity Insert(TEntity entity, bool autoSave = false)
        {
            var savedEntity = DbSet.Add(entity).Entity;

            if (autoSave)
            {
                DbContext.SaveChanges();
            }

            return savedEntity;
        }

        public void InsertRange(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            DbSet.AddRange(entities);

            if (autoSave)
            {
                DbContext.SaveChanges();
            }
        }

        public async Task InsertRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            await DbSet.AddRangeAsync(entities);

            if (autoSave)
            {
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false)
        {
            var savedEntity = DbSet.Add(entity).Entity;

            if (autoSave)
            {
                await DbContext.SaveChangesAsync();
            }

            return savedEntity;
        }

        public TEntity Update(TEntity entity, bool autoSave = false)
        {
            DbContext.Attach(entity);

            var updatedEntity = DbContext.Update(entity).Entity;

            if (autoSave)
            {
                DbContext.SaveChanges();
            }

            return updatedEntity;
        }

        public void UpdateRange(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            DbSet.UpdateRange(entities);
            if (autoSave)
            {
                DbContext.SaveChanges();
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            DbSet.UpdateRange(entities);
            if (autoSave)
            {
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false)
        {
            DbContext.Attach(entity);

            var updatedEntity = DbContext.Update(entity).Entity;

            if (autoSave)
            {
                await DbContext.SaveChangesAsync();
            }

            return updatedEntity;
        }


        public void Delete(TEntity entity, bool autoSave = false)
        {
            DbSet.Remove(entity);

            if (autoSave)
            {
                DbContext.SaveChanges();
            }
        }

        public async Task DeleteAsync(TEntity entity, bool autoSave = false)
        {
            DbSet.Remove(entity);

            if (autoSave)
            {
                await DbContext.SaveChangesAsync();
            }
        }

        public void DeleteRange(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            DbSet.RemoveRange(entities);

            if (autoSave)
            {
                DbContext.SaveChanges();
            }
        }

        public long GetCount()
        {
            return DbSet.LongCount();
        }

        public async Task<long> GetCountAsync()
        {
            return await DbSet.LongCountAsync();
        }

        protected IQueryable<TEntity> GetQueryable()
        {
            return DbSet.AsQueryable();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate, bool autoSave = false)
        {
            foreach (var entity in GetQueryable().Where(predicate).ToList())
            {
                Delete(entity, autoSave);
            }

            if (autoSave)
            {
                DbContext.SaveChanges();
            }
        }

        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false)
        {
            var entities = await GetQueryable()
                .Where(predicate)
                .ToListAsync();

            foreach (var entity in entities)
            {
                DbSet.Remove(entity);
            }

            if (autoSave)
            {
                await DbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public virtual List<TEntity> SearchWithFilters(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, bool>>> filters = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = TableNoTracking;

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            query = query.IncludeMultiple(includes);


            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                throw new Exception();
            }

            return query.ToList();
        }

        public virtual IQueryable<TResult> SearchAndSelectWithFiltersQuery<TResult>(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, TResult>> selectors = null,
            IEnumerable<Expression<Func<TEntity, bool>>> filters = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = TableNoTracking;

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            query = query.IncludeMultiple(includes);


            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query.Select(selectors);
        }

        public virtual IQueryable<TEntity> SearchWithFiltersQuery(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<Expression<Func<TEntity, bool>>> filters = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = TableNoTracking;

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            query = query.IncludeMultiple(includes);


            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }

        public virtual PagedList<TEntity> SearchWithFilters(
            int pageNumber,
            int pageSize,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            IEnumerable<Expression<Func<TEntity, bool>>> filters = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = TableNoTracking;

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            query = query.IncludeMultiple(includes);


            if (orderBy != null)
                query = orderBy(query);

            return new PagedList<TEntity>(query, pageNumber, pageSize);
        }


        public virtual PagedList<TResult> SearchAndSelectWithFilters<TResult>(
            int pageNumber,
            int pageSize,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TResult>> selectors,
            IEnumerable<Expression<Func<TEntity, bool>>> filters = null,
          params Expression<Func<TEntity, object>>[] includes
        )
        {
            var query = TableNoTracking;

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }
            if (includes != null)
                query = query.IncludeMultiple(includes);
            if (orderBy != null)
                query = orderBy(query);

            var queryList = query.Select(selectors);

            return new PagedList<TResult>(queryList, pageNumber, pageSize);
        }

        public virtual PagedList<TResult> SearchAndSelectWithFilters<TResult>(
            int pageNumber,
            int pageSize,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TResult>> selectors,
            IEnumerable<Expression<Func<TEntity, bool>>> filters = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
          params Expression<Func<TEntity, object>>[] includes
        )
        {
            var query = TableNoTracking;

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }
            if (includes != null)
                query = query.IncludeMultiple(includes);

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
                query = orderBy(query);

            var queryList = query.Select(selectors);

            return new PagedList<TResult>(queryList, pageNumber, pageSize);
        }


        public virtual List<TResult> SearchAndSelectWithFilters<TResult>(
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
    Expression<Func<TEntity, TResult>> selectors,
    IEnumerable<Expression<Func<TEntity, bool>>> filters = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
  params Expression<Func<TEntity, object>>[] includes
)
        {
            var query = TableNoTracking;

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }
            if (includes != null)
                query = query.IncludeMultiple(includes);

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var queryList = query.Select(selectors);

            return queryList.ToList();
        }
        public virtual TEntity GetById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual void Delete(object id, bool autoSave = false)
        {
            var entity = GetById(id);
            if (entity == null)
            {
                return;
            }

            Delete(entity, autoSave);
        }

        public virtual async Task DeleteAsync(object id, bool autoSave = false)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return;
            }

            await DeleteAsync(entity, autoSave);
        }


        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate = null,
            List<Expression<Func<TEntity, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query;
            query = (disableTracking) ? TableNoTracking : Table;

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetSingleWithDeepRelationsAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query;
            query = (disableTracking) ? TableNoTracking : Table;

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetWithDeepRelationsAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query;
            query = (disableTracking) ? TableNoTracking : Table;

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query;
            query = (disableTracking) ? TableNoTracking : Table;

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public List<TEntity> GetSync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query;
            query = (disableTracking) ? TableNoTracking : Table;

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).ToList();
            return query.ToList();
        }

        public virtual async Task<List<TEntity>> GetAsync(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          params Expression<Func<TEntity, object>>[] includes)
        {
            var query = TableNoTracking;
            if (filter != null)
                query = query.Where(filter);

            query = query.IncludeMultiple(includes);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }


        public virtual TEntity GetFirst(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = TableNoTracking;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                query = query.IncludeMultiple(includeProperties);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
             params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = TableNoTracking;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                query = query.IncludeMultiple(includeProperties);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIsExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await TableNoTracking.AnyAsync(predicate);
        }

    }

}



