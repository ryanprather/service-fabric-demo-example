using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnalyticsJobsService.Interface;
using AnalyticsJobsService.Logic;
using AnalyticsJobsService.Models;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace AnalyticsJobsService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class AnalyticsJobsService : StatelessService, IAnalyticsJobsService
    {
        private readonly IAnalyticsJobsDataRepository _analyticsJobsDataRepository;
        public AnalyticsJobsService(StatelessServiceContext context, IAnalyticsJobsDataRepository analyticsJobsDataRepository)
            : base(context)
        {
            _analyticsJobsDataRepository = analyticsJobsDataRepository;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        public async Task<bool> AlgorithmSettingExistsForStudyAsync(long studyId)
        {
            return await _analyticsJobsDataRepository.AlgorithmSettingExistsForStudyAsync(studyId);
        }

        public async Task CreateNewStudySubjectAsync(StudySubjectEntity subject)
        {
            await _analyticsJobsDataRepository.CreateNewStudySubjectAsync(subject);
        }

        public async Task CreateNewSubjectDeviceAsync(SubjectDeviceEntity subjectDeviceEntity)
        {
            await _analyticsJobsDataRepository.CreateNewSubjectDeviceAsync(subjectDeviceEntity);
        }

        public async Task CreateNewSubjectUploadAsync(SubjectDeviceUploadEntity subjectUploadEntity)
        {
            await _analyticsJobsDataRepository.CreateNewSubjectUploadAsync(subjectUploadEntity);
        }

        public async Task CreateNewUploadProcessingJob(Guid subjectDeviceUploadId)
        {
            await _analyticsJobsDataRepository.CreateNewUploadProcessingJob(subjectDeviceUploadId);
        }

        public async Task<AlgorithmSettingEntity[]> GetAlgorithmSettingForStudyAsync(long studyId)
        {
            return (await _analyticsJobsDataRepository.GetAlgorithmSettingForStudyAsync(studyId)).ToArray();
        }

        public async Task<SubjectDeviceUploadEntity[]> GetFollowingUploadsFromUploadAsync(SubjectDeviceUploadEntity subjectUploadEntity)
        {
            return (await _analyticsJobsDataRepository.GetFollowingUploadsFromUploadAsync(subjectUploadEntity)).ToArray();
        }

        public async Task<StudySubjectEntity> GetStudySubjectEntityAsync(long studyId, long subjectId)
        {
            return await _analyticsJobsDataRepository.GetStudySubjectEntityAsync(studyId, subjectId);
        }

        public async Task<SubjectDeviceEntity> GetSubjectDeviceAsync(Guid studySubjectId, string deviceSerial)
        {
            return await _analyticsJobsDataRepository.GetSubjectDeviceAsync(studySubjectId, deviceSerial);
        }

        public async Task<SubjectDeviceUploadEntity> GetSubjectUploadAsync(Guid subjectDeviceId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {
            return await _analyticsJobsDataRepository.GetSubjectUploadAsync(subjectDeviceId, beginTimestampUtc, endTimestampUtc);
        }

        public async Task<UploadProcessingJobEntity> GetSubjectUploadNotStartedJob(Guid subjectDeviceUploadId)
        {
            return await _analyticsJobsDataRepository.GetSubjectUploadNotStartedJob(subjectDeviceUploadId);
        }

        public async Task<SubjectUploadDto> GetSubjectUploadWithDevice(Guid subjectDeviceUploadId)
        {
            return await _analyticsJobsDataRepository.GetSubjectUploadWithDevice(subjectDeviceUploadId);
        }

        public async Task<UploadProcessingJobEntity> GetUploadProcessingJob(Guid uploadProcessingJobId)
        {
            return await _analyticsJobsDataRepository.GetUploadProcessingJob(uploadProcessingJobId);
        }

        public async Task<bool> StudySubjectExistsAsync(long studyId, long subjectId)
        {
            return await _analyticsJobsDataRepository.StudySubjectExistsAsync(studyId, subjectId);
        }

        public async Task<bool> SubjectDeviceExistsAsync(Guid studySubjectId, string deviceSerial)
        {
            return await _analyticsJobsDataRepository.SubjectDeviceExistsAsync(studySubjectId, deviceSerial);
        }

        public async Task<bool> SubjectUploadExistsAsync(Guid subjectDeviceId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {
            return await _analyticsJobsDataRepository.SubjectUploadExistsAsync(subjectDeviceId, beginTimestampUtc, endTimestampUtc);
        }

        public async Task<bool> SubjectUploadTimeRangeMatchesAsync(Guid subjectDeviceUploadId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {
            return await _analyticsJobsDataRepository.SubjectUploadTimeRangeMatchesAsync(subjectDeviceUploadId, beginTimestampUtc, endTimestampUtc);
        }

        public async Task UpdateSubjectUploadTimeRangeAsync(Guid uploadId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {
            await _analyticsJobsDataRepository.UpdateSubjectUploadTimeRangeAsync(uploadId, beginTimestampUtc, endTimestampUtc);
        }

        #region Algorithm Tasks

        public async Task<AlgorithmTaskEntity> CreateNewAlgorithmTaskEntityAsync(Guid uploadId, Guid analtyicsSettingsId, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc)
        {
            return await _analyticsJobsDataRepository.CreateNewAlgorithmTaskEntityAsync(uploadId, analtyicsSettingsId, adjustedBeginTimestampUtc, adjustedEndTimestampUtc);
        }

        public async Task<AlgorithmTaskDto[]> GetAlgorithmTaskDtos(Guid uploadProcessingJobId) 
        {
            return (await _analyticsJobsDataRepository.GetAlgorithmTaskDtos(uploadProcessingJobId)).ToArray();
        }

        public async Task UpdateAlgorithmTaskStarted(Guid taskId, DateTime processingStartedUtc) 
        {
            await _analyticsJobsDataRepository.UpdateAlgorithmTaskStarted(taskId, processingStartedUtc);
        }

        public async Task UpdateAlgorithmTaskCompleted(Guid taskId, DateTime processingCompletedUtc, int itemsComputed) 
        {
            await _analyticsJobsDataRepository.UpdateAlgorithmTaskCompleted(taskId, processingCompletedUtc, itemsComputed);
        }

        public async Task UpdateAlgorithmTaskErrored(Guid taskId, DateTime processingCompletedUtc, string errorMessage) 
        {
            await _analyticsJobsDataRepository.UpdateAlgorithmTaskErrored(taskId, processingCompletedUtc, errorMessage);
        }

        public async Task UpdateAlgorithmTaskProcessingComplete(Guid taskId, DateTime processingStartedUtc, DateTime processingEndedUtc, int itemsComputed, string errorMessage) 
        {
            await _analyticsJobsDataRepository.UpdateAlgorithmTaskProcessingComplete(taskId, processingStartedUtc, processingEndedUtc, itemsComputed, errorMessage);
        }

        public async Task UpdateAlgorithmTaskStorageItemsComplete(Guid taskId, DateTime StartedUtc,DateTime CompletedUtc, int itemsComputed, string errorMessage) 
        {
            await _analyticsJobsDataRepository.UpdateAlgorithmTaskStorageItemsComplete(taskId, StartedUtc, CompletedUtc, itemsComputed, errorMessage);
        }

        #endregion

        #region Upload Processing Job

        public async Task SetProcessingJobStartedDateTime(DateTime startedDateTime, Guid jobId) 
        {
            await _analyticsJobsDataRepository.SetProcessingJobStartedDateTime(startedDateTime, jobId);
        }

        public async Task SetProcessingJobCompletedDateTime(DateTime completedDateTime, Guid jobId) 
        {
            await _analyticsJobsDataRepository.SetProcessingJobCompletedDateTime(completedDateTime, jobId);
        }

        #endregion

        #region Processing Job Epoch Retrieval
        public async Task CreateEpochRetrievalForProcessingJob(Guid uploadProcessingJobId, DateTime processingStartedUtc, DateTime processingCompletedDateTimeUtc, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc, string errorReason) 
        {
            await _analyticsJobsDataRepository.CreateEpochRetrievalForProcessingJob(uploadProcessingJobId, processingStartedUtc, processingCompletedDateTimeUtc, adjustedBeginTimestampUtc, adjustedEndTimestampUtc, errorReason);
        }
        #endregion

    }
}
