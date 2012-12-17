using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTemplate.Domain;
using AppTemplate.Data.Infrastructure;

namespace AppTemplate.Data.Repositories
{
  public interface IUserRepository : IRepository<User>
  {
    /// <summary>
    /// Get by Email Address
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    User Get(string email);
  }

  public class UserRepository : RepositoryBase<User>, IUserRepository
  {
    public UserRepository(IDatabaseFactory databaseFactory)
      : base(databaseFactory)
    {
    }

    public User Get(string email)
    {
      return DataContext.Users.SingleOrDefault(x => x.Email == email);
    }
  }
}
