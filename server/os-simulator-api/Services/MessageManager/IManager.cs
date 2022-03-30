using System.Threading.Tasks;
using SoMeSimulator.Data.Models;

namespace SoMeSimulator.Services.MessageManager
{
    public interface IManager
    {
        Task Send(SessionGroup sessionGroup);
    }
}