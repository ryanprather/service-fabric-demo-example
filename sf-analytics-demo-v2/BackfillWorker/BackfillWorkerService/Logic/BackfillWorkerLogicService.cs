using BackfillWorker.Models;
using Dapper;
using Global.Services;
using SubjectModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BackfillWorkerService.Logic
{
    public class BackfillWorkerLogicService : IBackfillWorkerLogicService
    {
        private readonly string _connectionString;
        public BackfillWorkerLogicService(string connectionString)
        {
            _connectionString = connectionString;

        }

        public async Task<IEnumerable<BackfillUpload>> GetBackfillSubjects()
        {
            var sql = @"SELECT subjs.SubjectId, subjs.DeviceSerial, Min(Timestamp) as UploadBeginTimestampUtc, MAX(Timestamp) as UploadEndTimestampUtc
                            FROM [dbo].[Counts] as cnts
                            JOIN [dbo].[StudySubjectDevices] as subjs ON subjs.Id = cnts.StudySubjectDeviceId
                            Group by subjs.SubjectId, subjs.DeviceSerial";

            using (var connection = new SqlConnection(_connectionString))
            {
                var subjectUploads = await connection.QueryAsync<BackfillUpload>(sql);
                return subjectUploads;
            }
        }

        public async Task ProcessSubjectUploads(IEnumerable<BackfillUpload> subjectBackfillUploads)
        {
            if (subjectBackfillUploads.Any())
            {
                foreach (var subjectUpload in subjectBackfillUploads)
                {
                    var beginTimestampUtc = DateTimeOffset.FromUnixTimeSeconds(subjectUpload.UploadBeginTimestampUtc);
                    var endTimestampUtc  = DateTimeOffset.FromUnixTimeSeconds(subjectUpload.UploadEndTimestampUtc);

                    var subjectMdo = new SubjectMdo()
                    {
                        StudyId = 423,
                        SubjectId = subjectUpload.SubjectId,
                        DeviceSerial = subjectUpload.DeviceSerial,
                        SubjectUpload = new SubjectUploadMdo()
                        {
                            BeginTimestampUtc = new DateTime(beginTimestampUtc.Year, beginTimestampUtc.Month, beginTimestampUtc.Day, beginTimestampUtc.Hour, beginTimestampUtc.Minute, beginTimestampUtc.Second), 
                            EndTimestampUtc = new DateTime(endTimestampUtc.Year, endTimestampUtc.Month, endTimestampUtc.Day, endTimestampUtc.Hour, endTimestampUtc.Minute, endTimestampUtc.Second),
                        },
                    };

                    var subjectActor = FabricServices.GetSubjectActor(subjectMdo.SubjectId);
                    await subjectActor.InitSubjectActor(subjectMdo);
                }
            }
        }
    }
}
