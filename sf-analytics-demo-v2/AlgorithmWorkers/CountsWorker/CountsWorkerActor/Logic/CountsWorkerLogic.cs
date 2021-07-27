using CountsStorage.Interface;
using CountsStorage.Models;
using EpochRetrieval.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountsWorkerActor.Logic
{
    public class CountsWorkerLogic: ICountsWorkerLogic
    {
        private readonly ICountsStorageService _countsStorageService;
        private readonly int _maxBatchSize = 1000;

        public CountsWorkerLogic(ICountsStorageService countsStorageService) 
        {
            _countsStorageService = countsStorageService;
        }

        public async Task<int> ProcessCounts(EpochRecord[] epochRecords, long subjectId, string deviceId, Guid taskId, Guid settingsId) 
        {
            int itemsComputed = 0;
            var countsList = new List<CountsDto>();
            foreach (var epochRecord in epochRecords) 
            {
                // create a counts record //
                var storageDto = new CountsDto()
                {
                    XAxis = epochRecord.XAxisCounts,
                    YAxis = epochRecord.YAxisCounts,
                    ZAxis =epochRecord.ZAxisCounts,
                    TimestampUnixUtc = epochRecord.TimestampUnixUtc,
                };

                // add item to the counts worker queue //
                itemsComputed++;
                countsList.Add(storageDto);

                // batch up counts to send off // 
                if (countsList.Count <= _maxBatchSize) continue;
                
                var countsStorageDto = new CountsStorageDto()
                {
                    TaskId = taskId,
                    SettingsId = settingsId,
                    DeviceId = deviceId,
                    SubjectId = subjectId,
                    CountsDto = countsList.ToArray()
                };
                await _countsStorageService.EnqueueMessage(countsStorageDto);
                countsList = new List<CountsDto>();
            }

            if (countsList.Any()) 
            {
                var countsStorageDto = new CountsStorageDto()
                {
                    TaskId = taskId,
                    SettingsId = settingsId,
                    DeviceId = deviceId,
                    SubjectId = subjectId,
                    CountsDto = countsList.ToArray()
                };
                await _countsStorageService.EnqueueMessage(countsStorageDto);
            }

            return itemsComputed;
        }
    }
}
