using AnalyticsJobsService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsJobsService.Logic
{
    public interface IAnalyticsJobsDataRepository
    {
        #region Study Subject functions 
        Task<bool> StudySubjectExistsAsync(long studyId, long subjectId);
        Task CreateNewStudySubjectAsync(StudySubjectEntity subject);
        Task<StudySubjectEntity> GetStudySubjectEntityAsync(long studyId, long subjectId);
        #endregion

        #region Subject Device functions
        Task<bool> SubjectDeviceExistsAsync(Guid studySubjectId, string deviceSerial);
        Task CreateNewSubjectDeviceAsync(SubjectDeviceEntity subjectDeviceEntity);
        Task<SubjectDeviceEntity> GetSubjectDeviceAsync(Guid studySubjectId, string deviceSerial);
        #endregion

        #region Subject Upload functions
        Task<bool> SubjectUploadExistsAsync(Guid subjectDeviceId, DateTime beginTimestampUtc, DateTime endTimestampUtc);
        Task CreateNewSubjectUploadAsync(SubjectDeviceUploadEntity subjectUploadEntity);
        Task<bool> SubjectUploadTimeRangeMatchesAsync(Guid subjectDeviceUploadId, DateTime beginTimestampUtc, DateTime endTimestampUtc);
        Task UpdateSubjectUploadTimeRangeAsync(Guid uploadId, DateTime beginTimestampUtc, DateTime endTimestampUtc);
        Task<SubjectDeviceUploadEntity> GetSubjectUploadAsync(Guid subjectDeviceId, DateTime beginTimestampUtc, DateTime endTimestampUtc);
        Task<SubjectUploadDto> GetSubjectUploadWithDevice(Guid subjectDeviceUploadId);
        Task<List<SubjectDeviceUploadEntity>> GetFollowingUploadsFromUploadAsync(SubjectDeviceUploadEntity subjectUploadEntity);
        #endregion

        #region Analytics Settings
        Task<bool> AlgorithmSettingExistsForStudyAsync(long studyId);
        Task<IEnumerable<AlgorithmSettingEntity>> GetAlgorithmSettingForStudyAsync(long studyId);
        #endregion

        #region Upload Processing Job
        Task CreateNewUploadProcessingJob(Guid subjectDeviceUploadId);
        Task<UploadProcessingJobEntity> GetSubjectUploadNotStartedJob(Guid subjectDeviceUploadId);
        Task<UploadProcessingJobEntity> GetUploadProcessingJob(Guid uploadProcessingJobId);
        Task SetProcessingJobStartedDateTime(DateTime startedDateTime, Guid JobId);
        Task SetProcessingJobCompletedDateTime(DateTime completedDateTime, Guid jobId);
        #endregion

        #region Algorithm Tasks
        Task<AlgorithmTaskEntity> CreateNewAlgorithmTaskEntityAsync(Guid uploadProcessingJobId, Guid analtyicsSettingsId, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc);
        Task<IEnumerable<AlgorithmTaskDto>> GetAlgorithmTaskDtos(Guid uploadProcessingJobId);
        Task UpdateAlgorithmTaskStarted(Guid taskId, DateTime processingStartedUtc);
        Task UpdateAlgorithmTaskCompleted(Guid taskId, DateTime processingCompletedUtc, int itemsComputed);
        Task UpdateAlgorithmTaskErrored(Guid taskId, DateTime processingCompletedUtc, string errorMessage);

        Task UpdateAlgorithmTaskProcessingComplete(Guid taskId, DateTime processingStartedUtc, DateTime processingEndedUtc, int itemsComputed, string errorMessage);
        Task UpdateAlgorithmTaskStorageItemsComplete(Guid taskId, DateTime storageStartedUtc, DateTime storageCompletedUtc, int itemsProcessedByStorage, string errorMessage);
        

        
        #endregion

        #region Processing Job Epoch Retrieval
        Task CreateEpochRetrievalForProcessingJob(Guid uploadProcessingJobId, DateTime processingStartedUtc, DateTime processingCompletedDateTimeUtc, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc, string errorReason);
        #endregion
    }
}
