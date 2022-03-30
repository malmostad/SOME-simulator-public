using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SoMeSimulator.Data.Models.Defaults;
using SoMeSimulator.Data.Models.SessionGroups.Status;
using Stateless;

namespace SoMeSimulator.Data.Models
{
    /// <summary>
    /// A group of Sessions for one training session
    /// /// </summary>
    [DataContract]
    public class SessionGroup : EntityBase
    {
        [NotMapped] public StateMachine<SessionStatus, SessionTriggers> StateMachine { get; private set; }

        public SessionGroup()
        {
            Status = SessionStatus.New;

            Duration = DefaultValues.DefaultSessionRunTime;

            StateMachine = new StateMachine<SessionStatus, SessionTriggers>(() => Status, s => Status = s);

            StateMachine.Configure(SessionStatus.New)
                .Permit(SessionTriggers.Start, SessionStatus.Running)
                .Permit(SessionTriggers.Cancel, SessionStatus.Cancelled);


            StateMachine.Configure(SessionStatus.Running)
                .Permit(SessionTriggers.Pause, SessionStatus.Paused)
                .Permit(SessionTriggers.Stop, SessionStatus.Finished);

            StateMachine.Configure(SessionStatus.Paused)
                .SubstateOf(SessionStatus.Running)
                .Permit(SessionTriggers.UnPause, SessionStatus.Running);
        }

        /// <summary>
        /// Checks if the SessionGroup and the Usr can decouple.
        /// </summary>
        /// <returns></returns>
        public bool CanDecouple()
        {
            if (this.Usr == null)
            {
                return false;
            }
            return (StateMachine.IsInState(SessionStatus.Cancelled) || StateMachine.IsInState(SessionStatus.Finished));
        }
        
        /// <summary>
        /// Determines how plausible it is that some kind of message is sent.
        /// </summary>
        /// <value></value>
        [DataMember] public uint StressLevel { get; set; }

        /// <summary>
        /// Describes what status the SessionGroups is in.
        /// </summary>
        /// <value></value>
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public SessionStatus Status { get; set; }

        /// <summary>
        /// Describes how long the training occasion is
        /// </summary>
        /// <value></value>
        [DataMember] public TimeSpan Duration { get; set; }

        [DataMember] public int ScenarioId { get; set; }

        /// <summary>
        /// The Scenario that is used
        /// </summary>
        /// <value></value>
        public virtual Scenario Scenario { get; set; }

        /// <summary>
        /// Indicates when the statused changed to Running
        /// </summary>
        /// <value></value>
        [DataMember] public DateTime? StartDate { get; set; }

        /// <summary>
        /// Describes when the status changed to one of the final statuses
        /// </summary>
        /// <value></value>
        [DataMember] public DateTime? StopDate { get; set; }

        /// <summary>
        /// Is set to the current Phase of the SessionGroup
        /// </summary>
        /// <value></value>
        public virtual Phase CurrentPhase { get; set; }

        /// <summary>
        /// Sessions
        /// </summary>
        /// <value></value>
        [DataMember] public virtual ICollection<Session> Sessions { get; set; }

        /// <summary>
        /// This code is used by the participants to connect to the training occasion.
        /// </summary>
        /// <value></value>
        [DataMember] public string TypeableCode { get; set; }

        /// <summary>
        /// Described when the session is paused.
        /// </summary>
        /// <value></value>
        public DateTime? PauseStart { get; set; }

        /// <summary>
        /// A sum of the total time that the SessionGroup have been paused.
        /// </summary>
        /// <value></value>
        public TimeSpan PauseTimeSum { get; set; }

        /// <summary>
        /// Shortcut for the Scenarios Name
        /// </summary>
        /// <value></value>
        [DataMember] public string ScenarioName => Scenario.Name;

        /// <summary>
        /// A name set by the facilitator of the SessionGroup
        /// </summary>
        /// <value></value>
        [DataMember] public string GroupName { get; set; }

        /// <summary>
        /// Returns true if an active session has time left to run.
        /// </summary>
        /// <returns></returns>
        public bool TimeLeft()
        {
            return StartDate.HasValue && DateTime.Now <= StartDate.Value.Add(Duration).Add(PauseTimeSum);
        }
        
        public bool IsRunning()
        {
            return Status == SessionStatus.Running;
        }
        
        /// <summary>
        /// Creator and owner of SessionGroup
        /// </summary>
        public virtual Usr Usr { get; set; }
        
        public int? UsrId { get; set; }
    }
}