using DustinTracyStorage.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace DustinTracyStorage.Interface
{
    public interface IDustinTracyStorageService : IService
    {
        Task EnqueueMessage(DustinTracyStorageDto countsStorageDto);
    }
}
