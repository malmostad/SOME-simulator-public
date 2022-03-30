using System.Runtime.Serialization;
using SoMeSimulator.Data.Models.Types;

namespace SoMeSimulator.Services.MessageManager.Dto
{
    [DataContract]
    public class IncomingComment : IncomingMessage
    {
        [DataMember]
        public int? SessionLogId { get; set; }

        [DataMember]
        public string Avatar { get; set; }
        
        [DataMember]
        public MessageType MessageType { get; set; }
    }
}