using AppTemplate.Domain;
using DevOne.Security.Cryptography.BCrypt;

namespace AppTemplate.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AppTemplate.Data.AppTemplateContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppTemplate.Data.AppTemplateContext context)
        {
          string passwordSalt = BCryptHelper.GenerateSalt(12);

          context.Users.AddOrUpdate(
            p => p.Username,
            new User() {
              Username = "admin",
              PasswordSalt = passwordSalt,
              Email = "admin@test.com",
              PasswordHash = BCryptHelper.HashPassword("admin", passwordSalt),
              DateCreated = DateTime.Now,
              DateLastPasswordChange = DateTime.Now,
              DateLastLogin = DateTime.Now,
              UserRole = UserRole.Administrator,
              Agreement = true
            }
            );
        }
    }
}
