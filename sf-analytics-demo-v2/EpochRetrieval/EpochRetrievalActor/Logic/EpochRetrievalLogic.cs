using AnalyticsJobsService.Models;
using EpochRetrieval.Models;
using EpochRetrievalActor.EpochSqlService;
using Global.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpochRetrievalActor.Logic
{
    public class EpochRetrievalLogic
    {
        private readonly IEpochSqlService _epochSqlService;
        public EpochRetrievalLogic(IEpochSqlService epochSqlService)
        {
            _epochSqlService = epochSqlService;
        }

        public async Task<(DateTime, DateTime)> ProcessEpochRetrievalTask(long subjectId, AlgorithmTaskDto[] epochAlgorithmTasks)
        {
            // get date range // 
            var timeRange = GetTimeRange(epochAlgorithmTasks.ToList());
            // query data // 
            var counts = await _epochSqlService.GetEpochsAsync(subjectId, timeRange.Item1, timeRange.Item2);
            var countsEnumerator = counts.GetEnumerator();
            // 5 seconds setup variables //
            List<EpochRecord> fiveSecondEpochs = new List<EpochRecord>();
            EpochRecord fiveSecondAggregateEpoch = null;
            int fiveSecondAggregateEpochLength = 0;

            // 60 seconds setup variables //
            List<EpochRecord> sixtySecondEpochs = new List<EpochRecord>();
            EpochRecord sixtySecondAggregateEpoch = null;
            int sixtySecondAggregateEpochLength = 0;


            // loop through data and pass it along to each worker

            while (countsEnumerator.MoveNext())
            {
                var epoch = countsEnumerator.Current;

                if (epoch == null)
                    continue;

                // five second //
                if (fiveSecondAggregateEpoch == null && sixtySecondAggregateEpoch == null)
                {
                    fiveSecondAggregateEpoch = epoch;
                    fiveSecondAggregateEpoch.TimestampUnixUtc = epoch.TimestampUnixUtc / 5 * 5;
                    fiveSecondAggregateEpochLength = 1;

                    sixtySecondAggregateEpoch = epoch;
                    sixtySecondAggregateEpoch.TimestampUnixUtc = epoch.TimestampUnixUtc / 60 * 60;
                    sixtySecondAggregateEpochLength = 1;

                    continue;
                }

                if (fiveSecondAggregateEpoch.TimestampUnixUtc != epoch.TimestampUnixUtc / 5 * 5)
                {
                    //Epoch aligned timestamp has changed, set new timestamp and yield current epoch
                    var returnEpoch = fiveSecondAggregateEpoch;
                    var returnEpochLength = fiveSecondAggregateEpochLength;

                    fiveSecondAggregateEpoch = epoch;
                    fiveSecondAggregateEpoch.TimestampUnixUtc = epoch.TimestampUnixUtc / 5 * 5;
                    fiveSecondAggregateEpochLength = 1;

                    if (returnEpochLength == 5)
                    {
                        fiveSecondEpochs.Add(returnEpoch);
                        if (fiveSecondEpochs.Count >= 17280) 
                        {
                           await BatchSendFiveSecondsEpochRecords(epochAlgorithmTasks, fiveSecondEpochs);
                           fiveSecondEpochs = new List<EpochRecord>();
                        }
                    }
                }
                else
                {
                    //Within epoch length, sum up values
                    fiveSecondAggregateEpoch.XAxisCounts += epoch.XAxisCounts;
                    fiveSecondAggregateEpoch.YAxisCounts += epoch.YAxisCounts;
                    fiveSecondAggregateEpoch.ZAxisCounts += epoch.ZAxisCounts;
                    fiveSecondAggregateEpochLength++;
                }

                if (sixtySecondAggregateEpoch.TimestampUnixUtc != epoch.TimestampUnixUtc / 60 * 60)
                {
                    //Epoch aligned timestamp has changed, set new timestamp and yield current epoch
                    var returnEpoch = sixtySecondAggregateEpoch;
                    var returnEpochLength = sixtySecondAggregateEpochLength;

                    sixtySecondAggregateEpoch = epoch;
                    sixtySecondAggregateEpoch.TimestampUnixUtc = epoch.TimestampUnixUtc / 60 * 60;
                    sixtySecondAggregateEpochLength = 1;

                    if (returnEpochLength == 60)
                    {
                        sixtySecondEpochs.Add(returnEpoch);
                        if (sixtySecondEpochs.Count >= 720)
                        {
                            await BatchSendSixtySecondsEpochRecords(epochAlgorithmTasks, sixtySecondEpochs);
                            sixtySecondEpochs = new List<EpochRecord>();
                        }
                    }
                }
                else
                {
                    // Within epoch length, sum up values
                    sixtySecondAggregateEpoch.XAxisCounts += epoch.XAxisCounts;
                    sixtySecondAggregateEpoch.YAxisCounts += epoch.YAxisCounts;
                    sixtySecondAggregateEpoch.ZAxisCounts += epoch.ZAxisCounts;
                    sixtySecondAggregateEpochLength++;
                }
            }

            // get the last one(s) if avalible // 
            if (sixtySecondAggregateEpochLength == 60)
                sixtySecondEpochs.Add(sixtySecondAggregateEpoch);

            if (fiveSecondAggregateEpochLength == 5)
                fiveSecondEpochs.Add(fiveSecondAggregateEpoch);

            if (fiveSecondEpochs.Any())
                await BatchSendFiveSecondsEpochRecords(epochAlgorithmTasks, fiveSecondEpochs);
            
            if (sixtySecondEpochs.Any())
                await BatchSendSixtySecondsEpochRecords(epochAlgorithmTasks, sixtySecondEpochs);

            fiveSecondEpochs = new List<EpochRecord>();
            sixtySecondEpochs = new List<EpochRecord>();

            return timeRange;
        }

        private async Task BatchSendFiveSecondsEpochRecords(AlgorithmTaskDto[] epochAlgorithmTasks, List<EpochRecord> fiveSecondEpochRecords) 
        {
            if (epochAlgorithmTasks.Any(x => x.AnalyticsTypeId == Global.Constants.AnalyticsTypes.CrouterChildCutpoints))
            {
                //create worker //
                var uniqueEpochSummaryTasks = epochAlgorithmTasks.Where(x => x.AnalyticsTypeId == Global.Constants.AnalyticsTypes.CrouterChildCutpoints);
                foreach (var epochSummaryTask in uniqueEpochSummaryTasks)
                {
                    var crouterWorker = FabricServices.GetCrouterWorkerActor(epochSummaryTask.Id);
                    await crouterWorker.AddEpochProcessingRecords(fiveSecondEpochRecords.ToArray());
                }
            }
        }

        private async Task BatchSendSixtySecondsEpochRecords(AlgorithmTaskDto[] epochAlgorithmTasks, List<EpochRecord> sixtySecondEpochRecords)
        {
            if (epochAlgorithmTasks.Any(x => x.AnalyticsTypeId == Global.Constants.AnalyticsTypes.EpochSummary))
            {
                //create worker //
                var uniqueEpochSummaryTasks = epochAlgorithmTasks.Where(x => x.AnalyticsTypeId == Global.Constants.AnalyticsTypes.EpochSummary);
                foreach (var epochSummaryTask in uniqueEpochSummaryTasks)
                {
                    var countsWorker = FabricServices.GetCountsWorkerActor(epochSummaryTask.Id);
                    await countsWorker.AddEpochProcessingRecords(sixtySecondEpochRecords.ToArray());
                }
            }
            if (epochAlgorithmTasks.Any(x => x.AnalyticsTypeId == Global.Constants.AnalyticsTypes.ChoiWearPeriods))
            {
                //create worker //
                var uniqueEpochSummaryTasks = epochAlgorithmTasks.Where(x => x.AnalyticsTypeId == Global.Constants.AnalyticsTypes.ChoiWearPeriods);
                foreach (var epochSummaryTask in uniqueEpochSummaryTasks)
                {
                    var choiWorker = FabricServices.GetChoiWorkerActor(epochSummaryTask.Id);
                    await choiWorker.AddEpochProcessingRecords(sixtySecondEpochRecords.ToArray());
                }
            }
            if (epochAlgorithmTasks.Any(x => x.AnalyticsTypeId == Global.Constants.AnalyticsTypes.DustinTracySleepPeriods))
            {
                //create worker //
                var uniqueEpochSummaryTasks = epochAlgorithmTasks.Where(x => x.AnalyticsTypeId == Global.Constants.AnalyticsTypes.DustinTracySleepPeriods);
                foreach (var epochSummaryTask in uniqueEpochSummaryTasks)
                {
                    var dustinTracyWorker = FabricServices.GetDustinTracyWorkerActor(epochSummaryTask.Id);
                    await dustinTracyWorker.AddEpochProcessingRecords(sixtySecondEpochRecords.ToArray());
                }
            }
        }

        private (DateTime, DateTime) GetTimeRange(List<AlgorithmTaskDto> epochAlgorithmTasks)
        {
            return (epochAlgorithmTasks.Min(x => x.AdjustedBeginTimestampUtc), epochAlgorithmTasks.Max(x => x.AdjustedEndTimestampUtc));
        }
    }
}
