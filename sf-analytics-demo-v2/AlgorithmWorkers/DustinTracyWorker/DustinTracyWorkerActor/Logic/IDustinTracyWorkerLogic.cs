using EpochRetrieval.Models;
using System;
using System.Threading.Tasks;

namespace DustinTracyWorkerActor.Logic
{
    public interface IDustinTracyWorkerLogic
    {
        Task<int> ProcessRecords(EpochRecord[] epochRecords, long subjectId, string deviceId, string settings, Guid taskId, Guid settingsId);
    }
}
