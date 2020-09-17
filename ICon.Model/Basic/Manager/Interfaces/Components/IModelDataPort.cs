using System.IO;
using System.Runtime.Serialization;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     General base interface for all model data port interfaces
    /// </summary>
    public interface IModelDataPort
    {
        /// <summary>
        ///     Serializes the data into the JSON format
        /// </summary>
        /// <returns></returns>
        string JsonSerializeData();

        /// <summary>
        ///     Serialize the data as a data contract with the specified settings to the provided stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="settings"></param>
        void WriteDataContract(Stream stream, DataContractSerializerSettings settings);

        /// <summary>
        ///     Creates a copy of the internal data by json serialization and deserialization
        /// </summary>
        /// <returns></returns>
        object GetDataCopy();
    }
}