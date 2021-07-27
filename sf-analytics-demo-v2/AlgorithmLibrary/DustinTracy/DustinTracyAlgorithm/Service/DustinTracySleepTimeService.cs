using AlgorithmLibrary.Models;
using DustinTracyAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DustinTracyAlgorithm.Service
{
    public class DustinTracySleepTimeService: IDustinTracySleepTimeService
    {
        private const long SecondsInMinute = 60;
        private long _dataStartTimeStamp;
        private bool _adjustLastSleepPeriod;

        public DustinTracySleepTimeResult CalculateSleepPeriodsContinuous(IEnumerable<Epoch> epochData, DustinTracyParameters options)
        {
            if (epochData != null && epochData.Any())
            {
                var sleepPeriods = CalculateSleepPeriods(epochData, options);
                return new DustinTracySleepTimeResult()
                {
                    SleepPeriods = ConvertSleepPeriods(sleepPeriods).ToArray(),
                    SleepPeriodStates = ConvertSleepPeriodStates(sleepPeriods, epochData).ToArray()
                };
            }

            return null;
        }

        private List<SleepPeriod> ConvertSleepPeriods(IEnumerable<DustinTracySleepTimeOutput> sleepPeriods) 
        {
            if (sleepPeriods.Any())
                return sleepPeriods.Select(x => new SleepPeriod 
                {
                    IsComplete = x.IsComplete,
                    SleepPeriodBegin = new DateTime(x.SleepPeriodBegin.Year, x.SleepPeriodBegin.Month, x.SleepPeriodBegin.Day, x.SleepPeriodBegin.Hour, x.SleepPeriodBegin.Minute, x.SleepPeriodBegin.Second),
                    SleepPeriodEnd = new DateTime(x.SleepPeriodEnd.Year, x.SleepPeriodEnd.Month, x.SleepPeriodEnd.Day, x.SleepPeriodEnd.Hour, x.SleepPeriodEnd.Minute, x.SleepPeriodEnd.Second)
                }).ToList();
            
            return new List<SleepPeriod>();
        }

        private List<SleepPeriodState> ConvertSleepPeriodStates(IEnumerable<DustinTracySleepTimeOutput> sleepPeriods, IEnumerable<Epoch> epochData) 
        {
            if (sleepPeriods.Any())
                return sleepPeriods.Select(x => new SleepPeriodState {
                    DataStartTimestamp = new DateTime(x.DataStartTimestamp.Year, x.DataStartTimestamp.Month, x.DataStartTimestamp.Day, x.DataStartTimestamp.Hour, x.DataStartTimestamp.Minute, x.DataStartTimestamp.Second),
                    DataEndTimestamp = new DateTime(x.DataEndTimestamp.Year, x.DataEndTimestamp.Month, x.DataEndTimestamp.Day, x.DataEndTimestamp.Hour, x.DataEndTimestamp.Minute, x.DataEndTimestamp.Second)
                }).ToList();
            else
                return new List<SleepPeriodState> { new SleepPeriodState() { DataStartTimestamp = epochData.OrderBy(x => x.TimestampUnixUtc).First().TimestampUtc, DataEndTimestamp = epochData.OrderBy(x => x.TimestampUnixUtc).Last().TimestampUtc } };
        }

        /// <summary>
        /// Calculate Sleep Periods
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="countRecords"></param>
        /// <param name="sleepParams"></param>
        /// <param name="actigraphMonitorTypeInfo"></param>
        /// <returns></returns>
        public IEnumerable<DustinTracySleepTimeOutput> CalculateSleepPeriods(IEnumerable<Epoch> countRecords, DustinTracyParameters sleepParams)
        {
            var sleepPeriodList = new List<DustinTracySleepTimeOutput>();
            _adjustLastSleepPeriod = false;
            // order count records //
            countRecords = countRecords.OrderBy(c => c.TimestampUnixUtc).ToList();
            // get contiguous time ranges //
            IEnumerable<Tuple<long, long>> ranges = this.GroupConsecutiveTimeStamps(countRecords.Select(c => c.TimestampUnixUtc));

            // Select only ranges where the time range is large enough for 2 block lengths //
            var contiguousRanges = ranges.Where(r => (r.Item2 - r.Item1) >= (sleepParams.BlockLengthInMinutes * 2));

            // loop through contiguous time ranges //
            foreach (var contiguousRange in contiguousRanges)
            {
                // Calculate average counts/ min in 1 - hour blocks
                var epochBuckets = countRecords
                    .Where(c => c.TimestampUnixUtc >= contiguousRange.Item1 && c.TimestampUnixUtc <= contiguousRange.Item2)
                    // group counts into buckets // 
                    .GroupBy(x =>
                    {
                        var blockStart = (x.TimestampUnixUtc / (sleepParams.BlockLengthInMinutes * SecondsInMinute)) * sleepParams.BlockLengthInMinutes * SecondsInMinute;
                        var blockGroup = x.TimestampUnixUtc - (x.TimestampUnixUtc - blockStart);
                        return blockGroup;
                    })
                    // select buckets for algorithm //
                    .Select(g => new DustinTracyBucket
                    {
                        TimeStamp = g.Key,
                        // avg the vertical axis for device //
                        Axis1Avg = g.Average(s => s.YAxisCounts), //default to Y //
                        Epochs = g.Select(x => new DustinTracyMinuteEpoch
                        {
                            TimeStamp = x.TimestampUnixUtc,
                            Axis1 = x.YAxisCounts, //default to Y //
                            EpochType = null
                        })
                        .OrderBy(dtme => dtme.TimeStamp)
                    })
                    .OrderBy(x => x.TimeStamp)
                    .ToList();

                var lastSleepEndTime = sleepPeriodList.LastOrDefault()?.SleepPeriodEnd.ToUnixTimeSeconds();
                long? tempBrStart = null;

                //Determine whether 1st block average is less than threshold
                if (epochBuckets[0].Axis1Avg < sleepParams.ThresholdCountsPerMinute)
                {
                    //Mark first epoch timestamp as temporary bedrest start time
                    tempBrStart = epochBuckets[0].Epochs.First().TimeStamp;
                    _dataStartTimeStamp = epochBuckets[0].TimeStamp;

                    //Checks for and sets a flag to handle overlap
                    if (lastSleepEndTime != null && tempBrStart <= lastSleepEndTime)
                    {
                        _dataStartTimeStamp = lastSleepEndTime.Value;
                        _adjustLastSleepPeriod = true;
                    }

                }

                // loop through the epoch buckets // 
                for (int i = 1; i < epochBuckets.Count; i++)
                {
                    lastSleepEndTime = sleepPeriodList.LastOrDefault()?.SleepPeriodEnd.ToUnixTimeSeconds();
                    //Do we have a temporary bed rest start time?l
                    if (tempBrStart == null)
                    {
                        //Find temporary bed rest start time
                        tempBrStart = FindSleepStart(sleepParams, epochBuckets, i);

                        //Checks for and sets a flag to handle overlap
                        if (lastSleepEndTime != null && tempBrStart <= lastSleepEndTime)
                        {
                            _dataStartTimeStamp = lastSleepEndTime.Value;
                            _adjustLastSleepPeriod = true;
                        }
                    }
                    else
                    {
                        //Find temporary bed rest end time
                        var tempBrEnd = FindSleepEnd(sleepParams, epochBuckets, i);

                        if (tempBrEnd == null)
                        {
                            continue;
                        }

                        //Handles overlapping sleep periods by updating the previous sleep periods end time
                        if (_adjustLastSleepPeriod && sleepPeriodList.LastOrDefault()?.SleepPeriodEnd != null)
                        {
                            // ReSharper disable once PossibleNullReferenceException - handled by conditional statement.
                            sleepPeriodList.LastOrDefault().SleepPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(tempBrEnd.Value);
                            _adjustLastSleepPeriod = false;
                        }
                        // Add complete sleep period  if //
                        //Is bed rest period greater than or equal to minimum bed rest length
                        else if ((tempBrEnd.Value - tempBrStart.Value) >= sleepParams.MinimumBedRestLengthInMinutes * 60)
                        {
                            sleepPeriodList.Add(new DustinTracySleepTimeOutput
                            {
                                IsComplete = true,
                                SleepPeriodBegin = DateTimeOffset.FromUnixTimeSeconds(tempBrStart.Value),
                                SleepPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(tempBrEnd.Value),
                                DataStartTimestamp = DateTimeOffset.FromUnixTimeSeconds(_dataStartTimeStamp)
                            });
                        }
                        // reset values // 
                        tempBrStart = null;
                        tempBrEnd = null;
                    }
                }

                // Add incomplete sleep period //
                if (tempBrStart.HasValue)
                {
                    var sleepEnd = epochBuckets.Last().Epochs.Last().TimeStamp;

                    //Handles edge case were last epoch minute (and only the last minute)is considered sleep.
                    //This makes sure that the in bed start does not come after the out bed time.
                    if (tempBrStart > sleepEnd)
                    {
                        tempBrStart = sleepEnd;
                    }

                    //handles overlap for partial sleep periods
                    if (sleepPeriodList.LastOrDefault() != null && lastSleepEndTime != null && tempBrStart <= lastSleepEndTime)
                    {
                        _dataStartTimeStamp = lastSleepEndTime.Value;

                        // ReSharper disable once PossibleNullReferenceException - null check performed by conditional statement.
                        sleepPeriodList.LastOrDefault().SleepPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(sleepEnd);
                        // ReSharper disable once PossibleNullReferenceException - null check performed by conditional statement
                        sleepPeriodList.LastOrDefault().IsComplete = false;
                        _adjustLastSleepPeriod = false;
                    }
                    else
                    {
                        sleepPeriodList.Add(new DustinTracySleepTimeOutput
                        {
                            SleepPeriodBegin = DateTimeOffset.FromUnixTimeSeconds(tempBrStart.Value),
                            //Mark sleep period as incomplete since we have a sleep period start but did not find a sleep period end
                            IsComplete = false,
                            //Set incomplete sleep period end to last epoch timestamp
                            SleepPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(sleepEnd),
                            DataStartTimestamp = DateTimeOffset.FromUnixTimeSeconds(_dataStartTimeStamp)
                        });
                    }
                }

            }
            return sleepPeriodList;
        }

        /// <summary>
        /// get consecutive timestamps for a range
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<Tuple<long, long>> GroupConsecutiveTimeStamps(IEnumerable<long> source)
        {
            List<Tuple<long, long>> ranges = new List<Tuple<long, long>>();
            using (var e = source.GetEnumerator())
            {
                for (bool more = e.MoveNext(); more;)
                {
                    long first = e.Current, last = first, next;
                    while ((more = e.MoveNext()) && (next = e.Current) > last && next - last == SecondsInMinute)
                        last = next;
                    ranges.Add(new Tuple<long, long>(first, last));
                }
            }

            return ranges;
        }

        /// <summary>
        /// looks for a change in the bucket avgs from sleep to awake
        /// using the ThresholdCountsPerMinute if found
        /// it flattens the list of sleep epochs from that bucket
        /// and the previous bucket into one list
        /// </summary>
        /// <param name="sleepParams"></param>
        /// <param name="epochBuckets"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private long? FindSleepEnd(DustinTracyParameters sleepParams,
            IReadOnlyList<DustinTracyBucket> epochBuckets, int i)
        {
            if (epochBuckets[i - 1].Axis1Avg < sleepParams.ThresholdCountsPerMinute &&
                epochBuckets[i].Axis1Avg >= sleepParams.ThresholdCountsPerMinute)
            {
                var flattenedListEpochs = epochBuckets[i - 1].Epochs.Concat(epochBuckets[i].Epochs).ToList();

                for (int j = 1; j < flattenedListEpochs.Count; j++)
                {
                    if (flattenedListEpochs[j - 1].Axis1 > sleepParams.BedRestEndTriggerCountsPerMinute &&
                        flattenedListEpochs[j].Axis1 > sleepParams.BedRestEndTriggerCountsPerMinute)
                    {
                        return flattenedListEpochs[j - 1].TimeStamp;
                    }
                }

                return flattenedListEpochs[flattenedListEpochs.Count - 1].TimeStamp;
            }

            return null;
        }

        /// <summary>
        /// looks for a change in the bucket avgs from awake to sleep
        /// using the ThresholdCountsPerMinute if found
        /// it flattens the list of sleep epochs from that bucket
        /// and the previous bucket into one list
        /// </summary>
        /// <param name="sleepParams"></param>
        /// <param name="epochBuckets"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private long? FindSleepStart(DustinTracyParameters sleepParams, IReadOnlyList<DustinTracyBucket> epochBuckets, int i)
        {
            if (epochBuckets[i - 1].Axis1Avg >= sleepParams.ThresholdCountsPerMinute &&
                epochBuckets[i].Axis1Avg < sleepParams.ThresholdCountsPerMinute)
            {
                //Set block start to start timestamp of the two blocks that we are currently calculating
                //This value is used to set the epoch range start when we page the data♥
                _dataStartTimeStamp = epochBuckets[i - 1].TimeStamp;

                var flattenedListEpochs = epochBuckets[i - 1].Epochs.Concat(epochBuckets[i].Epochs).ToList();
                for (int j = flattenedListEpochs.Count - 1; j > 0; j--)
                {
                    if (flattenedListEpochs[j - 1].Axis1 > sleepParams.BedRestStartTriggerCountsPerMinute &&
                        flattenedListEpochs[j].Axis1 > sleepParams.BedRestStartTriggerCountsPerMinute)
                    {
                        return flattenedListEpochs[j].TimeStamp + SecondsInMinute;
                    }
                }
                return flattenedListEpochs[0].TimeStamp;
            }

            return null;

        }
    }
}
