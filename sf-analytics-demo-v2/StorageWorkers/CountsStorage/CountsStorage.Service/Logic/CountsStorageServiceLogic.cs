using CountsStorage.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CountsStorage.Service.Logic
{
    public class CountsStorageServiceLogic: ICountsStorageServiceLogic
    {
        private readonly string _connectionString;
        public CountsStorageServiceLogic(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> InsertNewCounts(CountsStorageDto countsStorageDto)
        {
            var dataTable = GetCountsDataTable(countsStorageDto.SubjectId, countsStorageDto.DeviceId, countsStorageDto.SettingsId, countsStorageDto.CountsDto);
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

        private DataTable GetCountsDataTable(long subjectId, string deviceId, Guid settingsId, CountsDto[] countDtos)
        {
            var dataTable = CreateCountsDataTable();

            foreach (var count in countDtos)
            {
                var timestampUtc = DateTimeOffset.FromUnixTimeSeconds(count.TimestampUnixUtc);
                var dateTimeUtc = new DateTime(timestampUtc.Year, timestampUtc.Month, timestampUtc.Day, timestampUtc.Hour, timestampUtc.Minute, timestampUtc.Second, DateTimeKind.Utc);
                var dataRow = dataTable.NewRow();
                dataRow[nameof(CountsEntity.SubjectId)] = subjectId;
                dataRow[nameof(CountsEntity.DeviceId)] = deviceId;
                dataRow[nameof(CountsEntity.SettingsId)] = settingsId;
                dataRow[nameof(CountsEntity.TimestampUtc)] = dateTimeUtc;
                dataRow[nameof(CountsEntity.XAxis)] = count.XAxis;
                dataRow[nameof(CountsEntity.YAxis)] = count.YAxis;
                dataRow[nameof(CountsEntity.ZAxis)] = count.ZAxis;
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private DataTable CreateCountsDataTable()
        {
            var dt = new DataTable { TableName = $"[algout].[CountsOutput]" };
            dt.Columns.AddRange(new[]
            {
                new DataColumn
                {
                    DataType = typeof(long),
                    ColumnName = nameof(CountsEntity.SubjectId)
                },
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = nameof(CountsEntity.DeviceId)
                },
                new DataColumn
                {
                    DataType = typeof(DateTime),
                    ColumnName = nameof(CountsEntity.TimestampUtc)
                },
                new DataColumn
                {
                    DataType = typeof(int),
                    ColumnName = nameof(CountsEntity.XAxis)
                },
                new DataColumn
                {
                    DataType = typeof(int),
                    ColumnName = nameof(CountsEntity.YAxis)
                },
                  new DataColumn
                {
                    DataType = typeof(int),
                    ColumnName = nameof(CountsEntity.ZAxis)
                },
                new DataColumn
                {
                    DataType = typeof(Guid),
                    ColumnName = nameof(CountsEntity.SettingsId)
                }
            });
            return dt;
        }

    }

}
