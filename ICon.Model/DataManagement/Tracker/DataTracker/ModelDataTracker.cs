using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.DataManagement
{
    /// <inheritdoc />
    [DataContract]
    public class ModelDataTracker : IModelDataTracker
    {
        /// <summary>
        ///     The object liker dictionary that contains cached linking delegates for known model data objects
        /// </summary>
        [IgnoreDataMember] private readonly Dictionary<Type, Action<object>> _objectLinkerDictionary;

        /// <summary>
        ///     The data object dictionary that stores the data object reference and affiliated manager as key value pairs
        /// </summary>
        [DataMember]
        public Dictionary<Type, object> ModelDataDictionary { get; set; }

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
            _objectLinkerDictionary = new Dictionary<Type, Action<object>>();
            ModelDataDictionary = new Dictionary<Type, object>();
        }

        /// <inheritdoc />
        public IModelManager CreateAndRegister(IModelProject modelProject, IModelManagerFactory managerFactory)
        {
            if (!(managerFactory.CreateNew(modelProject, out var dataObject) is IModelManager manager))
                return null;

            modelProject.RegisterManager(manager);
            ModelDataDictionary[manager.GetManagerInterfaceType()] = dataObject;
            UpdateObjectLookupDictionary(dataObject);
            return manager;
        }

        /// <inheritdoc />
        public TObject FindObjectByIndex<TObject>(int index) where TObject : IModelObject
        {
            if (typeof(TObject) == typeof(IModelObject))
                throw new InvalidOperationException("Lookup of unspecified model object interface is ambiguous");

            var lookup = FindObjectList(typeof(TObject));
            if (lookup.Count > index)
                return (TObject) lookup[index];

            return default;
        }

        /// <inheritdoc />
        public TObject FindObjectByKey<TObject>(string key) where TObject : IModelObject
        {
            if (string.IsNullOrWhiteSpace(key))
                return default;

            if (typeof(TObject) == typeof(IModelObject))
                throw new InvalidOperationException("Lookup of unspecified model object interface is ambiguous");

            var lookup = FindObjectList(typeof(TObject));
            foreach (TObject item in lookup)
            {
                if (item.Key == key)
                    return item;
            }

            return default;
        }

        /// <inheritdoc />
        public IEnumerable<TObject> EnumerateObjects<TObject>() where TObject : IModelObject
        {
            if (typeof(TObject) == typeof(IModelObject))
                    throw new InvalidOperationException("Lookup of unspecified model object interface is ambiguous");

            return FindObjectList(typeof(TObject)).Cast<TObject>();
        }

        /// <inheritdoc />
        public int ObjectCount<TObject>() where TObject : IModelObject
        {
            return FindObjectList(typeof(TObject)).Count;
        }

        /// <inheritdoc />
        public void LinkModelObject(object obj)
        {
            if (_objectLinkerDictionary.TryGetValue(obj.GetType(), out var linker))
            {
                linker(obj);
            }
            else
            {
                linker = MakeLinker(obj.GetType(), obj);
                _objectLinkerDictionary[obj.GetType()] = linker;
                linker(obj);
            }
        }

        /// <inheritdoc />
        public bool TryLinkModelObject(object obj)
        {
            try
            {
                LinkModelObject(obj);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        /// <summary>
        ///     Finds the object lookup list that belongs to or if no direct match is found the list with objects directly
        ///     assignable from the passed type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The matching list or if none is found an empty container</returns>
        protected IList FindObjectList(Type type)
        {
            if (ModelObjectDictionary.TryGetValue(type, out var result))
                return result;

            result = ModelObjectDictionary.SingleOrDefault(x => x.Key.IsAssignableFrom(type)).Value;
            return result ?? new IModelObject[0];
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
                if (!(property.GetCustomAttribute(typeof(UseTrackedDataAttribute)) is UseTrackedDataAttribute attribute))
                    continue;

                switch (attribute.ReferenceCorrectionLevel)
                {
                    case ReferenceCorrectionLevel.Full:
                        linkers.Add(MakeLinkDelegate(property));
                        break;
                    case ReferenceCorrectionLevel.IgnoreTopLevel:
                        linkers.Add(x => LinkContent(property.GetValue(x)));
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
        protected void LinkContent(object propertyValue)
        {
            if (propertyValue is IList list)
            {
                foreach (var item in list)
                    LinkModelObject(item);
            }
            else
                LinkModelObject(propertyValue);
        }

        /// <summary>
        ///     Determines the type of the linkable property based upon the info is a collection type or single type. Only works if
        ///     the collection implements only one indexed parameter
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected Action<object> MakeLinkDelegate(PropertyInfo info)
        {
            Func<IModelObject, IModelObject> providerDelegate;
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
        ///     Creates a delegate to find a model object of a specific type by its index or key
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected Func<IModelObject, IModelObject> MakeObjectProviderDelegate(Type objectType)
        {
            IModelObject GetObject(IModelObject obj)
            {
                var modelObject = ModelObjectDictionary[objectType].Cast<IModelObject>().FirstOrDefault(item => item.Index == obj.Index)
                                  ?? ModelObjectDictionary[objectType].Cast<IModelObject>().FirstOrDefault(item => item.Key == obj.Key);

                return modelObject
                       ?? throw new InvalidOperationException(
                           $"No Object [{obj.ObjectName}] with Key={obj.Key} Id={obj.Index} exists");
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
        protected Action<object> MakeListLinkerDelegate(PropertyInfo info, Func<IModelObject, object> objectProviderFunction)
        {
            void CorrectLinks(object obj)
            {
                if (!(info.GetValue(obj) is IList list))
                    return;

                for (var i = 0; i < list.Count; i++)
                    list[i] = objectProviderFunction((IModelObject) list[i]);
            }

            return CorrectLinks;
        }

        /// <summary>
        ///     Makes a delegate for a model object property that contains a single object link
        /// </summary>
        /// <param name="info"></param>
        /// <param name="objectProviderFunction"></param>
        /// <returns></returns>
        protected Action<object> MakeLinkDelegate(PropertyInfo info, Func<IModelObject, object> objectProviderFunction)
        {
            void Link(object obj)
            {
                var orgObj = (IModelObject) info.GetValue(obj);
                info.SetValue(obj, objectProviderFunction(orgObj));
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
            foreach (var entry in ModelDataDictionary)
                UpdateObjectLookupDictionary(entry.Value);
        }
    }
}