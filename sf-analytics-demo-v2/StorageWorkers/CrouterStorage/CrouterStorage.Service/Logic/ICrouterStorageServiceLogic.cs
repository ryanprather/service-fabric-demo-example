using CrouterStorage.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrouterStorage.Service.Logic
{
    public interface ICrouterStorageServiceLogic
    {
        Task<int> InsertNewCrouterCutpoints(CrouterStorageDto countsStorageDto);
    }
}
