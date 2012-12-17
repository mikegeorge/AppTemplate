using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace AppTemplate.Data.Infrastructure {
  public class UnitOfWork : IUnitOfWork {
    private readonly IDatabaseFactory _databaseFactory;
    private AppTemplateContext _dataContext;

    public UnitOfWork(IDatabaseFactory databaseFactory) {
      _databaseFactory = databaseFactory;
    }

    protected AppTemplateContext DataContext {
      get { return _dataContext ?? (_dataContext = _databaseFactory.Get()); }
    }

    public string GetDatabaseSchema() {
      return DataContext.DumpScript();
    }

    public void Commit() {
      DataContext.Commit();
    }

    public IEnumerable<DbEntityValidationResult> GetValidationErrors() {
      return DataContext.GetValidationErrors();
    }
  }
}