namespace SoMeSimulator.Data.Models.Types
{
    public enum MessageType
    {
        /// <summary>
        /// Events
        /// </summary>
        ScenarioEvent = 1,

        /// <summary>
        /// A bot posts in root level
        /// </summary>
        Message = 2,

        /// <summary>
        /// The participant posts something
        /// </summary>
        Participant = 3,

        /// <summary>
        /// A bot reacts to a participants reply
        /// </summary>
        Comment = 4,

    }
}