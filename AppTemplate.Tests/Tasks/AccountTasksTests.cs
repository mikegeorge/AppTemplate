using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTemplate.Tests.Mocks;
using Moq;
using NUnit.Framework;
using AppTemplate.Data.Repositories;
using AppTemplate.Domain;
using AppTemplate.Tasks;

namespace AppTemplate.Tests.Tasks
{
  [TestFixture]
  public class AccountTasksTests {

    [Test]
    public void AuthenticateUser_ValidUsernamePassword_ReturnsSuccess() {
      var accountService = GetAccountServiceWithValidUser();
      var result = accountService.AuthenticateUser("myname@test.com", "jzs3qv7d");
      Assert.AreEqual(AccountTasks.AuthenticationResult.Success, result);
    }

    [Test]
    public void AuthenticateUser_InvalidPassword_ReturnsInvalidPassword() {
      var accountService = GetAccountServiceWithValidUser();
      var result = accountService.AuthenticateUser("myname@test.com", "shouldnotwork");
      Assert.AreEqual(AccountTasks.AuthenticationResult.InvalidPassword, result);
    }

    [Test]
    public void AuthenticateUser_EnabledUser_ReturnsSuccess() {
      var accountService = GetAccountServiceWithValidUser();
      var result = accountService.AuthenticateUser("myname@test.com", "jzs3qv7d");
      Assert.AreEqual(AccountTasks.AuthenticationResult.Success, result);
    }

    [Test]
    public void AuthenticateUser_DisabledUser_ReturnsAccountDisabled() {
      var userRepositoryMock = new Mock<IUserRepository>();
      var disabledUser = new User {
        Username = "Test User",
        Email = "myname@test.com",
        Disabled = true,
        PasswordHash = "$2a$12$h0qgj3gpg4939GulaP6yPuBeuaWGlwZq0aiq1atMGpkxZ.XVWZEhe",
        PasswordSalt = "$2a$12$h0qgj3gpg4939GulaP6yPu"
      };
      userRepositoryMock.Setup(x => x.Get("disabled@test.com")).Returns(disabledUser);
      var accountService = new AccountTasks(UnitOfWorkMock.Create(), userRepositoryMock.Object);

      var result = accountService.AuthenticateUser("disabled@test.com", "jzs3qv7d");

      // assert
      Assert.AreEqual(AccountTasks.AuthenticationResult.AccountDisabled, result);
    }

    private static AccountTasks GetAccountServiceWithValidUser() {
      var userRepositoryMock = new Mock<IUserRepository>();
      var myUser = new User {
        Username = "Test User",
        Email = "myname@test.com",
        Disabled = false,
        PasswordHash = "$2a$12$h0qgj3gpg4939GulaP6yPuBeuaWGlwZq0aiq1atMGpkxZ.XVWZEhe",
        PasswordSalt = "$2a$12$h0qgj3gpg4939GulaP6yPu",
      };
      userRepositoryMock.Setup(x => x.Get("myname@test.com")).Returns(myUser);
      var accountService = new AccountTasks(UnitOfWorkMock.Create(), userRepositoryMock.Object);
      return accountService;
    }
  }}
