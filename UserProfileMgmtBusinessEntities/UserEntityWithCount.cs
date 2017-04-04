using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProfileMgmtBusinessEntities
{
    public class UserEntityWithCount
    {
        public List<UserEntity> userEntities { get; set; }
        public int userCount { get; set; }
        public int numberOfRowsPerPage { get; set; }

    }
}
