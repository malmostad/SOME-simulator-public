namespace SomeSimulator.Data.Models.Configurations {
    
    
    public class StressLevel {

        /// <summary>
        /// Max level of stress. Low values => more stress.
        /// </summary>
        public double MaxLevel { get; set; }
        /// <summary>
        /// Describes hos much you can slow down the frequency of new messages. 
        /// </summary>
        public double Modifier { get; set; }
    }
}
