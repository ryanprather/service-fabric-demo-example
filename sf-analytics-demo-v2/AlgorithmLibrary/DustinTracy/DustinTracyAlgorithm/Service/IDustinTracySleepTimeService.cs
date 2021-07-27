using AlgorithmLibrary.Models;
using DustinTracyAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DustinTracyAlgorithm.Service
{
    public interface IDustinTracySleepTimeService
    {
        DustinTracySleepTimeResult CalculateSleepPeriodsContinuous(IEnumerable<Epoch> epochData, DustinTracyParameters options);
    }
}
