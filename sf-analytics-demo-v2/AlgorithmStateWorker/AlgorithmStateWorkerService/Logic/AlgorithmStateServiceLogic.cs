using AlgorithmStateWorker.Models;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AlgorithmStateWorkerService.Logic
{
    public class AlgorithmStateServiceLogic : IAlgorithmStateServiceLogic
    {
        private readonly string _connectionString;
        public AlgorithmStateServiceLogic(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task StoreChoiStates(ChoiStateDto[] choiStates)
        {

            try
            {
                var sql = $@"INSERT into ServiceFabric.ChoiAlgorithmStates (SubjectId, AlgorithmSettingId, DataStartTimestamp)
                    VALUES (@SubjectId, @AlgorithmSettingId, @DataStartTimestamp)";
                
                var newChoiStates = choiStates.ToList().Select(x => new
                {
                    SubjectId = x.SubjectId,
                    AlgorithmSettingId = x.SettingsId,
                    DataStartTimestamp = x.DataStartTimestamp
                });

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.ExecuteAsync(sql, newChoiStates);
                }
            }
            catch (Exception ex)
            {
                var test = ex;
            }
        }

        public async Task StoreDustinTracyStates(DustinTracyStateDto[] dustinTracyStates)
        {
            try
            {
                var sql = $@"INSERT into ServiceFabric.DustinTracyAlgorithmStates (SubjectId, AlgorithmSettingId, DataStartTimestamp)
                    VALUES (@SubjectId, @AlgorithmSettingId, @DataStartTimestamp)";
                var newDustinTracyStates = dustinTracyStates.ToList().Select(x => new
                {
                    SubjectId = x.SubjectId,
                    AlgorithmSettingId = x.SettingsId,
                    DataStartTimestamp = x.DataStartTimestamp
                });

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.ExecuteAsync(sql, newDustinTracyStates);
                }
            }
            catch (Exception ex)
            {
                var test = ex;
            }
        }

    }
}
