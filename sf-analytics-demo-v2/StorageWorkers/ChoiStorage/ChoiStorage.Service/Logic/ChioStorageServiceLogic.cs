using ChoiStorage.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ChoiStorage.Service.Logic
{
    public class ChioStorageServiceLogic : IChioStorageServiceLogic
    {
        private readonly string _connectionString;
        public ChioStorageServiceLogic(string connectionString) 
        {
            _connectionString = connectionString;
        }

        public async Task<int> InsertNewChoiWearPeriods(ChoiStorageDto choiStorageDto) 
        {
            var dataTable = GetChoiDataTable(choiStorageDto.SubjectId, choiStorageDto.DeviceId, choiStorageDto.SettingsId, choiStorageDto.WearPeriods);
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

        private DataTable GetChoiDataTable(long subjectId, string deviceId, Guid settingsId, ChoiWearPeriodDto[] wearPeriods) 
        {
            var dataTable = CreateChoiDataTable();

            foreach (var wearPeriod in wearPeriods) 
            {
                var dataRow = dataTable.NewRow();
                dataRow[nameof(ChoiEntity.SubjectId)] = subjectId;
                dataRow[nameof(ChoiEntity.DeviceId)] = deviceId;
                dataRow[nameof(ChoiEntity.SettingsId)] = settingsId;
                dataRow[nameof(ChoiEntity.WearPeriodStartUtc)] = wearPeriod.BeginTimeUtc;
                dataRow[nameof(ChoiEntity.WearPeriodEndUtc)] = wearPeriod.EndTimeUtc;
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private DataTable CreateChoiDataTable() 
        {
            var dt = new DataTable { TableName = $"[algout].[ChoiOutput]" };
            dt.Columns.AddRange(new[]
            {
                new DataColumn
                {
                    DataType = typeof(long),
                    ColumnName = nameof(ChoiEntity.SubjectId)
                },
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = nameof(ChoiEntity.DeviceId)
                },
                new DataColumn
                {
                    DataType = typeof(DateTime),
                    ColumnName = nameof(ChoiEntity.WearPeriodStartUtc)
                },
                new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = nameof(ChoiEntity.WearPeriodEndUtc)
                },
                new DataColumn
                {
                    DataType = typeof(Guid),
                    ColumnName = nameof(ChoiEntity.SettingsId)
                }
            });
            return dt;
        }
    }
}
