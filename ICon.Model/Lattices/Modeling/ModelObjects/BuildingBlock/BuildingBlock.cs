using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Building Block for the lattice. Each building block has the size of the unit cell.
    /// </summary>
    [DataContract(Name = "BuildingBlock")]
    public class BuildingBlock : ModelObject, IBuildingBlock
    {
        /// <summary>
        /// The list interface of unit cell entries
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IParticle> CellEntries { get; set; }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetObjectName()
        {
            return "'Building Block'";
        }

        /// <summary>
        /// Copies the information from the provided model object interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="modelObject"></param>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (obj is IBuildingBlock casted)
            {
                CellEntries = casted.CellEntries;
                return this;
            }
            return null;
        }

    }
}
