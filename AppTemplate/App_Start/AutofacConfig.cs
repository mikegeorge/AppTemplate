using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using AppTemplate.Services;
using Autofac;
using Autofac.Integration.Mvc;
using AppTemplate.Data.Infrastructure;
using AppTemplate.Data.Repositories;
using AppTemplate.Tasks;

namespace AppTemplate.App_Start {
  public class AutofacConfig {
    public static void Start() {
      var builder = new ContainerBuilder();
      builder.RegisterFilterProvider();
      builder.RegisterControllers(Assembly.GetExecutingAssembly());

      builder.RegisterType<AppFormsAuthentication>().As<IFormsAuthentication>().InstancePerHttpRequest();
      builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Mailer")).
        AsImplementedInterfaces().InstancePerHttpRequest();

      builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerHttpRequest();
      builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerHttpRequest();

      // Register Repositories
      builder.RegisterAssemblyTypes(typeof(UserRepository).Assembly)
        .Where(t => t.Name.EndsWith("Repository"))
        .AsImplementedInterfaces().InstancePerHttpRequest();

      // Register Services
      builder.RegisterAssemblyTypes(typeof(AccountTasks).Assembly)
        .Where(t => t.Name.EndsWith("Tasks"))
        .AsImplementedInterfaces().InstancePerHttpRequest();

      var container = builder.Build();
      DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
    }
  }
}