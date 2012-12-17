using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTemplate.Data.Infrastructure;
using Moq;

namespace AppTemplate.Tests.Mocks {
  public static class UnitOfWorkMock {
    public static IUnitOfWork Create() {
      var mock = new Mock<IUnitOfWork>();

      return mock.Object;
    }
  }
}
