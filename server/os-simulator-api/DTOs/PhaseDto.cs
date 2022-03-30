using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace SomeSimulator.DTOs
{
    public class PhaseDto: EntityBaseDto
    {
        [Required]
        public string Description { get; set; }
        
        [Required]
        [RegularExpression(@"[0-1]{1,1}(\.[0-9]{1,2})?")]
        public double StartPercent { get; set; }
        
        public ICollection<ScenarioEventDto> ScenarioEvents { get; set; } = new List<ScenarioEventDto>();
        
    }
}