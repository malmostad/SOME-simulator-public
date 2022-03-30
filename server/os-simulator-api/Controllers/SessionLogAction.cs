using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionLogs;

namespace SomeSimulator.Controllers
{
    public class SessionLogAction
    {
        public int SessionLogId { get; set; }
        public BotReplyProperties BotReplyProperties { get; set; }
        public SessionLogTag SessionLogTag { get; set; }
    }
}