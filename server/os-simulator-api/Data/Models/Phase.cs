using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SoMeSimulator.Services.TimeCalc;

namespace SoMeSimulator.Data.Models
{
    [DataContract]
    public class Phase: EntityBase
    {
        public Phase()
        {
            ScenarioEvents = new List<ScenarioEvent>();
            PostLink = new List<PhasePost>();

        }

        public string Description { get; set; }
        /// <summary>
        /// How many percent into the session should this phase start
        /// </summary>
        /// <value></value>
        public double StartPercent { get; set; }

        /// <summary>
        /// Events in this phase
        /// </summary>
        /// <value></value>
        public virtual ICollection<ScenarioEvent> ScenarioEvents { get; set; }

        public int ScenarioId { get; set; }

        /// <summary>
        /// The phase belongs to this Scenario
        /// </summary>
        /// <value></value>
        public virtual Scenario Scenario { get; set; }

        /// <summary>
        /// Suitable posts for this Phase
        /// </summary>
        /// <value></value>
        public virtual ICollection<PhasePost> PostLink { get; set; }

        /// <summary>
        /// Suitable comments for this phase
        /// </summary>
        /// <value></value>
        public virtual ICollection<PhaseComment> CommentLink { get; set; }

    }
}
