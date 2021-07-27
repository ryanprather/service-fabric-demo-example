using AlgorithmLibrary.Models;
using Itenso.TimePeriod;
using System;
using System.Collections.Generic;

namespace AlgorithmLibraryHelpers
{
    public static class TimePeriodHelpers
    {
        /// <summary>
        /// Get a list of the start and stop times where two list of periods conflict with each other.
        /// </summary>
        /// <param name="collection1">Collection of start and end times to compare.</param>
        /// <param name="collection2">Second collection of start and end times to compare.</param>
        /// <returns>Times where the collections don't match.</returns>
        public static ITimePeriodCollection GetConflictedPeriods(TimePeriodCollection collection1,
                                                                 TimePeriodCollection collection2)
        {
            //get conflicts of wtv compared to capsense
            //AND then get conflicts of capsense compared to wtv
            //you have to do it both ways because getting conflicts of wtv compared to capsense
            //only returns where there is WTV time but NO capsense time.
            //To get times where there's capsense and no WTV requires the opposite calculation.
            //see http://www.codeproject.com/Articles/168662/Time-Period-Library-for-NET
            //and look under the "Subtraction of Time Periods"

            if (collection1 == null)
                throw new ArgumentNullException("collection1");
            if (collection2 == null)
                throw new ArgumentNullException("collection2");

            ITimePeriodCollection allConflictPeriods = GetSubtractedTimes(collection1, collection2);
            allConflictPeriods.AddAll(GetSubtractedTimes(collection2, collection1));

            //let's sort these puppies
            allConflictPeriods.SortByStart();

            return allConflictPeriods;
        }

        /// <summary>
        /// Get a list of time periods that are in the sourcePeriods but not in the subtractingPeriods
        /// </summary>
        /// <param name="sourcePeriods">The periods to compare against.</param>
        /// <param name="subtractingPeriods">The periods that you want subtract from the source.</param>
        /// <returns>The differences between the two time period collection.</returns>
        public static ITimePeriodCollection GetSubtractedTimes(ITimePeriodCollection sourcePeriods,
                                                               ITimePeriodCollection subtractingPeriods)
        {
            if (sourcePeriods == null)
                throw new ArgumentNullException("sourcePeriods");
            if (subtractingPeriods == null)
                throw new ArgumentNullException("subtractingPeriods");

            TimePeriodSubtractor<TimeRange> subtractor = new TimePeriodSubtractor<TimeRange>();
            return subtractor.SubtractPeriods(sourcePeriods, subtractingPeriods);
        }

        /// <summary>
        /// Get a list of time periods that are in the sourcePeriods but not in the subtractingPeriods
        /// </summary>
        /// <param name="sourcePeriods">The periods to compare against.</param>
        /// <param name="subtractingPeriods">The periods that you want subtract from the source.</param>
        /// <returns>The differences between the two time period collection.</returns>
        public static ITimePeriodCollection GetSubtractedTimes<T>(IEnumerable<T> sourcePeriods,
                                                                  IEnumerable<T> subtractingPeriods)
            where T : StartStopTimePeriod
        {
            TimePeriodCollection source = new TimePeriodCollection();
            TimePeriodCollection subtract = new TimePeriodCollection();

            if (sourcePeriods != null)
                foreach (var sourceFilter in sourcePeriods)
                    source.Add(new TimeRange(sourceFilter.Start, sourceFilter.Stop));

            if (subtractingPeriods != null)
                foreach (var subtractingPeriod in subtractingPeriods)
                    subtract.Add(new TimeRange(subtractingPeriod.Start, subtractingPeriod.Stop));

            return GetSubtractedTimes(source, subtract);
        }


