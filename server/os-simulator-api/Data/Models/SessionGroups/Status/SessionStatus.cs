namespace SoMeSimulator.Data.Models.SessionGroups.Status
{
    public enum SessionStatus
    {
        /// <summary>
        /// Created. Have not yet been active.
        /// </summary>
        New = 1,
        /// <summary>
        /// Is running
        /// </summary>
        Running = 2,
        /// <summary>
        /// Paused by facilitator
        /// </summary>
        Paused = 3,
        /// <summary>
        /// Finished
        /// </summary>
        Finished = 4,
        /// <summary>
        /// Stopped by facilitator
        /// </summary>
        Cancelled = 5,
        
    }
}