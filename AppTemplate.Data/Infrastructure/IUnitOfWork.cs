namespace AppTemplate.Data.Infrastructure {
  public interface IUnitOfWork {
    void Commit();
    string GetDatabaseSchema();
  }
}