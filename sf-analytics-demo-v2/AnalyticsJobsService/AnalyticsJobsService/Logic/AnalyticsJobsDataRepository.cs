using AnalyticsJobsService.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AnalyticsJobsService.Logic
{
    public class AnalyticsJobsDataRepository : IAnalyticsJobsDataRepository
    {
        private readonly string _connectionString;
        public AnalyticsJobsDataRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Study Subject functions
        /// <summary>
        /// Check if a subject already exists
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public async Task<bool> StudySubjectExistsAsync(long studyId, long subjectId)
        {
            bool exists = false;

            var parameters = new { StudyId = studyId, SubjectId = subjectId };

            var sql = $@"SELECT Count(*) 
                        FROM ServiceFabric.StudySubject 
                        WHERE StudyId = @StudyId AND SubjectId = @SubjectId";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var subjectDeviceCount = await connection.QueryFirstAsync<int>(sql, parameters);
                    if (subjectDeviceCount > 0)
                        exists = true;
                }
            }
            catch (Exception ex)
            {
                var test = ex;
            }


            return exists;
        }

        /// <summary>
        /// Create new Subject In the Database 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task CreateNewStudySubjectAsync(StudySubjectEntity subject)
        {
            var parameters = new { StudyId = subject.StudyId, SubjectId = subject.SubjectId };
            try
            {
                var sql = $@"INSERT INTO ServiceFabric.StudySubject 
                        (StudyId, SubjectId)
                        Values(@StudyId, @SubjectId)";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.ExecuteAsync(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                var test = ex;
            }

        }

        /// <summary>
        /// Get Study Subject Entity from the database 
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public async Task<StudySubjectEntity> GetStudySubjectEntityAsync(long studyId, long subjectId)
        {
            var parameters = new { StudyId = studyId, SubjectId = subjectId };

            var sql = $@"SELECT * 
                        FROM ServiceFabric.StudySubject 
                        WHERE StudyId = @StudyId AND SubjectId = @SubjectId";

            using (var connection = new SqlConnection(_connectionString))
            {
                var studySubject = await connection.QueryFirstAsync<StudySubjectEntity>(sql, parameters);
                return studySubject;
            }
        }
        #endregion

        #region Study Device functions
        public async Task<bool> SubjectDeviceExistsAsync(Guid studySubjectId, string deviceSerial)
        {
            bool exists = false;

            var parameters = new { StudySubjectId = studySubjectId, DeviceSerial = deviceSerial };

            var sql = $@"SELECT Count(*) 
                        FROM ServiceFabric.SubjectDevice 
                        WHERE StudySubjectId = @StudySubjectId AND DeviceSerial = @DeviceSerial";

            using (var connection = new SqlConnection(_connectionString))
            {
                var subjectDeviceCount = await connection.QueryFirstAsync<int>(sql, parameters);
                if (subjectDeviceCount > 0)
                    exists = true;
            }

            return exists;
        }

        public async Task CreateNewSubjectDeviceAsync(SubjectDeviceEntity subjectDeviceEntity)
        {
            var parameters = new { StudySubjectId = subjectDeviceEntity.StudySubjectId, DeviceSerial = subjectDeviceEntity.DeviceSerial };

            var sql = $@"INSERT INTO ServiceFabric.SubjectDevice 
                        (StudySubjectId, DeviceSerial)
                        Values(@StudySubjectId, @DeviceSerial)";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.ExecuteAsync(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                var test = ex;
            }


        }

        public async Task<SubjectDeviceEntity> GetSubjectDeviceAsync(Guid studySubjectId, string deviceSerial)
        {
            var parameters = new { StudySubjectId = studySubjectId, DeviceSerial = deviceSerial };

            var sql = $@"SELECT * 
                        FROM ServiceFabric.SubjectDevice 
                        WHERE StudySubjectId = @StudySubjectId AND DeviceSerial = @DeviceSerial";

            using (var connection = new SqlConnection(_connectionString))
            {
                var subjectDevice = await connection.QueryFirstAsync<SubjectDeviceEntity>(sql, parameters);
                return subjectDevice;
            }
        }
        #endregion

        #region Subject Device Uploads
        public async Task<bool> SubjectUploadExistsAsync(Guid subjectDeviceId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {
            bool exists = false;

            var parameters = new { @SubjectDeviceId = subjectDeviceId, @BeginTimestampUtc = beginTimestampUtc, @EndTimestampUtc = endTimestampUtc };

            var sql = $@"SELECT Count(*) 
                        FROM ServiceFabric.SubjectDeviceUpload 
                        WHERE SubjectDeviceId = @SubjectDeviceId AND BeginTimestampUtc = @BeginTimestampUtc AND EndTimestampUtc = @EndTimestampUtc";

            using (var connection = new SqlConnection(_connectionString))
            {
                var subjectDeviceCount = await connection.QueryFirstAsync<int>(sql, parameters);
                if (subjectDeviceCount > 0)
                    exists = true;
            }

            return exists;
        }

        public async Task CreateNewSubjectUploadAsync(SubjectDeviceUploadEntity subjectUploadEntity)
        {
            var parameters = new
            {
                SubjectDeviceId = subjectUploadEntity.SubjectDeviceId,
                BeginTimestampUtc = subjectUploadEntity.BeginTimestampUtc,
                EndTimestampUtc = subjectUploadEntity.EndTimestampUtc,
            };

            var sql = $@"INSERT INTO ServiceFabric.SubjectDeviceUpload 
                        (SubjectDeviceId, BeginTimestampUtc, EndTimestampUtc)
                        Values(@SubjectDeviceId, @BeginTimestampUtc, @EndTimestampUtc)";
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.ExecuteAsync(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                var test = ex;
            }

        }

        public async Task<SubjectDeviceUploadEntity> GetSubjectUploadAsync(Guid subjectDeviceId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {
            var parameters = new { @SubjectDeviceId = subjectDeviceId, @BeginTimestampUtc = beginTimestampUtc, @EndTimestampUtc = endTimestampUtc };

            var sql = $@"SELECT * 
                        FROM ServiceFabric.SubjectDeviceUpload 
                        WHERE SubjectDeviceId = @subjectDeviceId AND BeginTimestampUtc = @BeginTimestampUtc AND EndTimestampUtc = @EndTimestampUtc";

            using (var connection = new SqlConnection(_connectionString))
            {
                var studySubject = await connection.QueryFirstAsync<SubjectDeviceUploadEntity>(sql, parameters);
                return studySubject;
            }
        }

        public async Task<bool> SubjectUploadTimeRangeMatchesAsync(Guid subjectDeviceUploadId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {
            bool exists = false;

            var parameters = new
            {
                Id = subjectDeviceUploadId,
                BeginTimestampUtc = beginTimestampUtc,
                EndTimestampUtc = endTimestampUtc
            };

            var sql = $@"SELECT Count(*) 
                        FROM ServiceFabric.SubjectDeviceUpload 
                        WHERE Id = @Id 
                            AND BeginTimestampUtc = @BeginTimestampUtc 
                            AND EndTimestampUtc = @EndTimestampUtc";

            using (var connection = new SqlConnection(_connectionString))
            {
                var subjectDeviceCount = await connection.QueryFirstAsync<int>(sql, parameters);
                if (subjectDeviceCount > 0)
                    exists = true;
            }

            return exists;
        }

        public async Task UpdateSubjectUploadTimeRangeAsync(Guid subjectDeviceUploadId, DateTime beginTimestampUtc, DateTime endTimestampUtc)
        {
            var parameters = new { BeginTimestampUtc = beginTimestampUtc, EndTimestampUtc = endTimestampUtc, Id = subjectDeviceUploadId };

            var sql = $@"UPDATE ServiceFabric.SubjectDeviceUpload SET 
                            BeginTimestampUtc = @BeginTimestampUtc,
                            EndTimestampUtc = @EndTimestampUtc
                        WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                var studySubject = await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<SubjectUploadDto> GetSubjectUploadWithDevice(Guid subjectDeviceUploadId)
        {
            var parameters = new { Id = subjectDeviceUploadId };

            var sql = $@"SELECT sdu.Id, sd.DeviceSerial, sdu.BeginTimestampUtc, sdu.EndTimestampUtc
                        FROM ServiceFabric.SubjectDeviceUpload as sdu
                        JOIN ServiceFabric.SubjectDevice as sd ON sd.Id = sdu.SubjectDeviceId
                        WHERE sdu.Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                var upload = await connection.QueryFirstAsync<SubjectUploadDto>(sql, parameters);
                return upload;
            }
        }

        public Task<List<SubjectDeviceUploadEntity>> GetFollowingUploadsFromUploadAsync(SubjectDeviceUploadEntity subjectUploadEntity)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Analytics Settings
        /// <summary>
        /// get if any algorithm settings exist for a study
        /// </summary>
        /// <param name="studyId"></param>
        /// <returns></returns>
        public async Task<bool> AlgorithmSettingExistsForStudyAsync(long studyId)
        {
            bool exists = false;
            var parameters = new { StudyId = studyId, Deleted = false };

            var sql = $@"SELECT Count(*) 
                        FROM Analytics.AlgorithmSettings 
                        WHERE StudyId = @StudyId AND Deleted = @Deleted";

            using (var connection = new SqlConnection(_connectionString))
            {
                var studySubject = await connection.QueryFirstAsync<int>(sql, parameters);
                if (studySubject > 0)
                    exists = true;
            }

            return exists;
        }

        /// <summary>
        /// get list of non deleted algorithm settings
        /// </summary>
        /// <param name="studyId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AlgorithmSettingEntity>> GetAlgorithmSettingForStudyAsync(long studyId)
        {
            var parameters = new { StudyId = studyId, Deleted = 0 };

            var sql = $@"SELECT ass.Id, ass.AnalyticsTypeId, ass.Name, ass.Settings, ass.Deleted, att.IsStatelessAlgorithm, att.InputDataType as AlgorithmInputDataType
                        FROM Analytics.AlgorithmSettings as ass
                        JOIN Analytics.AnalyticsTypes as att ON ass.AnalyticsTypeId = att.Id 
                        WHERE StudyId = @StudyId AND Deleted = @Deleted";

            using (var connection = new SqlConnection(_connectionString))
            {
                var settingEntities = await connection.QueryAsync<AlgorithmSettingEntity>(sql, parameters);
                return settingEntities;
            }
        }

        /// <summary>
        /// get if any algorithm settings exist for a study
        /// </summary>
        /// <param name="studyId"></param>
        /// <returns></returns>
        public async Task MinuteAggregationsSettingForAlgorithmSettingsAsync(long studyId)
        {
            // TODO: Update data base and add query to retrieve minute stats for algorithms // 
        }


        #endregion

        #region Analytics Algorithm Tasks
        public async Task<AlgorithmTaskEntity> CreateNewAlgorithmTaskEntityAsync(Guid uploadProcessingJobId, Guid analtyicsSettingsId, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc)
        {
            var algorithmTask = new AlgorithmTaskEntity()
            {
                Id = Guid.NewGuid(),
                UploadProcessingJobId = uploadProcessingJobId,
                CreatedDateTimeUtc = DateTime.UtcNow,
                AlgorithmSettingId = analtyicsSettingsId,
                AdjustedBeginTimestampUtc = adjustedBeginTimestampUtc,
                AdjustedEndTimestampUtc = adjustedEndTimestampUtc,
            };
            var parameters = new
            {
                Id = algorithmTask.Id,
                UploadProcessingJobId = algorithmTask.UploadProcessingJobId,
                CreatedDateTimeUtc = algorithmTask.CreatedDateTimeUtc,
                AnalyticsSettingsId = analtyicsSettingsId,
                AdjustedBeginTimestampUtc = adjustedBeginTimestampUtc,
                AdjustedEndTimestampUtc = adjustedEndTimestampUtc,
            };

            var sql = $@"INSERT INTO ServiceFabric.AlgorithmTask 
                        (Id, UploadProcessingJobId, CreatedDateTimeUtc, AlgorithmSettingId, AdjustedBeginTimestampUtc, AdjustedEndTimestampUtc)
                        Values(@Id, @UploadProcessingJobId, @CreatedDateTimeUtc, @AnalyticsSettingsId, @AdjustedBeginTimestampUtc, @AdjustedEndTimestampUtc)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }

            return algorithmTask;
        }

        public async Task<IEnumerable<AlgorithmTaskDto>> GetAlgorithmTaskDtos(Guid uploadProcessingJobId)
        {
            var parameters = new { @UploadProcessingJobId = uploadProcessingJobId };

            var sql = @"SELECT sfat.Id, aas.AnalyticsTypeId, aat.InputDataType as 'AlgorithmInputDataType',aas.Id as 'AlgorithmSettingId', 
                        sfat.AdjustedBeginTimestampUtc, sfat.AdjustedEndTimestampUtc, aas.Settings, sfat.ErrorReason, sfat.IsError
                        FROM ServiceFabric.AlgorithmTask as sfat
                        JOIN Analytics.AlgorithmSettings as aas ON sfat.AlgorithmSettingId = aas.Id
                        JOIN Analytics.AnalyticsTypes as aat ON aas.AnalyticsTypeId = aat.Id
                        WHERE sfat.UploadProcessingJobId = @UploadProcessingJobId";

            using (var connection = new SqlConnection(_connectionString))
            {
                var settingEntities = await connection.QueryAsync<AlgorithmTaskDto>(sql, parameters);
                return settingEntities;
            }
        }

        public async Task UpdateAlgorithmTaskStarted(Guid taskId, DateTime processingStartedUtc) 
        {
            var parameters = new
            {
                @TaskId = taskId,
                @ProcessingStartedUtc = processingStartedUtc
            };

            var sql = @"UPDATE ServiceFabric.AlgorithmTask 
                        SET ProcessingStartedUtc = @ProcessingStartedUtc 
                        WHERE Id = @TaskId";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task UpdateAlgorithmTaskCompleted(Guid taskId, DateTime processingCompletedUtc, int itemsComputed)
        {
            var parameters = new
            {
                @TaskId = taskId,
                ProcessingCompletedDateTimeUtc = processingCompletedUtc,
                @ItemsComputed = itemsComputed
            };

            var sql = @"UPDATE ServiceFabric.AlgorithmTask 
                        SET ProcessingCompletedDateTimeUtc = @ProcessingCompletedDateTimeUtc, 
                            ItemsComputed = @ItemsComputed
                        WHERE Id = @TaskId";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task UpdateAlgorithmTaskErrored(Guid taskId, DateTime processingCompletedUtc, string errorMessage)
        {
            var parameters = new
            {
                @TaskId = taskId,
                @ProcessingCompletedDateTimeUtc = processingCompletedUtc,
                @ErrorReason = errorMessage
            };

            var sql = @"UPDATE ServiceFabric.AlgorithmTask 
                        SET ProcessingCompletedDateTimeUtc = @@ProcessingCompletedDateTimeUtc, 
                            IsError = @IsError
                        WHERE Id = @TaskId";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task UpdateAlgorithmTaskProcessingComplete(Guid taskId, DateTime processingStartedUtc, DateTime processingEndedUtc, int itemsComputed, string errorMessage)
        {


            var parameters = new
            {
                @TaskId = taskId,
                @ProcessingStartedUtc = processingStartedUtc,
                @ProcessingCompletedDateTimeUtc = processingEndedUtc,
                @ItemsComputed = itemsComputed,
                @ErrorReason = (string.IsNullOrEmpty(errorMessage)) ? null : errorMessage,
                @IsError = (string.IsNullOrEmpty(errorMessage)) ? false : true,
            };

            var sql = @"UPDATE ServiceFabric.AlgorithmTask 
                        SET ProcessingStartedUtc = @ProcessingStartedUtc, 
                        ProcessingCompletedDateTimeUtc = @ProcessingCompletedDateTimeUtc,
                        ItemsComputed = @ItemsComputed,
                        IsError = @IsError, 
                        ErrorReason = @ErrorReason
                        WHERE Id = @TaskId";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }

            if (itemsComputed == 0)
            {
                var param = new
                {
                    @StorageStartedUtc = processingStartedUtc,
                    @StorageCompletedUtc = processingEndedUtc,
                    @TaskId = taskId
                };

                var updateSql = @"UPDATE ServiceFabric.AlgorithmTask 
                        SET StorageStartedUtc = @StorageStartedUtc,
                        StorageCompletedUtc = @StorageCompletedUtc
                        WHERE Id = @TaskId";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.ExecuteAsync(updateSql, param);
                }

            }
        }

        public async Task UpdateAlgorithmTaskStorageItemsComplete(Guid taskId, DateTime storageStartedUtc, DateTime storageCompletedUtc, int itemsProcessedByStorage, string errorMessage)
        {
            try
            {
                var parameters = new { @Id = taskId };
                var sql = @"SELECT Id, ItemsComputed, ItemsStored, StorageStartedUtc, StorageCompletedUtc
                            FROM [ServiceFabric].[AlgorithmTask]
	                        WHERE Id = @Id";
                using (var connection = new SqlConnection(_connectionString))
                {
                    var algorithmTask = await connection.QueryFirstAsync<AlgorithmTaskStorageUpdateDto>(sql, parameters);
                    // storage has not started update started, update items, update completed //
                    if (!algorithmTask.StorageStartedUtc.HasValue)
                    {
                        var param1 = new { @Id = taskId, @StorageStartedUtc = storageStartedUtc, @StorageCompletedUtc = storageCompletedUtc, @ItemsStored = (itemsProcessedByStorage + algorithmTask.ItemsStored) };
                        sql = @"UPDATE [ServiceFabric].[AlgorithmTask] 
                            SET ItemsStored = @ItemsStored, StorageStartedUtc = @StorageStartedUtc, StorageCompletedUtc = @StorageCompletedUtc
                            WHERE Id = @Id";
                        await connection.ExecuteAsync(sql, param1);
                    }
                    // storage has started all items stored updated and storage completed updated //
                    else if (algorithmTask.StorageStartedUtc.HasValue)
                    {
                        var param1 = new { Id = taskId, @StorageCompletedUtc = storageCompletedUtc, ItemsStored = (itemsProcessedByStorage + algorithmTask.ItemsStored) };
                        sql = @"UPDATE [ServiceFabric].[AlgorithmTask] 
                            SET ItemsStored = @ItemsStored, StorageCompletedUtc = @StorageCompletedUtc
                            WHERE Id = @Id";
                        await connection.ExecuteAsync(sql, param1);
                    }
                }
            }
            catch (Exception ex)
            {
                var test = ex;
            }

        }

        #endregion

        #region Upload Processing Job
        /// <summary>
        /// Adds new Upload Processing Job to the database 
        /// </summary>
        /// <param name="uploadProcessingJobEntity"></param>
        /// <returns></returns>
        public async Task CreateNewUploadProcessingJob(Guid subjectDeviceUploadId)
        {
            var parameters = new
            {
                SubjectDeviceUploadId = subjectDeviceUploadId,
                CreatedDateTimeUtc = DateTime.UtcNow,
                IsError = false,
            };

            var sql = $@"INSERT INTO ServiceFabric.UploadProcessingJob 
                        (SubjectDeviceUploadId, CreatedDateTimeUtc, IsError)
                        Values(@SubjectDeviceUploadId, @CreatedDateTimeUtc, @IsError)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }

        /// <summary>
        /// Get subject jobs that have not started processing
        /// </summary>
        /// <param name="subjectDeviceUploadId"></param>
        /// <returns></returns>
        public async Task<UploadProcessingJobEntity> GetSubjectUploadNotStartedJob(Guid subjectDeviceUploadId)
        {
            var parameters = new
            {
                SubjectDeviceUploadId = subjectDeviceUploadId,
            };

            var sql = $@"SELECT * FROM ServiceFabric.UploadProcessingJob 
                        WHERE SubjectDeviceUploadId = @SubjectDeviceUploadId AND ProcessingStartedUtc IS null";

            using (var connection = new SqlConnection(_connectionString))
            {
                var studySubject = await connection.QueryFirstAsync<UploadProcessingJobEntity>(sql, parameters);
                return studySubject;
            }

        }

        /// <summary>
        /// Get Upload Processing Job by Id
        /// </summary>
        /// <param name="uploadProcessingJobEntity"></param>
        /// <returns></returns>
        public async Task<UploadProcessingJobEntity> GetUploadProcessingJob(Guid uploadProcessingJobId)
        {
            var parameters = new
            {
                Id = uploadProcessingJobId,
            };

            var sql = $@"SELECT * FROM ServiceFabric.UploadProcessingJob 
                        WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                var studySubject = await connection.QueryFirstAsync<UploadProcessingJobEntity>(sql, parameters);
                return studySubject;
            }
        }

        public async Task SetProcessingJobStartedDateTime(DateTime startedDateTime, Guid jobId)
        {
            var parameters = new { @Id = jobId, @ProcessingStartedUtc = startedDateTime };

            var sql = $@"UPDATE ServiceFabric.UploadProcessingJob 
                        SET ProcessingStartedUtc = @ProcessingStartedUtc
                        WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task SetProcessingJobCompletedDateTime(DateTime completedDateTime, Guid jobId)
        {
            var parameters = new { @Id = jobId, @CompletedDateTimeUtc = completedDateTime };

            var sql = $@"UPDATE ServiceFabric.UploadProcessingJob 
                        SET CompletedDateTimeUtc = @CompletedDateTimeUtc
                        WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }
        #endregion

        #region Processing Job Epoch Retrieval
        public async Task CreateEpochRetrievalForProcessingJob(Guid uploadProcessingJobId, DateTime processingStartedUtc, DateTime processingCompletedDateTimeUtc, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc, string errorReason)
        {
            var parameters = new
            {
                UploadProcessingJobId = uploadProcessingJobId,
                ProcessingStartedUtc = processingStartedUtc,
                ProcessingCompletedDateTimeUtc = processingCompletedDateTimeUtc,
                AdjustedBeginTimestampUtc = adjustedBeginTimestampUtc,
                AdjustedEndTimestampUtc = adjustedEndTimestampUtc,
                ErrorReason = (string.IsNullOrEmpty(errorReason)) ? null : errorReason,
                IsError = (string.IsNullOrEmpty(errorReason)) ? false : true,
            };

            var sql = $@"INSERT INTO ServiceFabric.ProcessingJobEpochRetrieval 
                        (UploadProcessingJobId, ProcessingStartedUtc, ProcessingCompletedDateTimeUtc, AdjustedBeginTimestampUtc, AdjustedEndTimestampUtc, IsError, ErrorReason)
                        Values(@UploadProcessingJobId, @ProcessingStartedUtc, @ProcessingCompletedDateTimeUtc, @AdjustedBeginTimestampUtc, @AdjustedEndTimestampUtc, @IsError, @ErrorReason)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }
        #endregion
    }
}
