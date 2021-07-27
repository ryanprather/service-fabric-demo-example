using System;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmLibrary.Models
{
    public class CutPointDefinition
    {
        /// <summary>
        /// name of the cutpoint bucket sedentary, light etc.. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// what type rendering should be used for the cutpoint 
        /// </summary>
        public CutPointEnums.CutPointRenderingOption CutPointRenderingOption { get; set; }

        /// <summary>
        /// Min Value of the cutpoint 
        /// </summary>
        public double MinimumValue { get; set; }

        /// <summary>
        ///  Value of the cutpoint
        /// </summary>
        public double? MaximumValue { get; set; }
    }
}
