using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.Types;

namespace SoMeSimulator.Services.MessageManager.Dto
{
    [DataContract]
    public class Message
    {
        [JsonProperty("messageCount")]
        [DataMember]
        public int MessageCount { get; set; }
        
        [JsonProperty("sessionLog")]
        [DataMember]
        public SessionLog SessionLog { get; set; }
        
        [JsonProperty("sessionGroup")]
        [DataMember]
        public string SessionGroup { get; set; }
        
        public Message OnlySessionLog()
        {
            return new Message
            {
                SessionLog = SessionLog
            };
        }
    }
}