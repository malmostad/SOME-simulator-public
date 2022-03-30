namespace SoMeSimulator.Services.MessageManager
{
    public interface IStressLevelCalculator
    {
        bool ShouldPost(uint stressLevel);
        bool ShouldComment(uint stressLevel);
    }
}