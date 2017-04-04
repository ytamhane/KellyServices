using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileMgmtDataModel;

namespace TestsHelper
{
    /// <summary>
    /// Data initializer for unit tests
    /// </summary>
    public class DataInitializer
    {
        /// <summary>
        /// Dummy Users
        /// </summary>
        /// <returns></returns>
        public static List<User> GetAllUsers()
        {
            var users = new List<User>
            {
                new User()
                {
                    UserID="TEST0001",
                    UserName="TEST USER 1"
                },
                new User()
                {
                    UserID="TEST0002",
                    UserName="TEST USER 2"
                },
                new User()
                {
                    UserID="TEST0003",
                    UserName="TEST USER 3"
                },
                new User()
                {
                    UserID="TEST0004",
                    UserName="TEST USER 4"
                }
            };
            return users;
        }


        /// <summary>
        /// Dummy Profiles
        /// </summary>
        /// <returns></returns>
        public static List<Profile> GetAllProfiles()
        {
            var profiles = new List<Profile>
            {
                new Profile()
                {
                    pID=1,
                    ProfileId="TESTP001",
                    Description="TEST Profile 1"
                },
                                new Profile()
                {
                    pID=2,
                    ProfileId="TESTP001",
                    Description="TEST Profile 2"
                },
                                                new Profile()
                {
                    pID=3,
                    ProfileId="TESTP003",
                    Description="TEST Profile 3"
                },
            };
            return profiles;
        }

    }

}

