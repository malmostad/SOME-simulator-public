using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SoMeSimulator.Data.Models
{
    public class UserRole: EntityBase
    {
        public int UsrId { get; set; }
        public int RoleId { get; set; }
        public virtual Usr Usr { get; set; }
        [DataMember]
        public virtual Role Role { get; set; }
    }
}
