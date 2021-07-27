using AlgorithmLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrouterAlgorithm.Models
{
    public class CrouterCutpointDefinition
    {
        public List<CutPointDefinition> CutPointDefinitions => new List<CutPointDefinition>
        {
            // epoch values //
            new CutPointDefinition{ CutPointRenderingOption = CutPointEnums.CutPointRenderingOption.VerticalAxis, Name = CutPointEnums.CutPointBucketValues.Sedentary.ToString(),  MinimumValue = 0, MaximumValue = 35 },
            new CutPointDefinition{ CutPointRenderingOption = CutPointEnums.CutPointRenderingOption.VerticalAxis, Name = CutPointEnums.CutPointBucketValues.Light.ToString(), MinimumValue = 36 , MaximumValue = 360 },
            new CutPointDefinition{ CutPointRenderingOption = CutPointEnums.CutPointRenderingOption.VerticalAxis, Name = CutPointEnums.CutPointBucketValues.Moderate.ToString(), MinimumValue = 361, MaximumValue = 1129 },
            new CutPointDefinition{ CutPointRenderingOption = CutPointEnums.CutPointRenderingOption.VerticalAxis, Name = CutPointEnums.CutPointBucketValues.Vigorous.ToString(), MinimumValue = 1130, MaximumValue = double.MaxValue },
            // vm values //
            new CutPointDefinition{ CutPointRenderingOption = CutPointEnums.CutPointRenderingOption.VectorMagnitude, Name = CutPointEnums.CutPointBucketValues.Sedentary.ToString(),  MinimumValue = 0, MaximumValue = 100 },
            new CutPointDefinition{ CutPointRenderingOption = CutPointEnums.CutPointRenderingOption.VectorMagnitude, Name = CutPointEnums.CutPointBucketValues.Light.ToString(), MinimumValue = 101, MaximumValue = 609 },
            new CutPointDefinition{ CutPointRenderingOption = CutPointEnums.CutPointRenderingOption.VectorMagnitude, Name = CutPointEnums.CutPointBucketValues.Moderate.ToString(), MinimumValue = 610, MaximumValue = 1809 },
            new CutPointDefinition{ CutPointRenderingOption = CutPointEnums.CutPointRenderingOption.VectorMagnitude, Name = CutPointEnums.CutPointBucketValues.Vigorous.ToString(), MinimumValue = 1810, MaximumValue = double.MaxValue },
        };
    }
}
