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
    [DataContract(Name = "LatticeInfo")]
    public class LatticeInfo : ModelParameter, ILatticeInfo
    {

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override String GetParameterName()
        {
            return "'Super Cell Parameters'";
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
                return true;
            }
            return false;
        }
    }
}
