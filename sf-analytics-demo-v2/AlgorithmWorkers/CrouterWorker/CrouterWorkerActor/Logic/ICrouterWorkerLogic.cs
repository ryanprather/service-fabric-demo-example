using EpochRetrieval.Models;
using System;
using System.Threading.Tasks;

namespace CrouterWorkerActor.Logic
{
    public interface ICrouterWorkerLogic
    {
        Task<int> ProcessCounts(EpochRecord[] epochRecords, long subjectId, string deviceId, Guid taskId, Guid settingsId);
    }
}
