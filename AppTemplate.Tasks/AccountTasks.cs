using System;
using System.Linq;
using AppTemplate.Domain;
using DevOne.Security.Cryptography.BCrypt;
using AppTemplate.Data.Infrastructure;
using AppTemplate.Data.Repositories;

namespace AppTemplate.Tasks
{
  public interface IAccountTasks {
    AccountTasks.AuthenticationResult AuthenticateUser(string email, string password);
    AccountTasks.ChangePasswordResult ChangePassword(string email, string password, string newPassword);
    string ForgotPassword(string email);
    AccountTasks.RegisterUserResult RegisterUser(string username, string email);
    string GetUserRole(string email);
  }


  public class AccountTasks : IAccountTasks {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public AccountTasks(IUnitOfWork unitOfWork, IUserRepository userRepository) {
      _unitOfWork = unitOfWork;
      _userRepository = userRepository;
    }

    public AuthenticationResult AuthenticateUser(string email, string password) {

      var user = _userRepository.Get(email);

      if (user != null)
      {
        if (BCryptHelper.CheckPassword(password, user.PasswordHash))
        {
          if (!user.Disabled)
          {
              user.DateLastLogin = DateTime.Now;
              _unitOfWork.Commit();
              return AuthenticationResult.Success;
          }
          return AuthenticationResult.AccountDisabled;
        }
        return AuthenticationResult.InvalidPassword;
      }
      return AuthenticationResult.NotRegistered;
    }

    public ChangePasswordResult ChangePassword(string email, string password, string newPassword) {
      var user = _userRepository.Get(email);
      if (BCryptHelper.CheckPassword(password, user.PasswordHash)) {
        var passwordSalt = BCryptHelper.GenerateSalt(12);

        user.PasswordHash = BCryptHelper.HashPassword(newPassword, passwordSalt);
        user.PasswordSalt = passwordSalt;
        user.DateLastPasswordChange = DateTime.Now;
        user.PasswordNeedsUpdating = false;
        _unitOfWork.Commit();
        return ChangePasswordResult.Success;
      }
      return ChangePasswordResult.InvalidPassword;
    }

    public string ForgotPassword(string email) {
      var user = _userRepository.Get(email);
      if (user != null) {
        // Generate new Password 
        var password = GeneratePassword(8);
        var passwordSalt = BCryptHelper.GenerateSalt(12);
        // Save
        user.PasswordHash = BCryptHelper.HashPassword(password, passwordSalt);
        user.PasswordSalt = passwordSalt;
        user.DateLastPasswordChange = DateTime.Now;
        user.PasswordNeedsUpdating = true;
        _unitOfWork.Commit();

        return password;
      }
      return string.Empty;
    }

    public RegisterUserResult RegisterUser(string username, string email) {
      // check if username is available
      if (_userRepository.GetAll().SingleOrDefault(x => x.Username == username) != null)
        return new RegisterUserResult() {RegistrationRequestResult = RegistrationRequestResult.UsernameTaken};
      if (_userRepository.GetAll().SingleOrDefault(x => x.Email == email) != null)
        return new RegisterUserResult() {RegistrationRequestResult = RegistrationRequestResult.EmailRegistered};

      // check if email has been registered
      var password = GeneratePassword(8);
      string passwordSalt = BCryptHelper.GenerateSalt(12);
      var user = new User
      {
        Username = username,
        Email = email,
        DateCreated = DateTime.Now,
        PasswordHash = BCryptHelper.HashPassword(password, passwordSalt),
        PasswordSalt = passwordSalt,
        DateLastPasswordChange = DateTime.Now,
        PasswordNeedsUpdating = true
      };
      // save
      _userRepository.Add(user);
      _unitOfWork.Commit();
      return new RegisterUserResult()
      {
        TempPassword = password,
        RegistrationRequestResult = RegistrationRequestResult.Success
      };
    }

    public string GetUserRole(string email) {
      var user = _userRepository.Get(email);
      return user.UserRole.ToString().ToLower();
    }

    private static string GeneratePassword(int passwordLength) {
      const string allowedChars = "abcdefghijkmnopqrstuvwxyz123456789";
      var randNum = new Random();
      var chars = new char[passwordLength];

      for (var i = 0; i < passwordLength; i++) {
        chars[i] = allowedChars[(int)((allowedChars.Length) * randNum.NextDouble())];
      }

      return new string(chars);
    }

    public class RegisterUserResult {
      public string TempPassword { get; set; }
      public AccountTasks.RegistrationRequestResult RegistrationRequestResult { get; set; }
    }


    public enum AuthenticationResult
    {
      Success,
      NotRegistered,
      InvalidPassword,
      AccountDisabled,
      ServerError
    }

    public enum ChangePasswordResult {
      Success,
      InvalidPassword,
      ServerError
    }
    public enum RegistrationRequestResult {
      Success,
      UsernameTaken,
      EmailRegistered,
    }
  }
}
