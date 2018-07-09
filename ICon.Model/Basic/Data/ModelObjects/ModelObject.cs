using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for implementations of the IModelObject interface
    /// </summary>
    [DataContract]
    public abstract class ModelObject : IModelObject
    {
        /// <summary>
        /// The index of the model object assigned by the affiliated manager
        /// </summary>
        [DataMember]
        public int Index { get; set; }

        /// <summary>
        /// Flag if the object is deprecated
        /// </summary>
        [DataMember]
        public bool IsDeprecated { get; set; }

        /// <summary>
        /// Base deprecation operation that only sets the deprecation flag to true
        /// </summary>
        public virtual void Deprecate()
        {
            IsDeprecated = true;
        }

        /// <summary>
        /// Base restore operation that only sets the deprecation falg to false
        /// </summary>
        public virtual void Restore()
        {
            IsDeprecated = false;
        }

        /// <summary>
        /// Basic string representation with name and json format serialization values
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{GetModelObjectName()}\n{JsonConvert.SerializeObject(this, Formatting.Indented)}";
        }

        /// <summary>
        /// Get the name of the model object as a string
        /// </summary>
        /// <returns></returns>
        public abstract string GetModelObjectName();

        /// <summary>
        /// Builds the specfified object and populates it by the passed interface
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TObject BuildInternalObject<TObject>(IModelObject obj) where TObject : ModelObject, new()
        {
           return new TObject().PopulateFrom(obj) as TObject;
        }

        /// <summary>
        /// Tries to fill a model object by interface, returns the filled object on success or null if not possible or if the interface object is deprecated
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract ModelObject PopulateFrom(IModelObject obj);

        /// <summary>
        /// Cast the passed model object inetrafec to the correct type (Retruns null if not possible or object ist dprectaed)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected T1 CastWithDepricatedCheck<T1>(IModelObject obj) where T1 : class, IModelObject
        {
            return (obj.IsDeprecated) ? null : obj as T1;
        }
    }
}
