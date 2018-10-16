using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.DataManagement
{
    /// <inheritdoc />
    [DataContract]
    public class ModelDataTracker : IModelDataTracker
    {
        /// <summary>
        ///     The object liker dictionary that contains cached linking delegates for known model data objects
        /// </summary>
        [IgnoreDataMember]
        public Dictionary<Type, Action<object>> ObjectLinkerDictionary { get; set; }

        /// <summary>
        ///     The data object dictionary that stores the data object reference and affiliated manager as key value pairs
        /// </summary>
        [DataMember]
        public Dictionary<Type, object> DataObjectDictionary { get; set; }

        /// <summary>
        ///     Lookup dictionary for model objects that assigns each type of model manager a dictionary of read only collections
        ///     containing the model objects
        /// </summary>
        [DataMember]
        public Dictionary<Type, IList> ModelObjectDictionary { get; set; }

        /// <summary>
        ///     Creates new model data tracker with empty dictionary initializations
        /// </summary>
        public ModelDataTracker()
        {
            ModelObjectDictionary = new Dictionary<Type, IList>();
            ObjectLinkerDictionary = new Dictionary<Type, Action<object>>();
            DataObjectDictionary = new Dictionary<Type, object>();
        }

        /// <inheritdoc />
        public IModelManager CreateAndRegister(IProjectServices projectServices, IModelManagerFactory managerFactory)
        {
            if (!(managerFactory.CreateNew(projectServices, out var dataObject) is IModelManager manager))
                return null;

            projectServices.RegisterManager(manager);
            DataObjectDictionary[manager.GetManagerInterfaceType()] = dataObject;
            UpdateObjectLookupDictionary(dataObject);
            return manager;

        }

        /// <inheritdoc />
        public TInterface FindObjectInterfaceByIndex<TInterface>(int index)
        {
            var lookup = ModelObjectDictionary[typeof(TInterface)];
            if (lookup.Count > index) 
                return (TInterface) lookup[index];

            return default;
        }

        /// <inheritdoc />
        public void LinkModelObject(object obj)
        {
            if (ObjectLinkerDictionary.TryGetValue(obj.GetType(), out var linker))
                linker(obj);
            else
            {
                linker = MakeLinker(obj.GetType(), obj);
                ObjectLinkerDictionary[obj.GetType()] = linker;
                linker(obj);
            }
        }

        /// <inheritdoc />
        public bool TryLinkModelObject(object obj)
        {
            try
            {
                LinkModelObject(obj);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Updates the dictionary entries for the provided manager type with the provided data object
        /// </summary>
        /// <param name="dataObject"></param>
        protected void UpdateObjectLookupDictionary(object dataObject)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            foreach (var property in dataObject.GetType().GetProperties(flags))
            {
                if (property.GetCustomAttribute(typeof(IndexedModelDataAttribute)) is IndexedModelDataAttribute attribute)
                    ModelObjectDictionary[attribute.InterfaceType] = (IList) property.GetValue(dataObject);
            }
        }

        /// <summary>
        ///     Creates a linking delegate for the provided object type based upon the model data reference attribute
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected Action<object> MakeLinker(Type objectType, object obj)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            var linkers = new List<Action<object>>();
            foreach (var property in objectType.GetProperties(flags))
            {
                if (!(property.GetCustomAttribute(typeof(IndexResolvedAttribute)) is IndexResolvedAttribute attribute))
                    continue;

                switch (attribute.IndexResolveLevel)
                {
                    case IndexResolveLevel.Value:
                        linkers.Add(MakeLinkDelegate(property));
                        break;
                    case IndexResolveLevel.Content:
                        HandleContentLinkableProperty(property);
                        break;
                    default:
                        throw new NotSupportedException("Linking flag is currently not supported by the tracker");
                }
            }

            void LinkAll(object value)
            {
                foreach (var item in linkers) 
                    item(value);
            }

            return LinkAll;
        }

        /// <summary>
        ///     Handles a property value that is marked as content linkable depending on it being a single value or a list of
        ///     content linkable objects
        /// </summary>
        /// <param name="propertyValue"></param>
        protected void HandleContentLinkableProperty(object propertyValue)
        {
            if (propertyValue is IList linkables)
            {
                foreach (var item in linkables) 
                    LinkModelObject(item);
            }
            else
                LinkModelObject(propertyValue);
        }

        /// <summary>
        ///     Determines the type of the linkable property based upon the info is a collection type or single type. Only works if
        ///     the collection implements only one
        ///     indexed parameter
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected Action<object> MakeLinkDelegate(PropertyInfo info)
        {
            Func<int, IModelObject> providerDelegate;
            if (typeof(IModelObject).IsAssignableFrom(info.PropertyType))
            {
                providerDelegate = MakeObjectProviderDelegate(info.PropertyType);
                return MakeLinkDelegate(info, providerDelegate);
            }

            if (!typeof(IList).IsAssignableFrom(info.PropertyType))
            {
                throw new ArgumentException(
                    "Could not create link delegate. Property has to be a value or implemented the non-generic IList interface");
            }

            providerDelegate = MakeObjectProviderDelegate(info.PropertyType.GetGenericArguments()[0]);
            return MakeListLinkerDelegate(info, providerDelegate);

        }

        /// <summary>
        ///     Creates a delegate to find a model object of a specific type by index
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected Func<int, IModelObject> MakeObjectProviderDelegate(Type objectType)
        {
            IModelObject GetObject(int index)
            {
                return ModelObjectDictionary[objectType].Cast<IModelObject>().FirstOrDefault(item => item.Index == index);
            }

            return GetObject;
        }

        /// <summary>
        ///     Makes a delegate for a property that is a list interface of model objects (Has to implement non-generic list
        ///     interface)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="objectProviderFunction"></param>
        /// <returns></returns>
        protected Action<object> MakeListLinkerDelegate(PropertyInfo info, Func<int, object> objectProviderFunction)
        {
            void CorrectLinks(object obj)
            {
                if (!(info.GetValue(obj) is IList list)) 
                    return;

                for (var i = 0; i < list.Count; i++)
                    list[i] = objectProviderFunction(((IModelObject) list[i]).Index);
            }

            return CorrectLinks;
        }

        /// <summary>
        ///     Makes a delegate for a model object property that contains a single object link
        /// </summary>
        /// <param name="info"></param>
        /// <param name="objectProviderFunction"></param>
        /// <returns></returns>
        protected Action<object> MakeLinkDelegate(PropertyInfo info, Func<int, object> objectProviderFunction)
        {
            void Link(object obj)
            {
                var orgObj = (IModelObject) info.GetValue(obj);
                info.SetValue(obj, objectProviderFunction(orgObj.Index));
            }

            return Link;
        }

        /// <summary>
        ///     Recreation method for the lookup dictionary after the data tracker is deserialized
        /// </summary>
        /// <param name="streamingContext"></param>
        [OnDeserialized]
        protected void RecreateLookupDictionary(StreamingContext streamingContext)
        {
            foreach (var entry in DataObjectDictionary) 
                UpdateObjectLookupDictionary(entry.Value);
        }
    }
}