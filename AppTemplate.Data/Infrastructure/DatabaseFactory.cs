
namespace AppTemplate.Data.Infrastructure {
  public class DatabaseFactory : Disposable, IDatabaseFactory {
    private AppTemplateContext _dataContext;

    #region IDatabaseFactory Members

    public AppTemplateContext Get() {
      if (_dataContext != null) return _dataContext;
      else return _dataContext = new AppTemplateContext();
    }

    #endregion

    protected override void DisposeCore() {
      if (_dataContext != null)
        _dataContext.Dispose();
    }
  }
}