        /// <summary>
        /// Get a list of the start and stop times where two list of periods conflict with each other.
        /// </summary>
        /// <param name="collection1">Collection of start and end times to compare.</param>
        /// <param name="collection2">Second collection of start and end times to compare.</param>
        /// <returns>Times where the collections don't match.</returns>
        public static ITimePeriodCollection GetConflictedPeriods<T>(IEnumerable<T> collection1,
                                                                    IEnumerable<T> collection2)
            where T : StartStopTimePeriod
        {
            TimePeriodCollection firstCollection = new TimePeriodCollection();
            TimePeriodCollection secondCollection = new TimePeriodCollection();

            //add each type of filter to a collection
            if (collection1 != null)
                foreach (var timePeriod in collection1)
                    firstCollection.Add(new TimeRange(timePeriod.Start, timePeriod.Stop));

            if (collection2 != null)
                foreach (var timePeriod in collection2)
                    secondCollection.Add(new TimeRange(timePeriod.Start, timePeriod.Stop));

            return GetConflictedPeriods(firstCollection, secondCollection);
        }

        /// <summary>
        /// Using a list of start and end times, find only overlapping periods of time.
        /// </summary>
        /// <param name="allPeriods">The periods to investigate.</param>
        /// <returns>A group of start and end times.</returns>
        public static ITimePeriodCollection GetOverlapPeriods(TimePeriodCollection allPeriods)
        {
            if (allPeriods == null)
                throw new ArgumentNullException("allPeriods");
            TimePeriodIntersector<TimeRange> periodCombiner = new TimePeriodIntersector<TimeRange>();
            return periodCombiner.IntersectPeriods(allPeriods);
        }

        /// <summary>
        /// Determine if a start and end time is overlaps with a collection of periods
        /// </summary>
        /// <param name="allPeriods">The list of all periods to check.</param>
        /// <param name="range">The range of time you want to know whether is in the collection.</param>
        /// <returns>True if the time range is contained in the collection. False if it doesn't.</returns>
        public static bool IsPeriodInsideCollection(TimePeriodCollection allPeriods, TimeRange range)
        {
            TimePeriodCollection collection = new TimePeriodCollection(allPeriods);
            collection.Add(range);
            return GetOverlapPeriods(collection).Count > 0;
        }

        /// <summary>
        /// Using a list of start and end times, find only overlapping periods of time.
        /// </summary>
        /// <param name="filters">The periods to investigate.</param>
        /// <returns>A group of start and end times.</returns>
        public static ITimePeriodCollection GetOverlapPeriods(IEnumerable<StartStopTimePeriod> filters)
        {
            TimePeriodCollection allPeriods = new TimePeriodCollection();
            if (filters != null)
                foreach (var filter in filters)
                    allPeriods.Add(new TimeRange(filter.Start, filter.Stop));

            return GetOverlapPeriods(allPeriods);
        }

        /// <summary>
        /// Using a list of start and end times, combine all periods into a consolidated list.
        /// </summary>
        /// <param name="allPeriods">The periods to investigate.</param>
        /// <returns>A group of start and end times.</returns>
        public static ITimePeriodCollection GetCombinedPeriods(TimePeriodCollection allPeriods)
        {
            if (allPeriods == null)
                throw new ArgumentNullException("allPeriods");
            TimePeriodCombiner<TimeRange> periodCombiner = new TimePeriodCombiner<TimeRange>();
            return periodCombiner.CombinePeriods(allPeriods);
        }

        /// <summary>
        /// Using a list of start and end times, combine all periods into a consolidated list.
        /// </summary>
        /// <param name="filters">The periods to investigate.</param>
        /// <returns>A group of start and end times.</returns>
        public static ITimePeriodCollection GetCombinedPeriods<T>(IEnumerable<T> filters)
            where T : StartStopTimePeriod
        {
            TimePeriodCollection allPeriods = new TimePeriodCollection();
            if (filters != null)
                foreach (var filter in filters)
                    allPeriods.Add(new TimeRange(filter.Start, filter.Stop));

            return GetCombinedPeriods(allPeriods);
        }
    }
}
