using System;
using SoMeSimulator.Data.Models.Abstract;
using SoMeSimulator.Data.Models.SessionGroups.Status;

namespace SoMeSimulator.Data.Models.SessionGroups.Commands
{
    public class Cancel : ICommand<SessionGroup>
    {
        public void Execute(SessionGroup entity)
        {
            if (!Validate(entity))
                throw new Exception("Kan inte avbryta sessionen.");

            entity.StopDate = DateTime.Now;
            entity.StateMachine.Fire(SessionTriggers.Cancel);
        }

        public bool Validate(SessionGroup entity)
        {
            if (!entity.StateMachine.CanFire(SessionTriggers.Cancel))
                return false;
            return true;
        }
    }
}