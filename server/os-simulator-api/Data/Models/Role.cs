using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SoMeSimulator.Data.Models
{
    public class Role : EntityBase
    {
        public Role()
        {
            UserRoles = new List<UserRole>();
        }
        /// <summary>
        /// Role Name
        /// </summary>
        /// <value></value>
        [DataMember]
        public string Name { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}