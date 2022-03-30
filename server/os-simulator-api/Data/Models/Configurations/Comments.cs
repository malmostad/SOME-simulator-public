namespace SomeSimulator.Data.Models.Configurations
{
    public class CommentsSettings
    {
        public  int CommentsPerPost { get; set; }
        public TimeWindow TimeWindow { get; set; }
    }

    public class TimeWindow
    {
        public int Start { get; set; }
        public int Stop { get; set; }
    }
    
}