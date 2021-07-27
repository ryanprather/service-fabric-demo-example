using ChoiStorage.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace ChoiStorage.Interface
{
    public interface IChoiStorageService : IService
    {
        Task EnqueueMessage(ChoiStorageDto countsStorageDto);
    }
}
