using AlgorithmStateWorker.Models;
using System.Threading.Tasks;

namespace AlgorithmStateWorkerService.Logic
{
    public interface IAlgorithmStateServiceLogic
    {
        Task StoreDustinTracyStates(DustinTracyStateDto[] dustinTracyStates);
        Task StoreChoiStates(ChoiStateDto[] choiStates);
    }
}
