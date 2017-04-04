using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProfileMgmtBusinessEntities
{
    public class UserProfileEntity
    {
        public int uID { get; set; }
        public int pID { get; set; }
        public System.DateTime createdOn { get; set; }
        public string CreatedBy { get; set; }

        public string userId { get; set; }

        public string profileId { get; set; }

        //public virtual ProfileEntity profileEntity { get; set; }
        //public virtual UserEntity userEntity { get; set; }
    }
}
