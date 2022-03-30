using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SoMeSimulator.Data.Models
{
    [DataContract]
    public abstract class AMessageEntity: EntityBase {
        
        /// <summary>
        /// Sender of the message
        /// </summary>
        /// <value></value>
        [JsonProperty("sender")]
        [DataMember]
        public string Sender { get; set; }
        
        /// <summary>
        /// Heading
        /// </summary>
        /// <value></value>
        [JsonProperty("heading")]
        [DataMember]
        public string Heading { get; set; }

        /// <summary>
        /// Text of the message
        /// </summary>
        /// <value></value>
        [JsonProperty("text")]
        [DataMember]
        public string Text { get; set; }
    }
}