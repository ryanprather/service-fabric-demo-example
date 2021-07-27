using AlgorithmLibrary.Models;
using CrouterAlgorithm.Models;
using CrouterAlgorithm.Service;
using System;
using System.Linq;

namespace CrouterAlgorithm
{
    public class CrouterCutpointService: ICrouterCutpointService
    {
        private static CrouterCutpointDefinition _crouterCutpointDefinition = new CrouterCutpointDefinition();

        public CrouterCutpointResult CalculateCrouterCutpoint(Epoch epoch) 
        {
            var cutpointResult = new CrouterCutpointResult();
            try 
            {
                // assign cutpoint bucket va // 
                cutpointResult.VerticalAxisResult = _crouterCutpointDefinition.CutPointDefinitions
                    .First(cpd => cpd.CutPointRenderingOption == CutPointEnums.CutPointRenderingOption.VerticalAxis
                    && cpd.MinimumValue <= epoch.YAxisCounts && cpd.MaximumValue >= epoch.YAxisCounts).Name;

                // assign cutpoint bucket vm // 
                cutpointResult.VectorMagnitudeAxisResult = _crouterCutpointDefinition.CutPointDefinitions
                    .First(cpd => cpd.CutPointRenderingOption == CutPointEnums.CutPointRenderingOption.VectorMagnitude
                    && cpd.MinimumValue <= Math.Round(epoch.VectorMagintude, MidpointRounding.AwayFromZero) && cpd.MaximumValue >= Math.Round(epoch.VectorMagintude, MidpointRounding.AwayFromZero)).Name;

            }
            catch (Exception ex) 
            {
                var test = ex;
                throw;
            }
            
            return cutpointResult;
        }
    }
}
