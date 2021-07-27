using AlgorithmLibrary.Models;
using ChoiAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChoiAlgorithm.Service
{
    public interface IChoiWearTimeService
    {
        ChoiWearTimeResult CalculateWearPeriodsContinuous(IEnumerable<Epoch> epochData, ChoiWearTimeParameters options);
        ChoiWearTimeResult CalculateWearPeriodsNonContinuous(IEnumerable<Epoch> epochData, ChoiWearTimeParameters options);
    }
}
