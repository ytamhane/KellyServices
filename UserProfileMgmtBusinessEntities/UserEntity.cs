using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProfileMgmtBusinessEntities
{
    public class UserEntity
    {
        public int uID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public bool Active { get; set; }
        public System.DateTime createdOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<UserProfileEntity> UserProfileEntities { get; set; }
    }
}
