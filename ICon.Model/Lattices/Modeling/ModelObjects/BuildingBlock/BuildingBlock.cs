using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Building Block for the lattice. Each building block has the size of the unit cell.
    /// </summary>
    [DataContract(Name = "BuildingBlock")]
    public class BuildingBlock : ModelObject, IBuildingBlock
    {
        /// <inheritdoc />
        /// <summary>
        /// The occupation of the building block
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
        public List<IParticle> CellEntries { get; set; }

	    /// <inheritdoc />
	    /// <summary>
	    /// The population instructions to fill the super cell
	    /// </summary>
	    //[DataMember]
	    //[UseTrackedReferences]
		//public Matrix2D PopulationInstructions { get; set; }

		/// <inheritdoc />
		/// <summary>
		/// Get the type name string
		/// </summary>
		/// <returns></returns>
		public override string ObjectName => "Building Block";

		/// <summary>
		/// Copies the information from the provided model object interface and returns the object (Retruns null if type mismatch)
		/// </summary>
		/// <param name="modelObject"></param>
		public override ModelObject PopulateFrom(IModelObject modelObject)
        {
            if (CastIfNotDeprecated<IBuildingBlock>(modelObject) is var casted)
            {
                CellEntries = casted.CellEntries;
	            //PopulationInstructions = casted.PopulationInstructions;
                return this;
            }
            return null;
        }

    }
}
