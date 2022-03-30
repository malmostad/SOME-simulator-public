using System.ComponentModel;
using System;
using System.Xml;
using System.Data.SqlTypes;
using SoMeSimulator.Data;
using System.Linq;
using SoMeSimulator.Data.Models.SessionGroups.Status;
using SoMeSimulator.Data.Models.Defaults;
using Serilog;
using System.Threading.Tasks;

namespace SoMeSimulator.Services 
{
    public class Remove : IRemove
    {
        private readonly SoMeContext _dbContext;

        public Remove(SoMeContext dbContext)
        {
            this._dbContext = dbContext;
        }
        /// <summary>
        /// Removes old sessions and related data
        /// </summary>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        public async Task RunAsync(bool dryRun = false)
        {
            var limit = DateTime.Now.AddDays(-DefaultValues.RemoveAfterDays);

            var sessionGroupsToDelete = _dbContext.SessionGroups.ToList().Where(s => s.StopDate <= limit ).ToList();
            var sessionsToDelete = sessionGroupsToDelete.SelectMany(sg => sg.Sessions).ToList();
            var sessionLogsToDelete = sessionsToDelete.SelectMany(s => s.SessionLogs).ToList();

            Log.Debug($"Deleting {sessionGroupsToDelete.Count()} SessionGroups, {sessionsToDelete.Count()} sessions and {sessionLogsToDelete.Count()} SessionLogs");

            if(!dryRun)  {
                _dbContext.SessionGroups.RemoveRange(sessionGroupsToDelete);
                _dbContext.Sessions.RemoveRange(sessionsToDelete);
                _dbContext.SessionLogs.RemoveRange(sessionLogsToDelete);
            
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}