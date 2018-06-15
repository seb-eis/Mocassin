using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Contains structure informations defined by a user that do not hold any model relevance other that misc information
    /// </summary>
    [DataContract(Name = "StructureInfo")]
    public class StructureInfo : ModelParameter, IStructureInfo
    {
        /// <summary>
        /// The name of the structure
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Get a string representing the name of the type
        /// </summary>
        /// <returns></returns>
        public override string GetParameterName()
        {
            return "'Structure Info'";
        }

        /// <summary>
        /// Creates the default initialized structure info
        /// </summary>
        /// <returns></returns>
        public static StructureInfo CreateDefault()
        {
            return new StructureInfo() { Name = "Unnamed" };
        }

        /// <summary>
        /// Copies values from provided interface into this object and returns object (Returns null if type mismatch)
        /// </summary>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (modelParameter is IStructureInfo casted)
            {
                Name = casted.Name;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks other model parameter interface for content equality (Returns false if type mismatch)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IModelParameter other)
        {
            if (other is IStructureInfo casted)
            {
                return Name == casted.Name;
            }
            return false;
        }
    }
}
