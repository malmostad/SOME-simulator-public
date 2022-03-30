using System.Runtime.Serialization;

namespace SoMeSimulator.Data.Models
{
    [DataContract]
    public class PhaseComment
    {
        public PhaseComment()
        {
        }
        public PhaseComment(Comment comment, Phase phase)
        {
            Comment = comment;
            Phase = phase;
        }

        public int PhaseId { get; set; }
        public int CommentId { get; set; }

        /// <summary>
        /// Link to Comment
        /// </summary>
        /// <value></value>
        public virtual Comment Comment { get; set; }
        /// <summary>
        /// Link to Phase
        /// </summary>
        /// <value></value>
        public virtual Phase Phase { get; set; }
    }
}