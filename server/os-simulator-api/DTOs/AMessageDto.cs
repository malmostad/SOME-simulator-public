using System.ComponentModel.DataAnnotations;

namespace SomeSimulator.DTOs
{
    public abstract class AMessageDto: EntityBaseDto
    {
        [Required]
        public string Sender { get; set; }
        public string Heading { get; set; }
        [Required]
        public string Text { get; set; }
    }
}