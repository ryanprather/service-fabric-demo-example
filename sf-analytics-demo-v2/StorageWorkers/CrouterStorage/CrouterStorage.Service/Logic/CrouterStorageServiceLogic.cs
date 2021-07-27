using CrouterStorage.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CrouterStorage.Service.Logic
{
    public class CrouterStorageServiceLogic: ICrouterStorageServiceLogic
    {
        private readonly string _connectionString;
        public CrouterStorageServiceLogic(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> InsertNewCrouterCutpoints(CrouterStorageDto countsStorageDto)
        {
            var dataTable = GetCrouterDataTable(countsStorageDto.SubjectId, countsStorageDto.DeviceId, countsStorageDto.SettingsId, countsStorageDto.CrouterDto);
            await Task.Delay(100);
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.CheckConstraints, trans))
                        {
                            bulkCopy.BulkCopyTimeout = 3600;
                            bulkCopy.BatchSize = 1000;

                            foreach (DataColumn column in dataTable.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                            }

                            //Bulk insert into temp table
                            bulkCopy.DestinationTableName = dataTable.TableName;
                            await bulkCopy.WriteToServerAsync(dataTable);
                        }

                        await trans.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await trans.RollbackAsync();
                        var message = ex.Message;
                    }
                }
            }

            return dataTable.Rows.Count;
        }

        private DataTable GetCrouterDataTable(long subjectId, string deviceId, Guid settingsId, CrouterDto[] crouterDtos)
        {
            var dataTable = CreateCrouterDataTable();

            foreach (var cutpoint in crouterDtos)
            {
                var timestampUtc = DateTimeOffset.FromUnixTimeSeconds(cutpoint.TimestampUtc);
                var dateTimeUtc = new DateTime(timestampUtc.Year, timestampUtc.Month, timestampUtc.Day, timestampUtc.Hour, timestampUtc.Minute, timestampUtc.Second, DateTimeKind.Utc);
                var dataRow = dataTable.NewRow();
                dataRow[nameof(CrouterEntity.SubjectId)] = subjectId;
                dataRow[nameof(CrouterEntity.DeviceId)] = deviceId;
                dataRow[nameof(CrouterEntity.SettingsId)] = settingsId;
                dataRow[nameof(CrouterEntity.TimestampUtc)] = dateTimeUtc;
                dataRow[nameof(CrouterEntity.CutPointBucketVerticalAxis)] = cutpoint.VerticalAxisResult;
                dataRow[nameof(CrouterEntity.CutPointBucketVectorMagnitude)] = cutpoint.VectorMagnitudeAxisResult;
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private DataTable CreateCrouterDataTable()
        {
            var dt = new DataTable { TableName = $"[algout].[CrouterOutput]" };
            dt.Columns.AddRange(new[]
            {
                new DataColumn
                {
                    DataType = typeof(long),
                    ColumnName = nameof(CrouterEntity.SubjectId)
                },
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = nameof(CrouterEntity.DeviceId)
                },
                new DataColumn
                {
                    DataType = typeof(DateTime),
                    ColumnName = nameof(CrouterEntity.TimestampUtc)
                },
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = nameof(CrouterEntity.CutPointBucketVerticalAxis)
                },
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = nameof(CrouterEntity.CutPointBucketVectorMagnitude)
                },
                new DataColumn
                {
                    DataType = typeof(Guid),
                    ColumnName = nameof(CrouterEntity.SettingsId)
                }
            });
            return dt;
        }
    }
}
