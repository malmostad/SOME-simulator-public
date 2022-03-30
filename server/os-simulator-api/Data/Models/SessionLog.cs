using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SoMeSimulator.Data.Models.SessionLogs;
using SoMeSimulator.Data.Models.Types;

namespace SoMeSimulator.Data.Models
{
    [DataContract]
    public class SessionLog : AMessageEntity
    {
        public SessionLog()
        {
            Children = new List<SessionLog>();
            BotReplyProperties = BotReplyProperties.Neutral;
        }
        
        /// <summary>
        /// Returns a SessionGuid if it is available
        /// </summary>
        /// <value></value>
        [JsonProperty("sessionGuid")]
        [DataMember] public string SessionGuid  {
            get
            {
                if (Type != MessageType.Participant && Type != MessageType.Message)
                {
                    return "";
                }
                return  Session.SessionGuid.ToString();
            } }
        
        /// <summary>
        /// Describes the SessionLog
        /// </summary>
        /// <value></value>
        [JsonProperty("sessionLogTag")]
        [DataMember] public SessionLogTag Tag { get; set; }

        /// <summary>
        /// Various proprties for bot messages like positive, negative, neutral
        /// </summary>
        /// <value></value>
        [JsonProperty("botReplyProperties")]
        [DataMember] public BotReplyProperties BotReplyProperties { get; set; }

        /// <summary>
        /// Describes in what level of the SessionLog hierarchy thus entity belongs
        /// </summary>
        /// <value></value>
        [JsonProperty("level")]
        [DataMember] public int Level { get; set; }

        /// <summary>
        /// If based on a Post the Posts Id is stored here
        /// </summary>
        /// <value></value>
        [JsonProperty("postId")]
        [DataMember] public int? PostId { get; set; }

        /// <summary>
        /// If based on a Comment the Comments Id is stored here
        /// </summary>
        /// <value></value>
        [JsonProperty("commentId")]
        [DataMember] public int? CommentId { get; set; }

        /// <summary>
        /// If based on a ScenarioEvent the ScenarioEvents Id is stored here
        /// </summary>
        /// <value></value>
        [JsonProperty("scenarioEventId")]
        [DataMember] public int? ScenarioEventId { get; set; }

        /// <summary>
        /// What MesssageFlow this log was posted.
        /// </summary>
        /// <value></value>
        [JsonProperty("messageFlow")]
        [DataMember] public MessageFlow MessageFlow { get; set; }

        /// <summary>
        /// Describes what type of message this SessionLog created from
        /// </summary>
        /// <value></value>
        [JsonProperty("messageType")]
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        /// <summary>
        /// Describes when this message was sent
        /// </summary>
        /// <value></value>
        [JsonProperty("sendDateTime")]
        [DataMember] public DateTime? SendDateTime { get; set; }

        [JsonProperty("sessionId")]
        [DataMember] public int? SessionId { get; set; }

        /// <summary>
        /// This SessionLog belongs to this Session
        /// </summary>
        /// <value></value>
        public virtual Session Session { get; set; }

        /// <summary>
        /// Child SessionLogs
        /// </summary>
        /// <value></value>
        [JsonProperty("children")]
        [DataMember] public virtual ICollection<SessionLog> Children { get; set; }

        /// <summary>
        /// Parent SessionLog
        /// </summary>
        /// <value></value>
        public virtual SessionLog Parent { get; set; }

        public int? ParentSessionLogId { get; set; }

        /// <summary>
        /// Avatar
        /// </summary>
        /// <value></value>
        [JsonProperty("avatar")]
        [DataMember] public string Avatar { get; set; }

        /// <summary>
        /// Recursive calculation of number of children
        /// </summary>
        /// <returns></returns>
        public int NumberOfChildren()
        {
            if (Children.Any())
            {
                var children = Children.Distinct().ToList();

                var list = new List<SessionLog>();
                list.AddRange(children);

                foreach (var child in children)
                {
                    list.Add(child);
                    child.NumberOfChildren();
                }


                return list.Distinct().Count();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Select Root SessionLog
        /// </summary>
        /// <returns></returns>
        public SessionLog Root()
        {
            var elm = this;

            while (elm.Level > 1)
            {
                elm = elm.Parent;
            }

            return elm;
        }
    }
}