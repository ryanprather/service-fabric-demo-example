using AnalyticsJobsService.Interface;
using AnalyticsJobsService.Models;
using Global.Services;
using JobModels;
using System.Threading.Tasks;

namespace JobsActorExternalService
{
    public class ExternalJobsService: IExternalJobsService
    {
        private readonly IAnalyticsJobsService _analyticsJobsService;
        public ExternalJobsService() 
        {
            _analyticsJobsService = FabricServices.GetAnalyticsJobsService();
        }

        public async Task InitNewJob(IJobDto jobDto) 
        {
            if (await _analyticsJobsService.AlgorithmSettingExistsForStudyAsync(jobDto.StudyId))
            {
                // create job // 
                var uploadProcessingJobEntity = await CreateAndGetNewJobEntity(jobDto);
                var jobActor = FabricServices.GetJobActor(jobDto.SubjectId);
                await jobActor.InitJobActor(jobDto.StudyId, jobDto.SubjectId, jobDto.UploadId, uploadProcessingJobEntity.Id, jobDto.BeginTimestampUtc, jobDto.EndTimestampUtc);
            }
        }


        private async Task<UploadProcessingJobEntity> CreateAndGetNewJobEntity(IJobDto jobDto)
        {
            await _analyticsJobsService.CreateNewUploadProcessingJob(jobDto.UploadId);
            return await _analyticsJobsService.GetSubjectUploadNotStartedJob(jobDto.UploadId);
        }
    }
}
