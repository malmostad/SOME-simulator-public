using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SoMeSimulator.Data.Models;

namespace SoMeSimulator.Services.MessageManager.Dto
{
    [DataContract]
    public class IncomingMessage: AMessageEntity
    {
        [DataMember]
        public Guid SessionGroup { get; set; }
    }
}
