using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Building Block for the lattice. Each building block has the size of the unit cell.
    /// </summary>
    [Serializable]
    [DataContract(Name = "BuildingBlock")]
    public class BuildingBlock : ModelObject, IBuildingBlock
    {
        /// <summary>
        /// The list interface of unit cell entries
        /// </summary>
        public int[] CellEntries { get; set; }

        /// <summary>
        /// Flag that indicates whether building block is user defined
        /// </summary>
        public bool IsCustom { get; set; }

        /// <summary>
        /// Get Enumerator of list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<uint> GetEnumerator()
        {
            return CellEntries.GetEnumerator();
        }

        /// <summary>
        /// Indexer for the CellEntries
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public uint this[int index] => CellEntries[index];

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Building Block'";
        }

        /// <summary>
        /// creates a string that contains the model object information
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{GetModelObjectName()} ({IsCustom}, {CellEntries})";
        }

        /// <summary>
        /// Copies the information from the provided model object interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        protected override ModelObject ConsumeInterface(IModelObject modelObject)
        {
            if (modelObject is IBuildingBlock casted)
            {
                IsCustom = casted.IsCustom;
                CellEntries = casted.CellEntries;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Mandatory GetEnumerator method
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
