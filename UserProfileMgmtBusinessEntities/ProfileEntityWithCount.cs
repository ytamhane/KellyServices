using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProfileMgmtBusinessEntities
{
    public class ProfileEntityWithCount
    {
        public List<ProfileEntity> profileEntities { get; set; }
        public int profileCount { get; set; }
        public int numberOfRowsPerPage { get; set; }
    }
}
