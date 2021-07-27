using AnalyticsJobsService.Interface;
using AnalyticsJobsService.Models;
using SubjectModels;
using System.Threading.Tasks;

namespace SubjectActor.Logic
{
    public class SubjectActorLogic: ISubjectActorLogic
    {
        private readonly IAnalyticsJobsService _analyticsJobsService;
        public SubjectActorLogic(IAnalyticsJobsService analyticsJobsService) 
        {
            _analyticsJobsService = analyticsJobsService;
        } 

        public async Task<StudySubjectEntity> CreateSubjectIfNotExists(SubjectMdo subjectMdo)
        {
            if (!await _analyticsJobsService.StudySubjectExistsAsync(subjectMdo.StudyId, subjectMdo.SubjectId))
            {
                var subjectEntity = new StudySubjectEntity()
                {
                    StudyId = subjectMdo.StudyId,
                    SubjectId = subjectMdo.SubjectId
                };
                await _analyticsJobsService.CreateNewStudySubjectAsync(subjectEntity);
            }

            return await _analyticsJobsService.GetStudySubjectEntityAsync(subjectMdo.StudyId, subjectMdo.SubjectId);
        }

        public async Task<SubjectDeviceEntity> CreateSubjectDeviceIfNotExist(StudySubjectEntity studySubjectEntity, SubjectMdo subjectMdo)
        {
            if (!await _analyticsJobsService.SubjectDeviceExistsAsync(studySubjectEntity.Id, subjectMdo.DeviceSerial))
            {
                var subjectDeviceEntity = new SubjectDeviceEntity()
                {
                    StudySubjectId = studySubjectEntity.Id,
                    DeviceSerial = subjectMdo.DeviceSerial
                };
                await _analyticsJobsService.CreateNewSubjectDeviceAsync(subjectDeviceEntity);
            }

            return await _analyticsJobsService.GetSubjectDeviceAsync(studySubjectEntity.Id, subjectMdo.DeviceSerial);
        }

        public async Task<SubjectDeviceUploadEntity> CreateSubjectUploadIfNotExists(SubjectDeviceEntity subjectDeviceEntity, SubjectUploadMdo subjectUploadMdo)
        {
            // create new  upload if not exists // 
            if (!await _analyticsJobsService.SubjectUploadExistsAsync(subjectDeviceEntity.Id, subjectUploadMdo.BeginTimestampUtc, subjectUploadMdo.EndTimestampUtc))
            {
                var subjectUploadEntity = new SubjectDeviceUploadEntity()
                {
                    SubjectDeviceId = subjectDeviceEntity.Id,
                    BeginTimestampUtc = subjectUploadMdo.BeginTimestampUtc,
                    EndTimestampUtc = subjectUploadMdo.EndTimestampUtc
                };
                await _analyticsJobsService.CreateNewSubjectUploadAsync(subjectUploadEntity);
            }
            var subjectDeviceUpload = await _analyticsJobsService.GetSubjectUploadAsync(subjectDeviceEntity.Id, subjectUploadMdo.BeginTimestampUtc, subjectUploadMdo.EndTimestampUtc);

            // update subject device upload if needed //
            if (!await _analyticsJobsService.SubjectUploadTimeRangeMatchesAsync(subjectDeviceUpload.Id, subjectUploadMdo.BeginTimestampUtc, subjectUploadMdo.EndTimestampUtc))
            {
                await _analyticsJobsService.UpdateSubjectUploadTimeRangeAsync(subjectDeviceUpload.Id, subjectUploadMdo.BeginTimestampUtc, subjectUploadMdo.EndTimestampUtc);
                subjectDeviceUpload = await _analyticsJobsService.GetSubjectUploadAsync(subjectDeviceEntity.Id, subjectUploadMdo.BeginTimestampUtc, subjectUploadMdo.EndTimestampUtc);
            }

            return subjectDeviceUpload;
        }
    }
}
