using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract model parameter class for implementations of unique model propertiesthat do not support indexing
    /// </summary>
    [DataContract]
    public abstract class ModelParameter : IModelParameter
    {
        /// <summary>
        /// Get the type name of the model parameter
        /// </summary>
        /// <returns></returns>
        public abstract string GetParameterName();

        /// <summary>
        /// Basic string representation with name and json format serialization values
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{GetParameterName()}\n{JsonConvert.SerializeObject(this)}";
        }

        /// <summary>
        /// Builds a new object of the specfified type and populates it from the provided matching interface (Returns null if not possible)
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public static TParam BuildInternalObject<TParam>(IModelParameter modelParameter) where TParam : ModelParameter, new()
        {
            return new TParam().PopulateObject(modelParameter) as TParam;
        }

        /// <summary>
        /// Consumes the provided interface if possible and returns the model object filled with the parametr info (Returns null if wrong type)
        /// </summary>
        /// <param name="modelParameter"></param>
        public abstract ModelParameter PopulateObject(IModelParameter modelParameter);

        /// <summary>
        /// Compares the contents of two model parameters for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals(IModelParameter other);
    }
}
