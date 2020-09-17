using System.Collections.Generic;
using Mocassin.Framework.Constraints;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents an energy setter provider that supplies the setter adapters to savely manipulate interaction energies
    /// </summary>
    public interface IEnergySetterProvider
    {
        /// <summary>
        ///     Get or set the value constraint for the pair energies
        /// </summary>
        IValueConstraint<double, double> PairEnergyConstraint { get; set; }

        /// <summary>
        ///     Get or set the value constraint for the group energies
        /// </summary>
        IValueConstraint<double, double> GroupEnergyConstraint { get; set; }

        /// <summary>
        ///     Get a read only list of all stable pair interaction energy setters
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IPairEnergySetter> GetStablePairEnergySetters();

        /// <summary>
        ///     Get the stable pair interaction energy setter for the interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPairEnergySetter GetStablePairEnergySetter(int index);

        /// <summary>
        ///     Get a read only list of all unstable pair interaction energy setters
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IPairEnergySetter> GetUnstablePairEnergySetters();

        /// <summary>
        ///     Get the unstable pair interaction energy setter for the interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPairEnergySetter GetUnstablePairEnergySetter(int index);

        /// <summary>
        ///     Get a read only list of all energy setters for group interactions
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IGroupEnergySetter> GetGroupEnergySetters();

        /// <summary>
        ///     Get the group interaction energy setter for the group interaction at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGroupEnergySetter GetGroupEnergySetter(int index);
    }
}