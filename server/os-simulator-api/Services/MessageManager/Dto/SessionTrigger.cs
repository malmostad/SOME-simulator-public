using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SoMeSimulator.Data.Models.SessionGroups.Status;

namespace SoMeSimulator.Services.MessageManager.Dto
{
    public class SessionTrigger
    {
        public SessionTrigger(SessionStatus status)
        {
            Status = status;
        }

        public SessionTrigger(Dialog dialog, SessionStatus status)
        {
            Dialog = dialog;
            Status = status;
        }

        public Dialog Dialog { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SessionStatus Status { get; set; }
    }
}