using System;
using System.Threading.Tasks;
using AlgorithmStateWorker.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace AlgorithmStateWorker.Interface
{
    public interface IAlgorithmStateWorkerService : IService
    {
        Task StoreDustinTracySleepPeriodState(DustinTracyStateDto[] dustinTracyStateDto);
        Task StoreChoiWearPeriodState(ChoiStateDto[] dustinTracyStateDto);

    }
}
