using System;
using AppTemplate.Data;

namespace AppTemplate.Data.Infrastructure {
  public interface IDatabaseFactory : IDisposable {
    AppTemplateContext Get();
  }
}