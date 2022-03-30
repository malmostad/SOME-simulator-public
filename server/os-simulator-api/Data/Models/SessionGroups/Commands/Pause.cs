using System;
using System.Linq;
using Bogus.DataSets;
using SoMeSimulator.Data.Models.Abstract;
using SoMeSimulator.Data.Models.SessionGroups.Status;

namespace SoMeSimulator.Data.Models.SessionGroups.Commands
{
    public class Pause: ICommand<SessionGroup>
    {
        public void Execute(SessionGroup entity)
        {
            if (!Validate(entity))
                throw new Exception("Kan inte pausa sessionen.");

            entity.PauseStart = DateTime.Now;
            entity.StateMachine.Fire(SessionTriggers.Pause);
        }

        public bool Validate(SessionGroup entity)
        {
            if (!entity.StateMachine.CanFire(SessionTriggers.Pause))
                return false;

            return true;

        }
    }

    
}
