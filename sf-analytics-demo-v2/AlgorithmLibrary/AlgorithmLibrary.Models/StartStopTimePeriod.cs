using System;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmLibrary.Models
{
    /// <summary>
    /// A range of time that can be used for wear times, bouts or anything else that needs a start and stop time
    /// </summary>
    public class StartStopTimePeriod
    {
        /// <summary>
        /// The start date and time of a period
        /// </summary>
        public DateTime Start { get; protected set; }

        /// <summary>
        /// The stop date and time of a period
        /// </summary>
        public DateTime Stop { get; protected set; }

        /// <summary>
        /// The length of the time period.
        /// Note: it's not just Stop - Start. We have to add one minute to Stop - Start since we are dealing with epoch data.
        /// </summary>
        public TimeSpan Length { get; protected set; }

        /// <summary>
        /// The default constructor for a start stop Period
        /// </summary>
        /// <param name="start">The start date and time of a period</param>
        /// <param name="stop">The stop date and time of a period</param>
        public StartStopTimePeriod(DateTime start, DateTime stop)
        {
            Start = start;
            Stop = stop;
            Length = Stop.Subtract(Start);
        }
    }
}
