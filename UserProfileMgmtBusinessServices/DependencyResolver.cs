using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileDependencyResolver;
using System.ComponentModel.Composition;
using UserProfileMgmtDataModel;
using UserProfileMgmtDataModel.UnitOfWork;

namespace UserProfileMgmtBusinessServices
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IUserServices, UserServices>();
            registerComponent.RegisterType<IProfileService, ProfileService>();
        }
    }
}
