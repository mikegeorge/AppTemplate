using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using AppTemplate.Domain;

namespace AppTemplate.Data {

  public class AppTemplateContext : DbContext {
    public DbSet<User> Users { get; set; }

    public virtual void Commit()
    {
      base.SaveChanges();
    }

    public string DumpScript()
    {
      return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
    }
  }

}