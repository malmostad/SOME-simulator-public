using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SoMeSimulator.Data.Models
{
    /// <summary>
    /// A training session for one participant
    /// </summary>
    [DataContract]
    public class Session: EntityBase
    {
        /// <summary>
        /// Unique identifier for the session
        /// </summary>
        /// <value></value>
        [DataMember] public Guid SessionGuid { get; set; }

        /// <summary>
        /// All relatives to AMessageEntities is stored as a SessionLog once it is posted.
        /// </summary>
        /// <value></value>
        [DataMember] public virtual ICollection<SessionLog> SessionLogs { get; set; }

        /// <summary>
        /// Links the participants to the Session
        /// </summary>
        /// <value></value>
        public virtual SessionGroup SessionGroup { get; set; }

        [DataMember] public string Participant { get; set; }

        /// <summary>
        /// Calculates the number of SessionLogs totally for the Session.
        /// </summary>
        /// <returns></returns>
        public int CountSessionLogs()
        {
            return SessionLogs.Where(s => s.Level == 1).ToList().Count + SessionLogs.Sum(sl => sl.NumberOfChildren());
        }
    }
}