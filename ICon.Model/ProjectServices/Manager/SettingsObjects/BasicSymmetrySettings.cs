using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Basic symmetry settings that contain information on space group and crystal system handling and databases
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicSymmetrySettings
    {
        /// <summary>
        /// The full filepath to the space group database
        /// </summary>
        [DataMember(Name ="Database")]
        public String DatabaseFilepath { get; set; }

        /// <summary>
        /// The tolerance value for equality comparisons of the vectors during wyckoff position extension
        /// </summary>
        [DataMember(Name ="VectorTolerance")]
        public Double VectorTolerance { get; set; }

        /// <summary>
        /// The tolerance value for equality comparisons of parameters in the cyrstal systems
        /// </summary>
        [DataMember(Name ="ParameterTolerance")]
        public Double ParameterTolerance { get; set; }
    }
}
