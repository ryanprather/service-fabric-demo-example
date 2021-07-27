using EpochRetrieval.Models;
using System;
using System.Threading.Tasks;

namespace CountsWorkerActor.Logic
{
    public interface ICountsWorkerLogic
    {
        Task<int> ProcessCounts(EpochRecord[] epochRecords, long subjectId, string deviceId, Guid taskId, Guid settingsId);
    }
}
