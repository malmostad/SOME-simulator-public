using System;
using System.Linq;
using SoMeSimulator.Data.Models.Abstract;
using SoMeSimulator.Data.Models.SessionGroups.Status;

namespace SoMeSimulator.Data.Models.SessionGroups.Commands
{
    public class Start: ICommand<SessionGroup>
    {
        public void Execute(SessionGroup entity)
        {
            if (!Validate(entity))
                throw new Exception("Kan inte aktivera sessionen.");

            entity.StartDate = DateTime.Now;
            entity.StateMachine.Fire(SessionTriggers.Start);
            entity.CurrentPhase = entity.Scenario.Phases.OrderBy(p => p.StartPercent).First();
        }

        public bool Validate(SessionGroup entity)
        {
            if (!entity.Sessions.Any())
                return false;

            if (!entity.StateMachine.CanFire(SessionTriggers.Start))
                return false;

            if (entity.Scenario == null)
                return false;

            return true;

        }
    }
}
