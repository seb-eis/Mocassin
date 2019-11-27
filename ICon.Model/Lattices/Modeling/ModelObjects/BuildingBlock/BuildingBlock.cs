using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Building Block for the lattice. Each building block has the size of the unit cell.
    /// </summary>
    [DataContract(Name = "BuildingBlock")]
    public class BuildingBlock : ModelObject, IBuildingBlock
    {
        /// <inheritdoc />
        /// <summary>
        ///     The occupation of the building block
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
        public List<IParticle> CellEntries { get; set; }

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
            if (CastIfNotDeprecated<IBuildingBlock>(modelObject) is var casted)
            {
                CellEntries = casted.CellEntries;
                return this;
            }

            return null;
        }
    }
}