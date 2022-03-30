using System.Collections.Generic;
using System.Runtime.Serialization;
using SoMeSimulator.Data.Models.SessionLogs;

namespace SoMeSimulator.Data.Models
{
    [DataContract]
    public class Post: AMessageEntity
    {
        public Post()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            PhaseLink = new List<PhasePost>();
        }

        /// <summary>
        /// Suitable Phases
        /// </summary>
        /// <value></value>
        public virtual ICollection<PhasePost> PhaseLink { get; set; }

        /// <summary>
        /// Sender Avatar
        /// </summary>
        /// <value></value>
        public string Avatar { get; set; }
        
        /// <summary>
        /// Suitable for MessageFlow
        /// </summary>
        /// <value></value>
        public MessageFlow MessageFlow { get; set; }
    }
}
