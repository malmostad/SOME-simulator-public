using System;
using SoMeSimulator.Data.Models.Abstract;
using SoMeSimulator.Data.Models.SessionGroups.Status;

namespace SoMeSimulator.Data.Models.SessionGroups.Commands
{
    public class Stop : ICommand<SessionGroup>
    {
        public void Execute(SessionGroup entity)
        {
            if (!Validate(entity))
                throw new Exception("Kan inte stoppa sessionen.");

            entity.StopDate = DateTime.Now;
            entity.StateMachine.Fire(SessionTriggers.Stop);
        }

        public bool Validate(SessionGroup entity)
        {
            if (!entity.StateMachine.CanFire(SessionTriggers.Stop))
                return false;
            return true;
        }
    }
}
