using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace SoMeSimulator.Data.Models
{
    public class Usr: EntityBase
    {
        public Usr()
        {
            UserRoles = new List<UserRole>();
        }
        /// <summary>
        /// Username 
        /// </summary>
        /// <value></value>
        public string Username { get; set; }
        
        /// <summary>
        /// Hashed password
        /// </summary>
        /// <value></value>
        public string Password { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        /// <value></value>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Selects all Roles for a user
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Role> GetRoles()
        {
            return UserRoles.Select(r => r.Role).AsEnumerable();
        }

        /// <summary>
        /// SessionsGroup
        /// </summary>
        public virtual SessionGroup ActiveSessionGroup { get; set; }
    }
}
