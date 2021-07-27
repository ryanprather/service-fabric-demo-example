using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace BackfillWorker.Interface
{
    public interface IBackfillWorkerService : IService
    {
        Task CreateBackfill();
    }
}
