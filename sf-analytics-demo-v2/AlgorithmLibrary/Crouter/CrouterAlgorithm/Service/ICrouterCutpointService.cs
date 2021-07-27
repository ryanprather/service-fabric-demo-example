using AlgorithmLibrary.Models;
using CrouterAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrouterAlgorithm.Service
{
    public interface ICrouterCutpointService
    {
        CrouterCutpointResult CalculateCrouterCutpoint(Epoch epoch);
    }
}
