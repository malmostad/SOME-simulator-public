using System;

namespace SomeSimulator.Controllers
{
    public partial class FacilitatorController
    {
        public class JoinSessionPost
        {
            public string TypeableCode { get; set; }
            public string Participant { get; set; }
        }
    }
}