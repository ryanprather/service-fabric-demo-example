using Dapper;
using EpochRetrieval.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace EpochRetrievalActor.EpochSqlService
{
    public class EpochSqlService : IEpochSqlService
    {
        private readonly string _connectionString;
        public EpochSqlService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<EpochRecord>> GetEpochsAsync(long subjectId, DateTime adjustedBeginTimestampUtc, DateTime adjustedEndTimestampUtc)
        {
            adjustedBeginTimestampUtc = DateTime.SpecifyKind(adjustedBeginTimestampUtc, DateTimeKind.Utc);
            adjustedEndTimestampUtc = DateTime.SpecifyKind(adjustedEndTimestampUtc.AddSeconds(1), DateTimeKind.Utc);
            DateTimeOffset adjustedBeginTimestampUtcOffset = adjustedBeginTimestampUtc;
            DateTimeOffset adjustedEndTimestampUtcOffset = adjustedEndTimestampUtc;
            var parameters = new
            {
                @SubjectId = subjectId,
                @BeginTimestampUtc = adjustedBeginTimestampUtcOffset.ToUnixTimeSeconds(),
                @EndTimestampUtc = adjustedEndTimestampUtcOffset.ToUnixTimeSeconds(),
            };
            var sql = @"SELECT [Timestamp] as 'TimestampUnixUtc', [X] as 'XAxisCounts' ,[Y] as 'YAxisCounts', [Z] as 'ZAxisCounts'
                        FROM [dbo].[Counts] as cnt
                        JOIN dbo.StudySubjectDevices as ssd ON cnt.StudySubjectDeviceId = ssd.Id
                        WHERE ssd.SubjectId = @SubjectId AND Timestamp >= @BeginTimestampUtc AND Timestamp <= @EndTimestampUtc";

            using (var connection = new SqlConnection(_connectionString))
            {
                var epochEntities = await connection.QueryAsync<EpochRecord>(sql, parameters);
                return epochEntities;
            }
        }

    }
}
