using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;
using Newtonsoft.Json;

using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.DataManagement
{
    /// <summary>
    /// Model data tracker that collects the data objects of a set of model managers for reference lookup and serialization
    /// </summary>
    [DataContract]
    public class ModelDataTracker : IModelDataTracker
    {
        /// <summary>
        /// The object liker dictionary that contains cached linkingdelegates for known model data objects
        /// </summary>
        [IgnoreDataMember]
        public Dictionary<Type, Action<object>> ObjectLinkerDictionary { get; set; }

        /// <summary>
        /// The data object dictionary that stores the data object reference and affiliated manager as key value pairs
        /// </summary>
        [DataMember]
        public Dictionary<Type, object> DataObjectDictionary { get; set; }

        /// <summary>
        /// Lookup dictionary for model objects that assigns each type of model manager a dictionary of read only collections containing the model objects
        /// </summary>
        [DataMember]
        public Dictionary<Type, IList> ModelObjectDictionary { get; set; }

        /// <summary>
        /// Creates new model data tarcker with empty dictionary initializations
        /// </summary>
        public ModelDataTracker()
        {
            ModelObjectDictionary = new Dictionary<Type, IList>();
            ObjectLinkerDictionary = new Dictionary<Type, Action<object>>();
            DataObjectDictionary = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Creates a new manager using the provided model manager factory and project service and registers the data object with the tracking system
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="managerFactory"></param>
        /// <returns></returns>
        public IModelManager CreateAndRegister(IProjectServices projectServices, IModelManagerFactory managerFactory)
        {
            if (managerFactory.CreateNew(projectServices, out var dataObject) is IModelManager manager)
            {
                projectServices.RegisterManager(manager);
                DataObjectDictionary[manager.GetManagerInterfaceType()] = dataObject;
                UpdateObjectLookupDictionary(dataObject);
                return manager;
            }
            return null;
        }

        /// <summary>
        /// Lookup the internal data object of the specfified interface type that belongs to the given index, returns null if non exists
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TInterfaceObject FindObjectInterfaceByIndex<TInterfaceObject>(int index)
        {
            var lookup = ModelObjectDictionary[typeof(TInterfaceObject)];
            if (lookup.Count > index)
            {
                return (TInterfaceObject)lookup[index];
            }
            return default;
        }

        /// <summary>
        /// Takes a generic object and corrects lookups the correct linker. If non exists one ist created and put into the linking dictionary
        /// </summary>
        /// <param name="obj"></param>
        public void LinkModelObject(object obj)
        {
            if (ObjectLinkerDictionary.TryGetValue(obj.GetType(), out var linker))
            {
                linker(obj);
            }
            else
            {
                linker = MakeLinker(obj.GetType(), obj);
                ObjectLinkerDictionary[obj.GetType()] = linker;
                linker(obj);
            }
        }

        /// <summary>
        /// Tries to link a model object. Returns false if the linking failed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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
        /// Updates the dictionary entries for the provided manager type with the provided data object
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="managerType"></param>
        protected void UpdateObjectLookupDictionary(object dataObject)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            foreach (var property in dataObject.GetType().GetProperties(flags))
            {
                if (property.GetCustomAttribute(typeof(IndexedModelDataAttribute)) is IndexedModelDataAttribute attribute)
                {
                    ModelObjectDictionary[attribute.InterfaceType] = (IList)property.GetValue(dataObject);
                }
            }
        }

        /// <summary>
        /// Creates a linking delegate for the provided object type based upon the model data refernce attribute
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected Action<object> MakeLinker(Type objectType, object obj)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            var linkers = new List<Action<object>>();
            foreach (var property in objectType.GetProperties(flags))
            {
                if (property.GetCustomAttribute(typeof(LinkableByIndexAttribute)) is LinkableByIndexAttribute attribute)
                {
                    switch (attribute.LinkableType)
                    {
                        case LinkableType.Value:
                            linkers.Add(MakeLinkDelegate(property));
                            break;
                        case LinkableType.Content:
                            HandleContentLinkableProperty(property);
                            break;
                        default:
                            throw new NotSupportedException("Linking flag is currently not supported by the tracker");
                    }
                }
            }

            void LinkAll(object value)
            {
                foreach (var item in linkers)
                {
                    item(value);
                }
            }
            return LinkAll;
        }

        /// <summary>
        /// Handles a property value that is marked as content linkable depending on it beeing a single value or a list of content linkables
        /// </summary>
        /// <param name="propertyValue"></param>
        protected void HandleContentLinkableProperty(object propertyValue)
        {
            if (propertyValue is IList linkables)
            {
                foreach (var item in linkables)
                {
                    LinkModelObject(item);
                }
            }
            else
            {
                LinkModelObject(propertyValue);
            }
        }

        /// <summary>
        /// Determines the type of the linkable property based upon the info is a collection type or single type. Only works if the collection implements only one
        /// indexed parameter
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
            if (typeof(IList).IsAssignableFrom(info.PropertyType))
            {
                providerDelegate = MakeObjectProviderDelegate(info.PropertyType.GetGenericArguments()[0]);
                return MakeListLinkerDelegate(info, providerDelegate);
            }
            throw new ArgumentException("Could not create link delegate. Property has to be a value or implemend the non-generic IList interface");
        }

        /// <summary>
        /// Creates a delegate to find a model object of a specfific type by index
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected Func<int, IModelObject> MakeObjectProviderDelegate(Type objectType)
        {
            IModelObject GetObject(int index)
            {
                foreach (IModelObject item in ModelObjectDictionary[objectType])
                {
                    if (item.Index == index)
                    {
                        return item;
                    }
                }
                return null;
            }
            return GetObject;
        }

        /// <summary>
        /// Makes a delegate for a property that is a list interafce of model objects (Has to implement non-generic list interface)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="objectProviderFunction"></param>
        /// <returns></returns>
        protected Action<object> MakeListLinkerDelegate(PropertyInfo info, Func<int, object> objectProviderFunction)
        {
            void CorrectLinks(object obj)
            {
                if (info.GetValue(obj) is IList list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] = objectProviderFunction(((IModelObject)list[i]).Index);
                    }
                }
            }
            return CorrectLinks;
        }

        /// <summary>
        /// Makes a delegate for a model object property that contains a single object link
        /// </summary>
        /// <param name="info"></param>
        /// <param name="objectProviderFunction"></param>
        /// <returns></returns>
        protected Action<object> MakeLinkDelegate(PropertyInfo info, Func<int, object> objectProviderFunction)
        {
            void Link(object obj)
            {
                var orgObj = (IModelObject)info.GetValue(obj);
                info.SetValue(obj, objectProviderFunction(orgObj.Index));
            }
            return Link;
        }

        /// <summary>
        /// Recreation method for the lookup dictionary after the data tracker is deserialized
        /// </summary>
        /// <param name="streamingContext"></param>
        [OnDeserialized]
        protected void RecreateLookupDictionary(StreamingContext streamingContext)
        {
            foreach (var entry in DataObjectDictionary)
            {
                UpdateObjectLookupDictionary(entry.Value);
            }
        }
    }
}
