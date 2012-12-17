using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AppTemplate.Data.Infrastructure;
using AppTemplate.Data;

namespace AppTemplate.Data.Repositories {
  public abstract class RepositoryBase<T> where T : class {
    private readonly IDbSet<T> _dbset;
    private AppTemplateContext _dataContext;

    protected RepositoryBase(IDatabaseFactory databaseFactory) {
      DatabaseFactory = databaseFactory;
      _dbset = DataContext.Set<T>();
    }

    protected IDatabaseFactory DatabaseFactory { get; private set; }

    protected AppTemplateContext DataContext
    {
      get { return _dataContext ?? (_dataContext = DatabaseFactory.Get()); }
    }

    public virtual IQueryable<T> GetAll() {
      return _dbset;
    }

    public virtual IEnumerable<T> GetMany(Func<T, bool> where) {
      return _dbset.Where(where);
    }

    public virtual T Get(int id) {
      return _dbset.Find(id);
    }

    public T Get(Func<T, Boolean> where) {
      return _dbset.Where(where).FirstOrDefault();
    }

    public virtual void Add(T entity) {
      _dbset.Add(entity);
    }

    public virtual T Delete(int id) {
      T entity = _dbset.Find(id);

      if (entity != null)
        _dbset.Remove(entity);

      return entity;
    }

    public virtual void Delete(T entity) {
      _dbset.Remove(entity);
    }

    public void Delete(Func<T, Boolean> where) {
      IEnumerable<T> objects = _dbset.Where(where).AsEnumerable();
      foreach (T obj in objects)
        _dbset.Remove(obj);
    }
  }
}