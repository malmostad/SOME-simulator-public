using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SomeSimulator.DTOs
{
    public class PostDto : AMessageDto
    {
        [Required]
        public MessageFlowDto MessageFlow { get; set; }
        public string Avatar { get; set; }
        
        [MinLength(1)]
        public ICollection<int> Phases { get; set; } = new List<int>();
    }
    
    public enum MessageFlowDto: uint
    {
        Short = 1,
        Long = 2
    }
}