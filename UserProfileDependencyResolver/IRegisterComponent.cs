using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProfileDependencyResolver
{
    public interface IRegisterComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="withIntercepation"></param>
        void RegisterType<TFrom, TTo>(bool withIntercepation = false) where TTo : TFrom;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="withIntercepation"></param>
        void RegisterTypeWithControlledLifeTime<TFrom, TTo>(bool withIntercepation = false) where TTo : TFrom;
    }
}
