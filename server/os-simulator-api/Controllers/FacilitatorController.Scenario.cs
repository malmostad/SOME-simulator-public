using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SoMeSimulator.Data.Models;

namespace SomeSimulator.Controllers
{
    public partial class FacilitatorController
    {
        protected class PhaseTimeDto
        {
            public String Heading { get; set; }
            public double Start { get; set; }
            public double End { get; set; }
        }

        protected class EventTimeDto
        {
            public String Heading { get; set; }
            public double ProgressPoint { get; set; }
        }

        [DataContract] 
        protected class ScenarioDto
        {
            public ScenarioDto()
            {
                Events = new List<EventTimeDto>();
                Phases = new List<PhaseTimeDto>();
            }

            [DataMember] public Scenario Scenario { get; set; }

            [DataMember] public IEnumerable<EventTimeDto> Events { get; set; }

            [DataMember] public IEnumerable<PhaseTimeDto> Phases { get; set; }
        }
    }
}