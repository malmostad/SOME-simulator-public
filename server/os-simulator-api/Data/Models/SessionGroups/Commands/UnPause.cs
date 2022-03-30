namespace SoMeSimulator.Data.Models.SessionGroups.Commands
{
    using System;
    using System.Linq;
    using SoMeSimulator.Data.Models.Abstract;
    using SoMeSimulator.Data.Models.SessionGroups.Status;

    public class UnPause : ICommand<SessionGroup>
    {
        public void Execute(SessionGroup entity)
        {
            if (!Validate(entity))
                throw new Exception("Kan inte pausa sessionen.");

            TimeSpan duration = DateTime.Now - entity.PauseStart.Value;
            entity.PauseStart = null;
            entity.PauseTimeSum = Duration(entity, duration);

            entity.StateMachine.Fire(SessionTriggers.UnPause);
        }

        private static TimeSpan Duration(SessionGroup entity, TimeSpan duration)
        {
            return entity.PauseTimeSum != null ? entity.PauseTimeSum.Add(duration) : duration;
        }

        public bool Validate(SessionGroup entity)
        {
            if (!entity.PauseStart.HasValue)
                return false;

            if (!entity.StateMachine.CanFire(SessionTriggers.UnPause))
                return false;

            return true;

        }
    }
}
