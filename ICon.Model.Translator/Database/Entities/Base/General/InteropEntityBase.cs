using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Represents an interop entity that has properties that are represents as local blobs in the database but is not
    ///     itself a blob stored entity
    /// </summary>
    public abstract class InteropEntityBase : EntityBase
    {
        /// <summary>
        ///     Delegate fro the state change action of interop entities
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="service"></param>
        /// <param name="isToBinary"></param>
        public delegate void StateChangeAction(InteropEntityBase entity, IMarshalService service, bool isToBinary);

        /// <summary>
        ///     Abstract access to the implementing class state change action list
        /// </summary>
        protected abstract IList<StateChangeAction> StateChangeActions { get; set; }

        /// <summary>
        ///     Parses the blob entity object into the binary data and header properties
        /// </summary>
        public virtual void ChangePropertyStatesToBinaries(IMarshalService marshalService)
        {
            foreach (var item in GetStateChangeDelegates())
                item.Invoke(this, marshalService, true);
        }

        /// <summary>
        ///     Parses the binary data and header properties and populates the object
        /// </summary>
        public virtual void ChangePropertyStatesToObjects(IMarshalService marshalService)
        {
            foreach (var item in GetStateChangeDelegates())
                item.Invoke(this, marshalService, false);
        }

        /// <summary>
        ///     Gets the state change delegates for the object. Builds the delegates on first call
        /// </summary>
        /// <returns></returns>
        public IList<StateChangeAction> GetStateChangeDelegates()
        {
            if (StateChangeActions != null)
                return StateChangeActions;

            StateChangeActions = BuildStateChangeDelegates();
            return StateChangeActions;
        }

        /// <summary>
        ///     Builds all state change delegates for the implementing object type
        /// </summary>
        /// <returns></returns>
        protected IList<StateChangeAction> BuildStateChangeDelegates()
        {
            var actions = new List<StateChangeAction>();

            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(typeof(InteropPropertyAttribute)) is InteropPropertyAttribute interopAttribute)
                    actions.Add(BuildInteropPropertyHandler(propertyInfo, interopAttribute));

                if (propertyInfo.GetCustomAttribute(typeof(OwnedBlobPropertyAttribute)) is OwnedBlobPropertyAttribute ownedAttribute)
                    actions.Add(BuildOwnedBlobPropertyHandler(propertyInfo, ownedAttribute));
            }

            return actions;
        }

        /// <summary>
        ///     Build handler delegate for an interop property that handles conversion from or to the binary state
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected StateChangeAction BuildInteropPropertyHandler(PropertyInfo sourceProperty, InteropPropertyAttribute attribute)
        {
            var targetProperty = GetType().GetProperty(attribute.BinaryPropertyName)
                                 ?? throw new InvalidOperationException("Binary target property does not exist");

            void HandleToBinary(InteropEntityBase entity, IMarshalService marshalService)
            {
                if (!(sourceProperty.GetValue(entity) is InteropObject netObject))
                    return;

                var bytes = new byte[netObject.ByteCount];
                netObject.ToBinary(bytes, 0, marshalService);
                targetProperty.SetValue(entity, bytes);
                sourceProperty.SetValue(entity, null);
            }

            void HandleFromBinary(InteropEntityBase entity, IMarshalService marshalService)
            {
                if (!(targetProperty.GetValue(entity) is byte[] bytes))
                    return;

                var netObject = (InteropObject) Activator.CreateInstance(sourceProperty.PropertyType);
                netObject.FromBinary(bytes, 0, marshalService);
                sourceProperty.SetValue(entity, netObject);
                targetProperty.SetValue(entity, null);
            }

            return (entity, marshalProvider, isToBinary) =>
            {
                if (isToBinary)
                {
                    HandleToBinary(entity, marshalProvider);
                    return;
                }

                HandleFromBinary(entity, marshalProvider);
            };
        }


        /// <summary>
        ///     Build handler delegate for an owned blob property that handles conversion from or to the binary state
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected StateChangeAction BuildOwnedBlobPropertyHandler(PropertyInfo sourceProperty, OwnedBlobPropertyAttribute attribute)
        {
            var targetProperty = GetType().GetProperty(attribute.BlobPropertyName)
                                 ?? throw new InvalidOperationException("Blob property does not exist");

            void HandleToBinary(InteropEntityBase entity, IMarshalService marshalService)
            {
                if (!(sourceProperty.GetValue(entity) is BlobEntityBase netObject))
                    return;

                netObject.ChangeStateToBinary(marshalService);
                targetProperty.SetValue(entity, netObject.BinaryState);
            }

            void HandleFromBinary(InteropEntityBase entity, IMarshalService marshalService)
            {
                if (!(targetProperty.GetValue(entity) is byte[] binBuffer))
                    return;

                var netObject = (BlobEntityBase) Activator.CreateInstance(sourceProperty.PropertyType);
                netObject.BinaryState = binBuffer;
                netObject.ChangeStateToObject(marshalService);
                sourceProperty.SetValue(entity, netObject);
            }

            return (entity, marshalProvider, isToBinary) =>
            {
                if (isToBinary)
                {
                    HandleToBinary(entity, marshalProvider);
                    return;
                }

                HandleFromBinary(entity, marshalProvider);
            };
        }
    }
}