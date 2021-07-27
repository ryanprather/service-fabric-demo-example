using ChoiStorage.Models;
using System.Threading.Tasks;

namespace ChoiStorage.Service.Logic
{
    public interface IChioStorageServiceLogic
    {
        Task<int> InsertNewChoiWearPeriods(ChoiStorageDto choiStorageDto);
    }
}
