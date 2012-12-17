using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTemplate.Data.Infrastructure;
using AppTemplate.Data.Repositories;
using AppTemplate.Domain;

namespace AppTemplate.Tasks {
  public interface IUserTasks {
    IList<User> GetList();
    User GetUser(int id);
    User UpdateUser(int id, User userFromForm);
    void DeleteUser(int id);
  }
  public class UserTasks : IUserTasks {
    private readonly IUserRepository _userRepository;
    private IUnitOfWork _unitOfWork;

    public UserTasks(IUnitOfWork unitOfWork, IUserRepository userRepository) {
      _unitOfWork = unitOfWork;
      _userRepository = userRepository;
    }

    public IList<User> GetList() {
      return _userRepository.GetAll().OrderBy(x => x.Email).ToList();
    }

    public User GetUser(int id) {
      return _userRepository.Get(id);
    }

    public User UpdateUser(int id, User userFromForm) {
      var user = _userRepository.Get(id);
      user.Username = userFromForm.Username;
      user.Email = userFromForm.Email;
      user.UserRole = userFromForm.UserRole;
      user.Comment = userFromForm.Comment;
      user.Disabled = userFromForm.Disabled;
      user.Agreement = userFromForm.Agreement;
      user.PasswordNeedsUpdating = userFromForm.PasswordNeedsUpdating;

      _unitOfWork.Commit();

      return user;
    }

    public void DeleteUser(int id) {
      var user = _userRepository.Get(id);
      _userRepository.Delete(user);
      _unitOfWork.Commit();
    }
  }
}
