using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileMgmtBusinessEntities;

namespace UserProfileMgmtBusinessServices
{
    public interface IProfileService
    {
        ProfileEntityWithCount GetProfileById(int pId);
        ProfileEntityWithCount GetProfileById(string profileId);
        ProfileEntityWithCount GetAllProfiles(int PageNumber, int numberOfRecords, string profileId, bool getDeleted);
        string CreateProfile(ProfileEntity ProfileEntity);
        string UpdateProfile(ProfileEntity ProfileEntity);
        bool DeleteProfile(int pId);
        bool DeleteProfile(string profileId);
    }
}
