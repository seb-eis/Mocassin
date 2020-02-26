using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents an unspecified pair interaction that describes the interaction between two positions depending on the
    ///     occupation
    /// </summary>
    public interface IPairInteraction : IModelObject
    {
        /// <summary>
        ///     The first reference unit cell position
        /// </summary>
        ICellReferencePosition Position0 { get; }

        /// <summary>
        ///     The second reference unit cell position
        /// </summary>
        ICellReferencePosition Position1 { get; }

        /// <summary>
        ///     The distance value between the positions in internal units
        /// </summary>
        double Distance { get; }

        /// <summary>
        ///     Get a boolean flag if the interaction is chiral
        /// </summary>
        bool IsChiral { get; }

        /// <summary>
        ///     Get a boolean flag if the interaction is symmetric 
        /// </summary>
        bool IsSymmetric { get; }

        /// <summary>
        ///     Get the <see cref="IPairInteraction" /> that is the L or R chiral partner
        /// </summary>
        IPairInteraction ChiralPartner { get; }

        /// <summary>
        ///     Get the actual position vector for the second unit cell position in order to describe the reference geometry of the
        ///     pair interaction
        /// </summary>
        /// <returns></returns>
        Fractional3D SecondPositionVector { get; }

        /// <summary>
        ///     Get all possible pair occupations of the pair with the corresponding energy value
        /// </summary>
        /// <returns></returns>
        IEnumerable<PairEnergyEntry> GetEnergyEntries();
    }
}