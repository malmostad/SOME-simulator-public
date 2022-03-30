using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SoMeSimulator.Data.Models.Comments;

namespace SomeSimulator.DTOs
{
    public class CommentDto : EntityBaseDto
    {
        
        [Required]
        public string Text { get; set; }
        public int ScenarioId { get; set; }
        public List<int> Phases { get; set; } = new List<int>();
        public CommentProperties Props { get; set; }
        [Required]
        public string Sender { get; set; }
        public string Avatar { get; set; }
        public MessageFlowDto MessageFlow { get; set; }
    }
    public enum CommentPropertiesDto: uint
    {
        Positive = 1,
        Negative = 2,
        Neutral = 4,

        Easy = 8,
        Difficult = 16,

        Reply = 32,
    }
}