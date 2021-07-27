using CountsStorage.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace CountsStorage.Interface
{
    public interface ICountsStorageService : IService
    {
        Task EnqueueMessage(CountsStorageDto countsStorageDto);
    }
}
