using AlgorithmLibrary.Models;
using CrouterAlgorithm.Service;
using CrouterStorage.Interface;
using CrouterStorage.Models;
using EpochRetrieval.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrouterWorkerActor.Logic
{
    public class CrouterWorkerLogic: ICrouterWorkerLogic
    {
        private readonly ICrouterCutpointService _crouterCutpointService;
        private readonly ICrouterStorageService _crouterStorageService;
        private readonly int _maxBatchSize = 10000;
        public CrouterWorkerLogic(ICrouterCutpointService crouterCutpointService, ICrouterStorageService crouterStorageService)
        {
            _crouterCutpointService = crouterCutpointService;
            _crouterStorageService = crouterStorageService;
        }

        public async Task<int> ProcessCounts(EpochRecord[] epochRecords, long subjectId, string deviceId, Guid taskId, Guid settingsId)
        {
            int itemsComputed = 0;
            var cutpointList = new List<CrouterDto>();

            foreach (var epochRecord in epochRecords)
            {
                // create a counts record //
                var algEpochModel = new Epoch() 
                {
                    TimestampUnixUtc = epochRecord.TimestampUnixUtc,
                    XAxisCounts = epochRecord.XAxisCounts,
                    YAxisCounts = epochRecord.YAxisCounts,
                    ZAxisCounts = epochRecord.ZAxisCounts
                };
                var result = _crouterCutpointService.CalculateCrouterCutpoint(algEpochModel);

                // create a counts record //
                var storageDto = new CrouterDto()
                {
                    VerticalAxisResult = result.VerticalAxisResult,
                    VectorMagnitudeAxisResult = result.VectorMagnitudeAxisResult,
                    TimestampUtc = epochRecord.TimestampUnixUtc,
                };

                // add item to the counts worker queue //
                itemsComputed++;
                cutpointList.Add(storageDto);

                // batch up cutpoints to send off // 
                if (cutpointList.Count <= _maxBatchSize) continue;

                var countsStorageDto = new CrouterStorageDto()
                {
                    TaskId = taskId,
                    SettingsId = settingsId,
                    DeviceId = deviceId,
                    SubjectId = subjectId,
                    CrouterDto = cutpointList.ToArray()
                };
                await _crouterStorageService.EnqueueMessage(countsStorageDto);
                cutpointList = new List<CrouterDto>();
            }

            if (cutpointList.Any()) 
            {
                var countsStorageDto = new CrouterStorageDto()
                {
                    TaskId = taskId,
                    SettingsId = settingsId,
                    DeviceId = deviceId,
                    SubjectId = subjectId,
                    CrouterDto = cutpointList.ToArray()
                };
                await _crouterStorageService.EnqueueMessage(countsStorageDto);
            }

            return itemsComputed;
        }
    }
}
