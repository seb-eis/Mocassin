using System.Collections.Generic;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents a group interaction to describe a more complex (No pair interaction) refernce grouping around a position
    /// </summary>
    public interface IGroupInteraction : IModelObject
    {
        /// <summary>
        /// The size of the group (Always larger than 2, max 8)
        /// </summary>
        int GroupSize { get; }

        /// <summary>
        /// Get the unit cell position this grouping starts from
        /// </summary>
        IUnitCellPosition UnitCellPosition { get; }

        /// <summary>
        /// Get a sequence of 3D vectors that describe the base geometry of the grouping without the start position
        /// </summary>
        /// <returns></returns>
        IEnumerable<Fractional3D> GetBaseGeometry();

        /// <summary>
        /// Get a read only dictionary that assigns each possible center particle an energy dictionary for the outer permutation
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<IParticle, IReadOnlyDictionary<OccupationState, double>> GetEnergyDictionarySet();
    }
}
