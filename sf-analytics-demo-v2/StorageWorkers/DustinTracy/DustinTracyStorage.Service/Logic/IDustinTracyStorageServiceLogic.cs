using DustinTracyStorage.Models;
using System.Threading.Tasks;

namespace DustinTracyStorage.Service.Logic
{
    public interface IDustinTracyStorageServiceLogic
    {
        Task<int> InsertNewDustinTracySleepPeriods(DustinTracyStorageDto countsStorageDto);
    }
}
