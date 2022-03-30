using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SoMeSimulator.Data.Models.Comments;
using SoMeSimulator.Data.Models.SessionLogs;

namespace SoMeSimulator.Data.Models
{
    public class Comment: EntityBase
    {
        public Comment()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            PhaseLink = new List<PhaseComment>();
        }
        /// <summary>
        /// Belongs to Scenario
        /// </summary>
        /// <value></value>
        public virtual Scenario Scenario { get; set; }
        
        /// <summary>
        /// Message
        /// </summary>
        /// <value></value>
        public string Text { get; set; }
        
        /// <summary>
        /// Props like positive, negative, neutral
        /// </summary>
        /// <value></value>
        public CommentProperties Props { get; set; }

        /// <summary>
        /// Alias of the sender
        /// </summary>
        /// <value></value>
        public string Sender { get; set; }

        /// <summary>
        /// Avatar image
        /// </summary>
        /// <value></value>
        public string Avatar { get; set; }

        /// <summary>
        /// What message flows is this message suitable for
        /// </summary>
        /// <value></value>
        public MessageFlow MessageFlow { get; set; }

        /// <summary>
        /// Phases for this comment
        /// </summary>
        /// <value></value>
        public virtual ICollection<PhaseComment> PhaseLink { get; set; }

    }
}
