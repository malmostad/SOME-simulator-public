using System.ComponentModel.DataAnnotations;

namespace SomeSimulator.DTOs
{
    public class ScenarioEventDto: AMessageDto
    {
        [Required]
        public int PhaseId { get; set; }

        [Required]
        [RegularExpression(@"[0-1]{1,1}(\.[0-9]{1,2})?")]
        public double TimePercent { get; set; }
    }
    
}