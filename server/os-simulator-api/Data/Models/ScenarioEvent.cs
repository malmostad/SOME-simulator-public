using System;
using System.Security.AccessControl;

namespace SoMeSimulator.Data.Models
{
    /// <summary>
    /// Simulates information from other sources than Social Media
    /// </summary>
    public class ScenarioEvent : AMessageEntity
    {
        public ScenarioEvent(){}

        public int PhaseId { get; set; }
        
        /// <summary>
        /// Occurs in Phase
        /// </summary>
        /// <value></value>
        public virtual Phase Phase { get; set; }
        
        /// <summary>
        /// Describes when in the Phase the Event should occur.
        /// </summary>
        /// <value></value>
        public double TimePercent { get; set; }

    }
}
