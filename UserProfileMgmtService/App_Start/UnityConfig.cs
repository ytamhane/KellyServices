using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using UserProfileMgmtBusinessEntities;
using UserProfileMgmtBusinessServices;
using UserProfileDependencyResolver;

namespace UserProfileMgmtService
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            //container.RegisterType<IUserServices, UserServices>(new HierarchicalLifetimeManager());
            //container.RegisterType<IProfileService, ProfileService>(new HierarchicalLifetimeManager());

            RegisterTypes(container);
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            ComponentLoader.LoadContainer(container, ".\\bin", "UserProfileMgmtService.dll");
            ComponentLoader.LoadContainer(container, ".\\bin", "UserProfileMgmtBusinessServices.dll");
        }
    }
}