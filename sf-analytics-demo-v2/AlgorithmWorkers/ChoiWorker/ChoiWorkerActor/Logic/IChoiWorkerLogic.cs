using EpochRetrieval.Models;
using System;
using System.Threading.Tasks;

namespace ChoiWorkerActor.Logic
{
    public interface IChoiWorkerLogic
    {
        Task<int> ProcessRecords(EpochRecord[] epochRecords, long subjectId, string deviceId, string settings, Guid taskId, Guid settingsId);
    }
}
