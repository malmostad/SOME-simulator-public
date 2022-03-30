using System;
using SoMeSimulator.Data.Models.Abstract;
using SoMeSimulator.Data.Models.SessionGroups.Status;

namespace SoMeSimulator.Data.Models.SessionGroups.Commands
{
    public class Uncouple : ICommand<SessionGroup>
    {
        public void Execute(SessionGroup entity)
        {
            if (!Validate(entity))
                throw new Exception("Användaren och träningstillfället kan inte frånkopplas.");

            entity.Usr.ActiveSessionGroup = null;
        }

        public bool Validate(SessionGroup entity)
        {
            if (!entity.CanDecouple())
                return false;
            
            return true;
        }
    }
}
