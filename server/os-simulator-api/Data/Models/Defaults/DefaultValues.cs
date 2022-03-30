using System;

namespace SoMeSimulator.Data.Models.Defaults
{
    public static class 
        DefaultValues
    {
        /// <summary>
        /// Default timespan for session length
        /// </summary>
        /// <value></value>
        public static TimeSpan DefaultSessionRunTime => new TimeSpan(0, 20, 0);

        /// <summary>
        /// Locale used by Bogus.
        /// </summary>
        /// <value></value>
        public static string FakeLocale => "sv";

        /// <summary>
        /// Chance that a comment is posted.
        /// </summary>
        /// <value></value>
        public static int CommentsChance => 15;

        /// <summary>
        /// How much the stress level
        /// affects the chance that a
        /// comment is posted.
        /// </summary>
        /// <value></value>
        public static int CommentsChanceSwing => 6;

        public static int SaltLength => 10;

        /// <summary>
        /// Number of hashrounds. Only change before the users are created.
        /// </summary>
        public static int HashRounds => 1000;

        /// <summary>
        /// How long can a session remain in the database.
        /// </summary>
        public static int RemoveAfterDays => 4;

        /// <summary>
        /// This setting effects the interval of the timer that determines when messages are sent. 
        /// </summary>
        /// <value></value>
        public static int TimerIntervalMilliseconds => 2000;

        /// <summary>
        /// Enables the timer to slow down when there is no active sessions. Slows down even more at night.
        /// </summary>
        public static bool TimerIntervalSlowDown => true;

        /// <summary>
        /// How long time the system waits to get a lock when sending ScenarioEvents.
        /// </summary>
        public static int ScenarioEventTimeoutLock => 5000;
    }
}