using System;
using System.Collections.Generic;
using System.Linq;

namespace AppTemplate.Data.Infrastructure {
  public interface IRepository<T> where T : class {
    IQueryable<T> GetAll();
    IEnumerable<T> GetMany(Func<T, bool> where);
    T Get(int id);
    T Get(Func<T, Boolean> where);
    void Add(T entity);
    T Delete(int id);
    void Delete(T entity);
    void Delete(Func<T, Boolean> predicate);
  }
}