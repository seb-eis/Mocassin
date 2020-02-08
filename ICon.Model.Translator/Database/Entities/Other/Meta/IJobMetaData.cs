namespace Mocassin.Model.Translator.Database.Entities.Other.Meta
{
    /// <summary>
    ///     Represents a set of meta data for simulation jobs
    /// </summary>
    public interface IJobMetaData
    {
        /// <summary>
        ///     Get or set the source job collection name
        /// </summary>
        string CollectionName { get; set; }

        /// <summary>
        ///     Get or set the job configuration name
        /// </summary>
        string ConfigName { get; set; }

        /// <summary>
        ///     Get or set the relative job index due to job multiplication in the configuration
        /// </summary>
        int JobIndex { get; set; }

        /// <summary>
        ///     Get or set the config index in the collection
        /// </summary>
        int ConfigIndex { get; set; }

        /// <summary>
        ///     Get or set the job collection index
        /// </summary>
        int CollectionIndex { get; set; }

        /// <summary>
        ///     Get or set the thermodynamic temperature
        /// </summary>
        double Temperature { get; set; }

        /// <summary>
        ///     Get or set the electric field modulus in [V/m]
        /// </summary>
        double ElectricFieldModulus { get; set; }

        /// <summary>
        ///     Get or set the base frequency in [Hz]
        /// </summary>
        double BaseFrequency { get; set; }

        /// <summary>
        ///     Get or set the main run target MCSP
        /// </summary>
        long Mcsp { get; set; }

        /// <summary>
        ///     Get or set the pre-run run target MCSP
        /// </summary>
        long PreRunMcsp { get; set; }

        /// <summary>
        ///     Get or set the fixed manual normalization
        /// </summary>
        double NormalizationFactor { get; set; }

        /// <summary>
        ///     Get or set the time limit in seconds
        /// </summary>
        long TimeLimit { get; set; }

        /// <summary>
        ///     Get or set the job flags
        /// </summary>
        string FlagString { get; set; }

        /// <summary>
        ///     Get or set a doping information <see cref="string" />
        /// </summary>
        string DopingInfo { get; set; }

        /// <summary>
        ///     Get or set a lattice information <see cref="string" />
        /// </summary>
        string LatticeInfo { get; set; }
    }
}