using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using StockDashboard.Core.Common.Utils;
using StockDashboard.Core.Interfaces;
using StockDashboard.Core.Model;

namespace StockDashboard.Core
{
    public abstract class DataRepositoryBase<T, U> : IDataRepository<T>
        where T : class, IIdentifiableEntity, new()
        where U : DbContext, new()
    {
        protected abstract T AddEntity(U entityContext, T entity);

        protected abstract T UpdateEntity(U entityContext, T entity);

        protected abstract T GetEntity(U entityContext, int id);

        protected abstract IEnumerable<T> GetEntities(U entityContext);

        public T Add(T entity)
        {
            using (U entityContext = new U())
            {
                T addedEntity = AddEntity(entityContext, entity);
                entityContext.SaveChanges();
                return addedEntity;
            }
        }

        public void Remove(T entity)
        {
            using (U entityContext = new U())
            {
                entityContext.Entry<T>(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }


        public void Remove(int id)
        {
            using (U entityContext = new U())
            {
                T entity = GetEntity(entityContext, id);
                entityContext.Entry<T>(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public IEnumerable<T> Get()
        {
            using (U entityContext = new U())
                return (GetEntities(entityContext)).ToArray().ToList();
        }

        public T Get(int id)
        {
            using (U entityContext = new U())
                return GetEntity(entityContext, id);
        }

        public T Update(T entity)
        {
            using (U entityContext = new U())
            {
                T existingEntity = UpdateEntity(entityContext, entity);
                SimpleMapper.PropertyMap(entity, existingEntity);
                entityContext.SaveChanges();
                return existingEntity;
            }
        }

        public  OperationStatus Save<E>(E entity) where E: class
        {
            OperationStatus opStatus = new OperationStatus { Status = true };

            using (U entityContext = new U())
            {
                try
                {
                    opStatus.Status = entityContext.SaveChanges() > 0;
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromException("Error saving " + typeof(E) + ".", exp);
                }
            }
            return opStatus;
        }
    }
}
