using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Repository;
using Omu.ProDinner.Infra;
using Omu.ValueInjecter;

namespace Omu.ProDinner.Data
{
    public class UniRepo : IUniRepo
    {
        private readonly DbContext c;

        public UniRepo(IDbContextFactory a)
        {
            c = a.GetContext();
        }

        public T Insert<T>(T o) where T : Entity, new()
        {
            var t = new T();
            t.InjectFrom(o);
            c.Set<T>().Add(t);
            return t;
        }

        public void Save()
        {
            c.SaveChanges();
        }

        public T Get<T>(int id) where T : Entity
        {
            return c.Set<T>().Find(id);
        }

        public IQueryable<T> GetAll<T>(bool showDeleted) where T : DelEntity
        {
            if (showDeleted)
            {
                return c.Set<T>();
            }

            return c.Set<T>().Where(o => !o.IsDeleted);
        }
    }
}