using AnalyticsJobsService.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace AnalyticsJobsService.Interface
{
    public interface IAnalyticsJobsService: IService
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
        Task<SubjectDeviceUploadEntity[]> GetFollowingUploadsFromUploadAsync(SubjectDeviceUploadEntity subjectUploadEntity);
        #endregion

        #region Analytics Settings
        Task<bool> AlgorithmSettingExistsForStudyAsync(long studyId);
        Task<AlgorithmSettingEntity[]> GetAlgorithmSettingForStudyAsync(long studyId);
        #endregion

        #region Upload Processing Job
        Task CreateNewUploadProcessingJob(Guid subjectDeviceUploadId);
        Task<UploadProcessingJobEntity> GetSubjectUploadNotStartedJob(Guid subjectDeviceUploadId);
        Task<UploadProcessingJobEntity> GetUploadProcessingJob(Guid uploadProcessingJobId);
        Task SetProcessingJobStartedDateTime(DateTime startedDateTime, Guid jobId);
        Task SetProcessingJobCompletedDateTime(DateTime completedDateTime, Guid jobId);
        #endregion

        #region Algorithm Tasks
        Task<AlgorithmTaskDto[]> GetAlgorithmTaskDtos(Guid uploadProcessingJobId);
        Task<AlgorithmTaskEntity> CreateNewAlgorithmTaskEntityAsync(Guid uploadId, Guid analtyicsSettingsId, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc);
        Task UpdateAlgorithmTaskStarted(Guid taskId, DateTime processingStartedUtc);
        Task UpdateAlgorithmTaskCompleted(Guid taskId, DateTime processingCompletedUtc, int itemsComputed);
        Task UpdateAlgorithmTaskErrored(Guid taskId, DateTime processingCompletedUtc, string errorMessage);

        Task UpdateAlgorithmTaskProcessingComplete(Guid taskId, DateTime processingStartedUtc, DateTime processingEndedUtc, int itemsComputed, string errorMessage);
        Task UpdateAlgorithmTaskStorageItemsComplete(Guid taskId, DateTime StartedUtc, DateTime CompletedUtc, int itemsComputed, string errorMessage);
        #endregion

        #region Processing Job Epoch Retrieval
        Task CreateEpochRetrievalForProcessingJob(Guid uploadProcessingJobId, DateTime processingStartedUtc, DateTime processingCompletedDateTimeUtc, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc, string errorReason);
        #endregion
    }
}
