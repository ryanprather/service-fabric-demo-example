using EpochRetrieval.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EpochRetrievalActor.EpochSqlService
{
    public interface IEpochSqlService
    {
        Task<IEnumerable<EpochRecord>> GetEpochsAsync(long subjectId, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc);
    }
}
