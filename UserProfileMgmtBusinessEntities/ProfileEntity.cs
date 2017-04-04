using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProfileMgmtBusinessEntities
{
    public class ProfileEntity
    {
        public int pID { get; set; }
        public string ProfileId { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public System.DateTime createdOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<UserProfileEntity> UserProfileEntities { get; set; }
    }
}
