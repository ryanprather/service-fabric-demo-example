using AnalyticsJobsService.Models;
using SubjectModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubjectActor.Logic
{
    public interface ISubjectActorLogic
    {

        Task<StudySubjectEntity> CreateSubjectIfNotExists(SubjectMdo subjectMdo);
        Task<SubjectDeviceEntity> CreateSubjectDeviceIfNotExist(StudySubjectEntity studySubjectEntity, SubjectMdo subjectMdo);
        Task<SubjectDeviceUploadEntity> CreateSubjectUploadIfNotExists(SubjectDeviceEntity subjectDeviceEntity, SubjectUploadMdo subjectUploadMdo);

    }
}
