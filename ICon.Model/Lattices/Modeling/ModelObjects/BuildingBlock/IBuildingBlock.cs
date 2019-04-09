using Mocassin.Symmetry.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Building Block for the lattice. Each building block has the size of the unit cell.
    /// </summary>
    public interface IBuildingBlock : IModelObject
    {
        /// <summary>
        /// The occupation of the building block
        /// </summary>
        List<IParticle> CellEntries { get; }

	    /// <inheritdoc />
	    /// <summary>
	    /// The population instructions to fill the super cell
	    /// </summary>
	    //Matrix2D PopulationInstructions { get; set; }

    }
}
