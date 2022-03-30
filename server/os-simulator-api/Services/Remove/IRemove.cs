using System.Threading.Tasks;

namespace SoMeSimulator.Services {
    public interface IRemove {
        Task RunAsync(bool dryRun = false);
    }
}
