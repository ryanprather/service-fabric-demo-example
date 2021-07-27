using DustinTracyStorage.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DustinTracyStorage.Service.Logic
{
    public class DustinTracyStorageServiceLogic: IDustinTracyStorageServiceLogic
    {
        private readonly string _connectionString;
        public DustinTracyStorageServiceLogic(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> InsertNewDustinTracySleepPeriods(DustinTracyStorageDto countsStorageDto)
        {
            var dataTable = GetDustinTracyDataTable(countsStorageDto.SubjectId, countsStorageDto.DeviceId, countsStorageDto.SettingsId, countsStorageDto.SleepPeriods);
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

        private DataTable GetDustinTracyDataTable(long subjectId, string deviceId, Guid settingsId, DustinTracySleepPeriodDto[] sleepPeriodsDto)
        {
            var dataTable = CreateDustinTracyDataTable();

            foreach (var sleepPeriod in sleepPeriodsDto)
            {
                var dataRow = dataTable.NewRow();
                dataRow[nameof(DustinTracyEntity.SubjectId)] = subjectId;
                dataRow[nameof(DustinTracyEntity.DeviceId)] = deviceId;
                dataRow[nameof(DustinTracyEntity.SettingsId)] = settingsId;
                dataRow[nameof(DustinTracyEntity.SleepPeriodStartUtc)] = sleepPeriod.BeginTimeUtc;
                dataRow[nameof(DustinTracyEntity.SleepPeriodEndUtc)] = sleepPeriod.EndTimeUtc;
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private DataTable CreateDustinTracyDataTable()
        {
            var dt = new DataTable { TableName = $"[algout].[DustinTracyOutput]" };
            dt.Columns.AddRange(new[]
            {
                new DataColumn
                {
                    DataType = typeof(long),
                    ColumnName = nameof(DustinTracyEntity.SubjectId)
                },
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = nameof(DustinTracyEntity.DeviceId)
                },
                new DataColumn
                {
                    DataType = typeof(DateTime),
                    ColumnName = nameof(DustinTracyEntity.SleepPeriodStartUtc)
                },
                new DataColumn
                {
                    DataType = typeof(DateTime),
                    ColumnName = nameof(DustinTracyEntity.SleepPeriodEndUtc)
                },
                new DataColumn
                {
                    DataType = typeof(Guid),
                    ColumnName = nameof(DustinTracyEntity.SettingsId)
                }
            });
            return dt;
        }
    }
}
