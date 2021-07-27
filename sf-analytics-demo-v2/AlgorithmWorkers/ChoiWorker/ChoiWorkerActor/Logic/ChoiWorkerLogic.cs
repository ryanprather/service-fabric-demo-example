using AlgorithmStateWorker.Interface;
using AlgorithmStateWorker.Models;
using ChoiAlgorithm.Models;
using ChoiAlgorithm.Service;
using ChoiStorage.Interface;
using ChoiStorage.Models;
using EpochRetrieval.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChoiWorkerActor.Logic
{
    public class ChoiWorkerLogic : IChoiWorkerLogic
    {
        private readonly IChoiWearTimeService _choiWearTimeService;
        private readonly IChoiStorageService _choiStorageService;
        private readonly IAlgorithmStateWorkerService _algorithmStateWorkerService;

        public ChoiWorkerLogic(IChoiWearTimeService choiWearTimeService, IAlgorithmStateWorkerService algorithmStateWorkerService, IChoiStorageService choiStorageService)
        {
            _choiWearTimeService = choiWearTimeService;
            _choiStorageService = choiStorageService;
            _algorithmStateWorkerService = algorithmStateWorkerService;
        }

        public async Task<int> ProcessRecords(EpochRecord[] epochRecords, long subjectId, string deviceId, string settings, Guid taskId, Guid settingsId)
        {
            // convert settings to correct settings //
            var choiOptions = JsonConvert.DeserializeObject<ChoiWearTimeParameters>(settings);
            // convert epochs to correct settings //
            var epochModels = epochRecords.Select(x =>
                new AlgorithmLibrary.Models.Epoch()
                {
                    TimestampUnixUtc = x.TimestampUnixUtc,
                    XAxisCounts = x.XAxisCounts,
                    YAxisCounts = x.YAxisCounts,
                    ZAxisCounts = x.ZAxisCounts
                });
            // calcuate algorithm //
            var result = _choiWearTimeService.CalculateWearPeriodsContinuous(epochModels, choiOptions);

            if (result != null && result.WearPeriods.Any())
            {
                // convert and add to storage queue for choi wear periods // 
                var storageDto = new ChoiStorageDto()
                {
                    TaskId = taskId,
                    SubjectId = subjectId,
                    DeviceId = deviceId,
                    SettingsId = settingsId,
                    WearPeriods = result.WearPeriods.Select(x =>
                        new ChoiWearPeriodDto()
                        {
                            BeginTimeUtc = x.StartDateTimeUtc,
                            EndTimeUtc = x.EndDateTimeUtc
                        }).ToArray()
                };
                await _choiStorageService.EnqueueMessage(storageDto);

                // convert and store state for wear periods //
                var stateDto = result.NextStartTime.Select(x => new ChoiStateDto()
                {
                    SubjectId = subjectId,
                    SettingsId = settingsId,
                    DataStartTimestamp = x,
                }).ToArray();
                await _algorithmStateWorkerService.StoreChoiWearPeriodState(stateDto);
            }
            else 
            {
                return 0;
            }

            return result.WearPeriods.Length;
        }
    }
}
