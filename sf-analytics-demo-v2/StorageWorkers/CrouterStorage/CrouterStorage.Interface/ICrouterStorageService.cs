using CrouterStorage.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace CrouterStorage.Interface
{
    public interface ICrouterStorageService : IService
    {
        Task EnqueueMessage(CrouterStorageDto countsStorageDto);
    }
}
