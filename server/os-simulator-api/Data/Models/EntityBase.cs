using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SoMeSimulator.Data.Models
{
    [DataContract]
    public class EntityBase
    {
        [JsonProperty("id")]
        [Key]
        [DataMember]
        public int Id { get; set; }
    }
}