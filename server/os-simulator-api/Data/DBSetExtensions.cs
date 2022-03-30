using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionGroups.Status;

namespace SoMeSimulator.Data
{
    public static class DBSetExtensions
    {
        /// <summary>
        /// Find a Session by Guid
        /// </summary>
        /// <param name="sessions"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Session FindByGuid(this DbSet<Session> sessions, Guid guid)
        {
            return sessions.SingleOrDefault(s => s.SessionGuid == guid);
        }
    }
}