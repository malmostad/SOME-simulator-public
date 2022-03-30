using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SoMeSimulator.Data.Models
{
    /// <summary>
    /// Many to many table
    /// </summary>
    [DataContract]
    public class PhasePost
    {
        public PhasePost(Post post, Phase phase)
        {
            Post = post;
            Phase = phase;
        }

        public PhasePost()
        {
        }

        public int PhaseId  { get; set; }
        public int PostId { get; set; }
        /// <summary>
        /// Post linked to Phase
        /// </summary>
        /// <value></value>
        public virtual Post Post { get; set; }

        /// <summary>
        /// Phase linked to Post
        /// </summary>
        /// <value></value>
        public virtual Phase Phase { get; set; }
    }
}
