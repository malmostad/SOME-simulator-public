using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Services.SignalR;
using SoMeSimulator.Services.TimeCalc;

namespace SoMeSimulator.Services.MessageManager
{
    public class ProgressManager: IManager
    {
        private readonly ISendMessage _sendMessage;

        public ProgressManager(ISendMessage sendMessage)
        {
            _sendMessage = sendMessage;
        }

        public Task Send(SessionGroup sessionGroup)
        {
            var timeCalc = new SessionGroupTimeCalc(sessionGroup);

            var currentSessionPercent = timeCalc.CurrentSessionPercent();

            return _sendMessage.SendCurrentSessionProgressAsync(currentSessionPercent, sessionGroup);

        }
    }
}
