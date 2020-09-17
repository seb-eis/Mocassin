using System.Collections.Generic;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a energy setter for pair interactions that enables save manipulation of the pair interaction energy
    ///     dictionary
    /// </summary>
    public interface IPairEnergySetter : IValueSetter
    {
        /// <summary>
        ///     Get the number of interaction geometries emerging at each origin site after P1 translation
        /// </summary>
        int OriginInteractionCountPerSite { get; }

        /// <summary>
        ///     Get the number of interaction geometries emerging at each partner site after p1 translation
        /// </summary>
        int PartnerInteractionCountPerSite { get; }

        /// <summary>
        ///     Get a read only list of the current energy entries
        /// </summary>
        IReadOnlyCollection<PairEnergyEntry> EnergyEntries { get; }

        /// <summary>
        ///     Get access to the wrapped pair interaction that is manipulated by the setter
        /// </summary>
        IPairInteraction PairInteraction { get; }

        /// <summary>
        ///     Constraint for the energy values
        /// </summary>
        IValueConstraint<double, double> EnergyConstraint { get; }

        /// <summary>
        ///     Set an energy entry in the local energy list
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <returns></returns>
        void SetEnergyValue(PairEnergyEntry energyEntry);

        /// <summary>
        ///     Set multiple entries within the temporary energy entry list
        /// </summary>
        /// <param name="energyEntries"></param>
        /// <returns></returns>
        void SetEnergyValues(IEnumerable<PairEnergyEntry> energyEntries);
    }
}