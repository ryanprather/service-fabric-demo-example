using BackfillWorker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackfillWorkerService.Logic
{
    public interface IBackfillWorkerLogicService
    {
        Task<IEnumerable<BackfillUpload>> GetBackfillSubjects();
        Task ProcessSubjectUploads(IEnumerable<BackfillUpload> subjectBackfillUploads);
    }
}
