using CountsStorage.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CountsStorage.Service.Logic
{
    public interface ICountsStorageServiceLogic
    {
        Task<int> InsertNewCounts(CountsStorageDto countsStorageDto);
    }
}
