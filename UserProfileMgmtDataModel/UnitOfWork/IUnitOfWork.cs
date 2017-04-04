using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileMgmtDataModel.GenericRepository;


namespace UserProfileMgmtDataModel.UnitOfWork
{
    public interface IUnitOfWork
    {
        #region Properties
        GenericRepository<Profile> ProfileRepository { get; }
        GenericRepository<User> UserRepository { get; }
        GenericRepository<UserProfile> UserProfileRepository { get; }

        #endregion

        #region Public Methods
        /// <summary>
        /// Save Method
        /// </summary>
        void Save();
        #endregion
    }
}
