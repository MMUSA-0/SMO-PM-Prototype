using System;
using System.Threading.Tasks;

namespace Framework.Core.Data.Uow
{
    public class UnitOfWorkBase<TContext> : IUnitOfWorkBase<TContext>
        where TContext : IBaseDbContext
    {

        public TContext Context { get; }


        public UnitOfWorkBase(TContext context)
        {
            Context = context;
        }


        public virtual int SaveChanges()
        {
            return Context.SaveChanges();
        }


        public virtual async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        ~UnitOfWorkBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Context?.Dispose();
                var dbContext = this.Context;
                dbContext?.Dispose();
            }
        }


    }
}


