using System;

namespace AppTemplate.Data.Infrastructure {
  public class Disposable : IDisposable {
    private bool _isDisposed;

    #region IDisposable Members

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

    ~Disposable() {
      Dispose(false);
    }

    private void Dispose(bool disposing) {
      if (!_isDisposed && disposing) {
        DisposeCore();
      }

      _isDisposed = true;
    }

    protected virtual void DisposeCore() {
    }
  }
}