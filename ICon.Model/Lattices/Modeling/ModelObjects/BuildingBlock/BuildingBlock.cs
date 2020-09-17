using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Building Block for the lattice. Each building block has the size of the unit cell.
    /// </summary>
    public class BuildingBlock : ModelObject, IBuildingBlock
    {
        /// <summary>
        ///     The occupation of the building block
        /// </summary>
        [UseTrackedData]
        public List<IParticle> CellEntries { get; set; }

        /// <inheritdoc />
        IReadOnlyList<IParticle> IBuildingBlock.CellEntries => CellEntries.AsReadOnly();

        /// <inheritdoc />
        /// <summary>
        ///     Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string ObjectName => "Building Block";

        /// <summary>
        ///     Copies the information from the provided model object interface and returns the object (Retruns null if type
        ///     mismatch)
        /// </summary>
        /// <param name="modelObject"></param>
        public override ModelObject PopulateFrom(IModelObject modelObject)
        {
            if (!(CastIfNotDeprecated<IBuildingBlock>(modelObject) is { } buildingBlock)) return null;
            CellEntries = buildingBlock.CellEntries.ToList();
            return this;
        }
    }
}