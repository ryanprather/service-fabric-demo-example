using Analytics.Api.Models;
using Global.Services;
using SubjectModels;
using System.Threading.Tasks;

namespace Analytics.Api.Service.Upload
{
    public class UploadService: IUploadService
    {

        public async Task ProcessUpload(UploadMetadataDto uploadMetadataDto) 
        {
            //map model //
            var dtoModel = MapDtoSubjectMdo(uploadMetadataDto);
            var subjectActor = FabricServices.GetSubjectActor(dtoModel.SubjectId);
            await subjectActor.InitSubjectActor(dtoModel);
        }

        private SubjectMdo MapDtoSubjectMdo(UploadMetadataDto uploadDto)
        {
            var upload = new SubjectUploadMdo()
            {
                BeginTimestampUtc = uploadDto.BeginTimeStampUtc,
                EndTimestampUtc = uploadDto.EndTimeStampUtc
            };

            return new SubjectMdo()
            {
                StudyId = uploadDto.StudyId,
                SubjectId = uploadDto.SubjectId,
                DeviceSerial = uploadDto.DeviceSerial,
                SubjectUpload = upload,
            };
        }
    }
}
