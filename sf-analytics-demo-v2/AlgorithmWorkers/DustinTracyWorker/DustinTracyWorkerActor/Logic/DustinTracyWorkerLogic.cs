using AlgorithmStateWorker.Interface;
using AlgorithmStateWorker.Models;
using DustinTracyAlgorithm.Models;
using DustinTracyAlgorithm.Service;
using DustinTracyStorage.Interface;
using DustinTracyStorage.Models;
using EpochRetrieval.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DustinTracyWorkerActor.Logic
{
    public class DustinTracyWorkerLogic : IDustinTracyWorkerLogic
    {
        private readonly IDustinTracySleepTimeService _dustinTracySleepTimeService;
        private readonly IDustinTracyStorageService _dustinTracyStorageService;
        private readonly IAlgorithmStateWorkerService _algorithmStateWorkerService;


        public DustinTracyWorkerLogic(IDustinTracySleepTimeService dustinTracySleepTimeService, IAlgorithmStateWorkerService algorithmStateWorkerService,  IDustinTracyStorageService dustinTracyStorageService)
        {
            _dustinTracySleepTimeService = dustinTracySleepTimeService;
            _dustinTracyStorageService = dustinTracyStorageService;
            _algorithmStateWorkerService = algorithmStateWorkerService;
        }

        public async Task<int> ProcessRecords(EpochRecord[] epochRecords, long subjectId, string deviceId, string settings, Guid taskId, Guid settingsId)
        {
            // convert settings to correct settings //
            var options = JsonConvert.DeserializeObject<DustinTracyParameters>(settings);
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
            var result = _dustinTracySleepTimeService.CalculateSleepPeriodsContinuous(epochModels, options);

            if (result != null && result.SleepPeriods.Any())
            {
                // convert and add to storage queue for dustin tracy sleep periods // 
                var storageDto = new DustinTracyStorageDto()
                {
                    SubjectId = subjectId,
                    DeviceId = deviceId,
                    SettingsId = settingsId,
                    TaskId = taskId,
                    SleepPeriods = result.SleepPeriods.Select(x =>
                        new DustinTracySleepPeriodDto()
                        {
                            BeginTimeUtc = x.SleepPeriodBegin,
                            EndTimeUtc = x.SleepPeriodEnd
                        }).ToArray()
                };
                await _dustinTracyStorageService.EnqueueMessage(storageDto);

                // convert and store states for dustin tracy sleep periods //  
                var stateDto = result.SleepPeriodStates.Select(x => new DustinTracyStateDto
                {
                    SubjectId = subjectId,
                    SettingsId = settingsId,
                    DataStartTimestamp = x.DataStartTimestamp,
                });
                await _algorithmStateWorkerService.StoreDustinTracySleepPeriodState(stateDto.ToArray());
            }
            else 
            {
                return 0;
            }
            
            //TODO: send next start date back //  
            return result.SleepPeriods.Length;
        }
    }
}
