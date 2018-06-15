using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for all model data manager implementations
    /// </summary>
    public abstract class ModelDataManager : IModelDataPort
    {
        /// <summary>
        /// Serializes the data into the JSON format
        /// </summary>
        /// <returns></returns>
        public abstract String JsonSerializeData();

        /// <summary>
        /// Serialize the data as a data contract with the specified settings to the provided stream
        /// </summary>
        /// <param name="stream"></param>
        public abstract void WriteDataContract(Stream stream, DataContractSerializerSettings settings);

        /// <summary>
        /// Creates a copy of the internal data by json serialization and deserialization
        /// </summary>
        /// <returns></returns>
        public abstract Object GetDataCopy();

        /// <summary>
        /// Generic reindexing function that creates a cleaned reindex info that results if deprecated model data is removed
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        protected ReindexingList CreateReindexing<TModelObject>(IEnumerable<TModelObject> modelObjects) where TModelObject : IModelObject
        {
            return CreateReindexing(modelObjects, 0);
        }

        /// <summary>
        /// Generic reindexing function that creates a cleaned reindex info that results if deprecated model data is removed (With additional list initiation size)
        /// </summary>
        /// <typeparam name="TModelObject"></typeparam>
        /// <param name="modelObjects"></param>
        /// <param name="listCount"></param>
        /// <returns></returns>
        protected ReindexingList CreateReindexing<TModelObject>(IEnumerable<TModelObject> modelObjects, Int32 listCount) where TModelObject : IModelObject
        {
            if (modelObjects == null)
            {
                throw new ArgumentNullException(nameof(modelObjects));
            }

            Int32 newIndex = -1;
            var result = new ReindexingList(listCount);
            foreach (TModelObject obj in modelObjects)
            {
                result.Add((obj.Index, (obj.IsDeprecated) ? -1 : ++newIndex));
            }
            return result;
        }
    }

    /// <summary>
    /// Generic base class for model data manager implementations
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ModelDataManager<TData> : ModelDataManager where TData : ModelData
    {
        /// <summary>
        /// The model data object
        /// </summary>
        protected TData Data { get; set; }

        /// <summary>
        /// Creates new model data manager base with the provided data object
        /// </summary>
        /// <param name="data"></param>
        public ModelDataManager(TData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Get a copy of the internal data object by json serialization
        /// </summary>
        /// <returns></returns>
        public override Object GetDataCopy()
        {
            return JsonConvert.DeserializeObject(JsonSerializeData(), typeof(TData));
        }

        /// <summary>
        /// Json serialize the data object
        /// </summary>
        /// <returns></returns>
        public override String JsonSerializeData()
        {
            return JsonConvert.SerializeObject(Data);
        }

        /// <summary>
        /// Use the data write to a stream of data contract serializer with the specified settings
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="settings"></param>
        public override void WriteDataContract(Stream stream, DataContractSerializerSettings settings)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            new DataContractSerializer(typeof(TData), settings).WriteObject(stream, Data);
        }
    }
}
