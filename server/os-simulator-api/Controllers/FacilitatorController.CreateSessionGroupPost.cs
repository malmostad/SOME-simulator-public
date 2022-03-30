namespace SomeSimulator.Controllers
{
    public partial class FacilitatorController
    {
        public class CreateSessionGroupPost
        {
            public int ScenarioId { get; set; }
            public uint Minutes { get; set; }
            public string GroupName { get; set; }
        }
    }
}