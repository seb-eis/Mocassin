using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Base class for all serializable data objects that supply <see cref="ModelObject" /> conversion for data input
    /// </summary>
    [XmlRoot]
    public abstract class XmlModelObject : XmlEntity
    {
        /// <summary>
        ///     Key value backing field
        /// </summary>
        [XmlIgnore] private string _key;

        /// <summary>
        ///     Key reference value backing field
        /// </summary>
        [XmlIgnore] private string _keyReference;

        /// <summary>
        ///     The key of the model object. Setting this value erases the reference
        /// </summary>
        [XmlAttribute("Key")]
        public string Key
        {
            get => _key;
            set
            {
                _keyReference = null;
                _key = value;
            }
        }

        /// <summary>
        ///     The key reference of the model object. Setting this property erases the key
        /// </summary>
        [XmlAttribute("Ref")]
        public string KeyReference
        {
            get => _keyReference;
            set
            {
                _key = null;
                _keyReference = value;
            }
        }

        /// <summary>
        ///     Get the set key or key reference of the object and throws an exception if both are null
        /// </summary>
        /// <returns></returns>
        public string GetKey()
        {
            return Key ?? KeyReference ?? throw new InvalidOperationException("Both key options are null");
        }

        /// <summary>
        ///     Get the input object for the automated data input system of the model management
        /// </summary>
        /// <returns></returns>
        public ModelObject GetInputObject()
        {
            var obj = GetPreparedModelObject();
            obj.Key = Key ?? KeyReference;
            obj.Index = -1;
            return obj;
        }

        /// <summary>
        ///     Get a prepared model object with all specific input data set
        /// </summary>
        /// <returns></returns>
        protected abstract ModelObject GetPreparedModelObject();
    }
}