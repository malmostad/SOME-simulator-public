using System.Linq;
using SoMeSimulator.Data.Models;

namespace SoMeSimulator.Services.TimeCalc
{

    /// <summary>
    /// Time related calculations for phases.
    /// </summary>
    public class PhaseTimeCalc
    {
        private readonly Scenario _scenario;
        private SessionGroup _sessionGroup;
        private readonly SessionGroupTimeCalc _sessionGroupTimeCalc;

        public PhaseTimeCalc(Phase phase, SessionGroup sessionGroup)
        {
            Phase = phase;
            _scenario = phase.Scenario;
            _sessionGroup = sessionGroup;
            _sessionGroupTimeCalc = new SessionGroupTimeCalc(sessionGroup);
        }

        public Phase Phase { get; }

        /// <summary>
        /// At how many percent of the scenario total time does the phase start.
        /// </summary>
        /// <returns></returns>
        public double StartPercent()
        {
            return Phase.StartPercent;
        }

        /// <summary>
        /// At how many percent of the scenario total time does the phase end.
        /// </summary>
        /// <returns></returns>
        public double StopPercent()
        {
            var siblings = _scenario.Phases.OrderBy(p => p.StartPercent).ToList();

            var count = siblings.ToList().Count();

            var currentPhaseIndex = siblings.IndexOf(Phase);

            //Only one phase or at last phase
            if (count > 1 && currentPhaseIndex < count - 1)
                return siblings.ElementAt(currentPhaseIndex + 1).StartPercent;

            return 1;
        }

        /// <summary>
        /// How many percent of the sessions total time is used.
        /// </summary>
        /// <returns></returns>
        public double DurationInPercent()
        {
            return StopPercent() - StartPercent();
        }

        /// <summary>
        /// Calculates how many percent of the current Phase that has passed.
        /// </summary>
        /// <param name="totalPercentOfScenario"></param>
        /// <returns></returns>
        public double CurrentPercentOfPhase()
        {
            
            var currentTimePercent = _sessionGroupTimeCalc.CurrentSessionPercent();

            //All phases over
            if (currentTimePercent >= 1)
                return 1;

            // Phase has not started
            if (StartPercent() > currentTimePercent)
                return 0;

            //Ongoing phase
            return (currentTimePercent - StartPercent()) / DurationInPercent();

        }

    }
}