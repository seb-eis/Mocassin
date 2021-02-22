using System;
using System.Runtime.Serialization;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Basic symmetry settings that contain information on space group and crystal system handling and databases
    /// </summary>
    [DataContract]
    public class MocassinSymmetrySettings
    {
        /// <summary>
        ///     The full filepath to the space group database
        /// </summary>
        [DataMember]
        public string SpaceGroupDbPath { get; set; } = SpaceGroupContextSource.DefaultDbPath;

        /// <summary>
        ///     The tolerance value for equality comparisons of the vectors during wyckoff position extension
        /// </summary>
        [DataMember]
        public double VectorTolerance { get; set; } = 1.0e-6;

        /// <summary>
        ///     The tolerance value for equality comparisons of parameters in the crystal systems
        /// </summary>
        [DataMember]
        public double ParameterTolerance { get; set; } = 1.0e-3;
    }
}