using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore.Internal;

namespace SoMeSimulator.Data.Models
{
    [DataContract]
    public class Scenario: EntityBase
    {
        
        public Scenario()
        {
            Phases = new List<Phase>();
            Sessions = new List<Session>();
            Comments = new List<Comment>();
        }
        
        /// <summary>
        /// Scenario Name
        /// </summary>
        /// <value></value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Scenario Description
        /// </summary>
        /// <value></value>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Sessions that is based on this Scenario
        /// </summary>
        /// <value></value>
        public virtual ICollection<Session> Sessions { get; set; }
        /// <summary>
        /// Phases of the scenario
        /// </summary>
        /// <value></value>
        public virtual ICollection<Phase> Phases { get; set; }
        /// <summary>
        /// The Scenarios Comments
        /// </summary>
        /// <value></value>
        public virtual ICollection<Comment> Comments { get; set; }
    }
}