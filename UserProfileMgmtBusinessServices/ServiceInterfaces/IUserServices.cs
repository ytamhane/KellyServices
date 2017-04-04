using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileMgmtBusinessEntities;

namespace UserProfileMgmtBusinessServices
{
    public interface IUserServices
    {
        UserEntityWithCount GetUserById(int uId);
        UserEntityWithCount GetUserById(string userId);
        UserEntityWithCount GetAllUsers(int PageNumber, int numberOfRecords, string userId, bool getDeleted);
        string CreateUser(UserEntity userEntity);
        string UpdateUser(UserEntity userEntity);
        bool DeleteUser(int uId);
        bool DeleteUser(string userId);
        string AssignProfiles(int uID, int[] pIDList, string CreatedName, DateTime CreatedDateTime);
    }
}
