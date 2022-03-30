using System.Collections.Generic;

namespace SomeSimulator.DTOs
{
    public class ScenarioDto: EntityBaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<PhaseDto> Phases { get; set; } = new List<PhaseDto>();
        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public ICollection<PostDto> Posts { get; set; } = new List<PostDto>();
    }
}