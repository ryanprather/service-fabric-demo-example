using AlgorithmLibrary.Models;
using AlgorithmLibraryHelpers;
using ChoiAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChoiAlgorithm.Service
{
    public class ChoiWearTimeService: IChoiWearTimeService
    {
        private int index = 0;
        private int epochLength = 60;

        public ChoiWearTimeResult CalculateWearPeriodsContinuous(IEnumerable<Epoch> epochData, ChoiWearTimeParameters options) 
        {
            if (epochData != null && epochData.Any()) 
            {
                var wearPeriods = CalculateWearPeriods(epochData, options);
                return new ChoiWearTimeResult() { WearPeriods = ConvertWearPeriods(wearPeriods), NextStartTime = GetNextStartDateTime(wearPeriods, epochData).ToArray() };
            }

            return null;            
        }

        public ChoiWearTimeResult CalculateWearPeriodsNonContinuous(IEnumerable<Epoch> epochData, ChoiWearTimeParameters options) 
        {
            var wearPeriods = CalculateWearPeriods(epochData, options);
            return new ChoiWearTimeResult() {WearPeriods = ConvertWearPeriods(wearPeriods), NextStartTime = null };
        }

        private ChoiWearPeriod[] ConvertWearPeriods(IEnumerable<StartStopTimePeriod> startStopTimePeriods) 
        {
            return startStopTimePeriods
                .Select(x => new ChoiWearPeriod() { StartDateTimeUtc = x.Start, EndDateTimeUtc = x.Stop })
                .OrderBy(x => x.StartDateTimeUtc)
                .ToArray();
        }

        private List<DateTime> GetNextStartDateTime(IEnumerable<StartStopTimePeriod> startStopTimePeriods, IEnumerable<Epoch> epochData) 
        {
            if (startStopTimePeriods.Any())
                return startStopTimePeriods.OrderByDescending(x => x.Start).Select(x=>x.Stop).ToList();
            else
                return new List<DateTime> { epochData.OrderBy(x => x.TimestampUnixUtc).First().TimestampUtc };
        }

        private IEnumerable<StartStopTimePeriod> CalculateWearPeriods(IEnumerable<Epoch> epochData, ChoiWearTimeParameters options)
        {
            //Since Choi algorithm does not properly use an IEnumerable<MinuteEpoch>
            //go ahead and convert to list to avoid performance issues
            var epochDataList = epochData.ToList();

            if (!epochDataList.Any())
            {
                return new List<StartStopTimePeriod>();
            }

            var startDate = epochDataList.First().TimestampUtc;
            var endDate = epochDataList.Last().TimestampUtc;
            var range = new List<StartStopTimePeriod>(1);
            range.Add(new StartStopTimePeriod(startDate, endDate.AddMinutes(1))); //Add 1 minute to end range (inclusive)
            IEnumerable<StartStopTimePeriod> nonWearPeriods = RetrieveNonWearPeriods(epochDataList, options);

            var wearTimePeriods = TimePeriodHelpers.GetSubtractedTimes(range, nonWearPeriods);
            var wearPeriods = new List<StartStopTimePeriod>(10);
            foreach (var wearPeriod in wearTimePeriods)
            {
                wearPeriods.Add(new StartStopTimePeriod(wearPeriod.Start, wearPeriod.End));
            }
            return wearPeriods;
        }

        public IEnumerable<StartStopTimePeriod> RetrieveNonWearPeriods(List<Epoch> epochData, ChoiWearTimeParameters options)
        {
            int totalCount = epochData.Count;
            ChoiWearTimeParameters choiOptions = options as ChoiWearTimeParameters;
            index = 0;

            if (totalCount < choiOptions.MinimumLengthInMinutes)
                yield break;

            //How to calculate Choi WTV Algorithm:
            //1. find next smallWindowLengthInMinutes minutes of consecutive minutes
            //2. read until non-zero
            //3. read allowanceFrameLengthInMinutes minutes of data and note the last non-zero
            //4. see if we have smallWindowLengthInMinutes minutes of zeros from last non-zero in step 3
            //5. if no, see if we have a valid bout and set it to right before the non-zero from step 2 and then go to step 1
            //   if yes, go to step 2

            //Non-wear bout must be at least largeWindowLengthInMinutes minutes
            //If AGD is less than 60 seconds, we need to reintegrate file to 60 seconds
            //This can be done on the fly to help speed up processing

            DateTime startOfNonWearPeriod = choiOptions.StartOfNonWearPeriod ?? DateTime.MaxValue;
            DateTime lastEpochTimestamp = epochData.First().TimestampUtc;
            DateTime CurrentTimestamp = lastEpochTimestamp;
            int epochLength = 60;
            do
            {
                //step 1: find next smallWindowLengthInMinutes minutes of consecutive minutes
                if (startOfNonWearPeriod == DateTime.MaxValue)
                {
                    DateTime nextSmall = FindNextSmallWindowOfZeros(epochData, choiOptions, CurrentTimestamp, index, totalCount);
                    if (nextSmall != DateTime.MinValue)
                        startOfNonWearPeriod = nextSmall;

                    if (nextSmall == DateTime.MinValue)
                        break;

                    lastEpochTimestamp = nextSmall;
                }

                //step 2: read until non-zero
                double count = 0;
                //Start New
                while (count == 0 && HasEpochsAvailable(index, totalCount))
                {
                    var epoch = epochData.ElementAt(index);//TODO: find better away to iterate through this list
                    if (CurrentTimestamp < epoch.TimestampUtc)
                    {
                        var numberOfMinuteDifference = (int)(epoch.TimestampUtc - CurrentTimestamp).TotalMinutes;
                        CurrentTimestamp = CurrentTimestamp.AddSeconds(epochLength * numberOfMinuteDifference);
                    }
                    lastEpochTimestamp = epoch.TimestampUtc;
                    count = options.UseVM ? epoch.VectorMagintude : epoch.YAxisCounts;
                    index++;
                    CurrentTimestamp = CurrentTimestamp.AddSeconds(epochLength);
                }

                DateTime endOfNonWearPeriod;
                if (HasEpochsAvailable(index, totalCount) == false)
                {
                    //we are at least smallWindowLengthInMinutes since the last count
                    //and we have just hit the end of the available epochs
                    //if the last count was a zero, use that timestamp as the end of non-wear period
                    //if it wasn't a zero, use the previous timestamp since that is guaranteed to be a zero (i think)
                    endOfNonWearPeriod = count == 0 ? lastEpochTimestamp : lastEpochTimestamp.AddMinutes(-1);

                    var nonWearPeriod = DetermineIfValidBoutFromStartAndStopTime(choiOptions, startOfNonWearPeriod, endOfNonWearPeriod);
                    if (nonWearPeriod != null) yield return nonWearPeriod;
                    break;
                }

                //step 3: read allowanceFrameLengthInMinutes minutes of data and note the last non-zero
                DateTime lastNonZero = lastEpochTimestamp;
                DateTime currentEpoch = lastEpochTimestamp;
                for (int i = 0; i < choiOptions.SpikeToleranceInMinutes - 1; i++)
                {
                    //add here
                    var epoch = epochData.ElementAt(index);

                    count = options.UseVM ? epoch.VectorMagintude : epoch.YAxisCounts;
                    if (CurrentTimestamp < epoch.TimestampUtc)
                    {
                        count = 0;
                        currentEpoch = CurrentTimestamp;
                    }
                    else
                    {
                        index++;
                        currentEpoch = epoch.TimestampUtc;
                    }
                    CurrentTimestamp = CurrentTimestamp.AddSeconds(epochLength);
                    if (count > 0)
                        lastNonZero = epoch.TimestampUtc;

                    if (HasEpochsAvailable(index, totalCount) == false)
                        break;
                }

                if (HasEpochsAvailable(index, totalCount) == false)
                {
                    //we have only read allowanceFrameLengthInMinutes so we need to ignore that time
                    //just use the lastEpochTimestamp minus 1 minute
                    endOfNonWearPeriod = lastEpochTimestamp.AddMinutes(-1);
                    var nonWearPeriod = DetermineIfValidBoutFromStartAndStopTime(choiOptions, startOfNonWearPeriod, endOfNonWearPeriod);
                    if (nonWearPeriod != null) yield return nonWearPeriod;
                    break;
                }

                //step 4: see if we have smallWindowLengthInMinutes minutes of zeros from last non-zero in step 3
                int minutesToRead = choiOptions.SmallWindowLengthInMinutes;
                if (currentEpoch != lastNonZero)
                    minutesToRead = (int)(choiOptions.SmallWindowLengthInMinutes - (currentEpoch.Subtract(lastNonZero).TotalMinutes));

                int minutesRead = 0;
                for (int i = 0; i < minutesToRead; i++)
                {
                    var epoch = epochData.ElementAt(index);
                    count = options.UseVM ? epoch.VectorMagintude : epoch.YAxisCounts;
                    if (CurrentTimestamp < epoch.TimestampUtc)
                    {
                        count = 0;
                    }
                    else index++;
                    CurrentTimestamp = CurrentTimestamp.AddSeconds(epochLength);
                    if (count > 0)
                        break;
                    minutesRead++;
                    if (HasEpochsAvailable(index, totalCount) == false)
                        break;
                }

                //step 5: 
                //if no, see if we have a valid bout and set it to right before the non-zero from step 2 and then go to step 1
                //if yes, if yes, go to step 2
                if (minutesRead != minutesToRead)
                {
                    endOfNonWearPeriod = lastEpochTimestamp.AddMinutes(-1);
                    var nonWearPeriod = DetermineIfValidBoutFromStartAndStopTime(choiOptions, startOfNonWearPeriod, endOfNonWearPeriod);
                    if (nonWearPeriod != null) yield return nonWearPeriod;
                    startOfNonWearPeriod = DateTime.MaxValue;
                }
            } while (true);
        }

        public DateTime FindNextSmallWindowOfZeros(List<Epoch> epochData, ChoiWearTimeParameters options, DateTime currentTimestamp, int index, int totalCount)
        {
            int zeroCounter = 0;
            DateTime startOfPeriod = DateTime.MinValue;
            while (zeroCounter < options.SmallWindowLengthInMinutes && HasEpochsAvailable(index, totalCount))
            {
                var epoch = epochData.ElementAt(index);
                index++;
                double count = options.UseVM ? epoch.VectorMagintude : epoch.YAxisCounts;
                if (currentTimestamp < epoch.TimestampUtc)
                {
                    count = 0;
                    if (startOfPeriod == DateTime.MinValue)
                        startOfPeriod = currentTimestamp;
                }
                if (count == 0)
                {
                    zeroCounter++;
                    if (startOfPeriod == DateTime.MinValue)
                        startOfPeriod = epoch.TimestampUtc;
                }
                else
                {
                    zeroCounter = 0;
                    //reset start of period
                    startOfPeriod = DateTime.MinValue;
                }
                currentTimestamp = currentTimestamp.AddSeconds(epochLength);
            }

            if (zeroCounter >= options.SmallWindowLengthInMinutes)
                return startOfPeriod;
            return DateTime.MinValue;

        }

        public StartStopTimePeriod DetermineIfValidBoutFromStartAndStopTime(ChoiWearTimeParameters options, DateTime start, DateTime stop)
        {
            if (stop.AddMinutes(1).Subtract(start).TotalMinutes >= options.MinimumLengthInMinutes)
            {
                return new StartStopTimePeriod(start, stop.AddMinutes(1)); //Add 1 minute for Inclusive range
            }
            return null;
        }

        public bool HasEpochsAvailable(int index, int count)
        {
            return index < count;
        }
    }
}
