using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Mocassin.Model.Basic
{
    /// <inheritdoc />
    /// <remarks> Abstract base class for model object implementations </remarks>
    [DataContract]
    public abstract class ModelObject : IModelObject
    {
        /// <inheritdoc />
        [DataMember]
        public int Index { get; set; }

        /// <inheritdoc />
        [DataMember]
        public bool IsDeprecated { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Key { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Construct new model object that has an invalid index
        /// </summary>
        protected ModelObject()
        {
            Index = -1;
        }

        /// <inheritdoc />
        public virtual void Deprecate()
        {
            IsDeprecated = true;
        }

        /// <summary>
        ///     Base restore operation that only sets the deprecation flag to false
        /// </summary>
        public virtual void Restore()
        {
            IsDeprecated = false;
        }

        /// <summary>
        ///     Basic string representation with name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{ObjectName}:{Name ?? "??"}";
        }

		/// <inheritdoc />
		public abstract string ObjectName { get; }

        /// <summary>
        ///     Builds the specified object and populates it by the passed interface
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T1 BuildInternalObject<T1>(IModelObject obj) where T1 : ModelObject, new()
        {
            if (!(new T1().PopulateFrom(obj) is T1 internalObj)) return null;

            internalObj.Key = obj.Key;
            internalObj.Name = obj.Name;

            return internalObj;
        }

        /// <summary>
        ///     Tries to fill a model object by interface, returns the filled object on success or null if not possible or if the
        ///     interface object is deprecated
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract ModelObject PopulateFrom(IModelObject obj);

        /// <summary>
        ///     Cast the passed model object interface to the correct type (Returns null if not possible or object is marked as
        ///     deprecated)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected T1 CastIfNotDeprecated<T1>(IModelObject obj) where T1 : class, IModelObject
        {
            return obj.IsDeprecated ? null : obj as T1;
        }
    }
}