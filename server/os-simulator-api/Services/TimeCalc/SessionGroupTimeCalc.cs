using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SoMeSimulator.Data.Models;

namespace SoMeSimulator.Services.TimeCalc
{
    public class SessionGroupTimeCalc
    {
        private readonly SessionGroup _sessionGroup;
        private Scenario _scenario;

        public SessionGroupTimeCalc(SessionGroup sessionGroup)
        {
            _sessionGroup = sessionGroup;
            _scenario = sessionGroup.Scenario;
        }

        /// <summary>
        /// Session duration.
        /// </summary>
        /// <returns></returns>
        public TimeSpan SessionRunTime()
        {
            return DateTime.Now
                .Subtract(_sessionGroup.StartDate.Value)
                .Subtract(_sessionGroup.PauseTimeSum);
        }

        /// <summary>
        /// Percent of session.
        /// </summary>
        /// <returns></returns>
        public double CurrentSessionPercent()
        {
            var percent = SessionRunTime() / _sessionGroup.Duration;

            return percent > 1? 1: percent;
        }

        /// <summary>
        /// Returns a PhaseTimeCalc for the session that should be the current session calculated from running time.
        /// </summary>
        /// <returns></returns>
        public PhaseTimeCalc CurrentPhaseTimeCalc()
        {
            
            Phase phase = CurrentPhase();

            return new PhaseTimeCalc(phase, _sessionGroup);
        }

        /// <summary>
        /// Fetch current phase
        /// </summary>
        /// <returns></returns>
        public Phase CurrentPhase()
        {
            Phase phase;
            var currentTimePercent = CurrentSessionPercent();

            if (currentTimePercent < 1)
            {
                //Passed stop tipe
                phase = _scenario.Phases.OrderBy(p => p.StartPercent).Last(p => p.StartPercent <= currentTimePercent);
            }
            else
            {
                //Running
                phase = _scenario.Phases.Last();
            }

            return phase;
        }
    }
}
