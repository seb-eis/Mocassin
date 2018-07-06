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
    public class BasicSymmetrySettings
    {
        /// <summary>
        /// The full filepath to the space group database
        /// </summary>
        [DataMember]
        public string SpaceGroupDbPath { get; set; }

        /// <summary>
        /// The tolerance value for equality comparisons of the vectors during wyckoff position extension
        /// </summary>
        [DataMember]
        public double VectorTolerance { get; set; }

        /// <summary>
        /// The tolerance value for equality comparisons of parameters in the cyrstal systems
        /// </summary>
        [DataMember]
        public double ParameterTolerance { get; set; }
    }
}
