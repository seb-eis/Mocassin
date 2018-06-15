using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Lattice information, such as the extent of the super cell
    /// </summary>
    [Serializable]
    [DataContract(Name = "LatticeInfo")]
    public class LatticeInfo : ModelParameter, ILatticeInfo
    {
        /// <summary>
        /// Extent of lattice
        /// </summary>
        [DataMember]
        public DataIntegralVector3D Extent { get; set; }

        /// <summary>
        /// Creates a new lattice information set
        /// </summary>
        /// <param name="paramX"></param>
        /// <param name="paramY"></param>
        /// <param name="paramZ"></param>
        public LatticeInfo(int paramX, int paramY, int paramZ)
        {
            Extent = new DataIntegralVector3D(paramX, paramY, paramZ);
        }

        /// <summary>
        /// Creates default lattice parameters (no extent of unit cell)
        /// </summary>
        /// <returns></returns>
        public static LatticeInfo CreateDefault()
        {
            return new LatticeInfo(1, 1, 1);
        }

        /// <summary>
        /// creates a string that contains the lattice information
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return $"{GetParameterName()} ({Extent})";
        }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override String GetParameterName()
        {
            return "'Super Cell Extent Parameters'";
        }

        /// <summary>
        /// Copies the information from the provided parameter interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (modelParameter is ILatticeInfo casted)
            {
                Extent = casted.Extent;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Compares if the model parameter interface contains the same parameter info (Returns false if type cannot be cast)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IModelParameter other)
        {
            if (other is ILatticeInfo castOther)
            {
                return Extent.Equals(castOther.Extent);
            }
            return false;
        }
    }
}
