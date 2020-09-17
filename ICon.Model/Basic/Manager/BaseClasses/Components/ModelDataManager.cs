using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for all model data manager implementations
    /// </summary>
    public abstract class ModelDataManager : IModelDataPort
    {
        /// <inheritdoc />
        public abstract string JsonSerializeData();

        /// <inheritdoc />
        public abstract void WriteDataContract(Stream stream, DataContractSerializerSettings settings);

        /// <inheritdoc />
        public abstract object GetDataCopy();

        /// <summary>
        ///     Generic reindexing function that creates a cleaned reindex info that results if deprecated model data is removed
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        protected ReindexingList CreateReindexing<TModelObject>(IEnumerable<TModelObject> modelObjects)
            where TModelObject : IModelObject =>
            CreateReindexing(modelObjects, 0);

        /// <summary>
        ///     Generic reindexing function that creates a cleaned reindex info that results if deprecated model data is removed
        ///     (With additional list initiation size)
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="modelObjects"></param>
        /// <param name="listCount"></param>
        /// <returns></returns>
        protected ReindexingList CreateReindexing<TObject>(IEnumerable<TObject> modelObjects, int listCount)
            where TObject : IModelObject
        {
            if (modelObjects == null)
                throw new ArgumentNullException(nameof(modelObjects));

            var newIndex = -1;
            var result = new ReindexingList(listCount);
            foreach (var obj in modelObjects)
                result.Add((obj.Index, obj.IsDeprecated ? -1 : ++newIndex));

            return result;
        }
    }

    /// <summary>
    ///     Generic base class for model data manager implementations
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ModelDataManager<TData> : ModelDataManager
        where TData : ModelData
    {
        /// <summary>
        ///     The model data object
        /// </summary>
        protected TData Data { get; set; }

        /// <summary>
        ///     Creates new model data manager base with the provided data object
        /// </summary>
        /// <param name="modelData"></param>
        public ModelDataManager(TData modelData)
        {
            Data = modelData ?? throw new ArgumentNullException(nameof(modelData));
        }

        /// <inheritdoc />
        public override object GetDataCopy() => JsonConvert.DeserializeObject(JsonSerializeData(), typeof(TData));

        /// <inheritdoc />
        public override string JsonSerializeData() => JsonConvert.SerializeObject(Data);

        /// <inheritdoc />
        public override void WriteDataContract(Stream stream, DataContractSerializerSettings settings)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            new DataContractSerializer(typeof(TData), settings).WriteObject(stream, Data);
        }
    }
}