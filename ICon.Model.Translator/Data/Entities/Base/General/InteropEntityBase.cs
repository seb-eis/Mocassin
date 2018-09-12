using System;
using System.Collections.Generic;
using System.Reflection;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Represents an interop entity that has properties that are representat as local blobs in the database but is not itself a blob
    /// </summary>
    public abstract class InteropEntityBase : EntityBase
    {
        /// <summary>
        /// Delegate fro the state change action of interop entities
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="provider"></param>
        /// <param name="isToBinary"></param>
        public delegate void InteropStateChangeAction(InteropEntityBase entity, IMarshalProvider provider, bool isToBinary);

        /// <summary>
        /// Abstract access to the implementing class state change action list
        /// </summary>
        protected abstract IList<InteropStateChangeAction> StateChangeActions { get; set; } 

        /// <summary>
        /// Parses the blob entity object into the binary data and header properties
        /// </summary>
        public virtual void ChangePropertyStatesToBinaries(IMarshalProvider marshalProvider)
        {
            foreach (var item in GetStateChangeDelegates())
            {
                item.Invoke(this, marshalProvider, true);
            }
        }

        /// <summary>
        /// Parses the binary data and header properties and populates the object
        /// </summary>
        public virtual void ChangePropertyStatesToObjects(IMarshalProvider marshalProvider)
        {
            foreach (var item in GetStateChangeDelegates())
            {
                item.Invoke(this, marshalProvider, false);
            }
        }

        /// <summary>
        /// Gets the state change delegates for the object. Builds the delegates on first call
        /// </summary>
        /// <returns></returns>
        public IList<InteropStateChangeAction> GetStateChangeDelegates()
        {
            if (StateChangeActions != null)
            {
                return StateChangeActions;
            }

            StateChangeActions = BuildStateChangeDelegates();
            return StateChangeActions;
        }

        /// <summary>
        /// Builds all state change delegates for the implementing object type
        /// </summary>
        /// <returns></returns>
        protected IList<InteropStateChangeAction> BuildStateChangeDelegates()
        {
            var actions = new List<InteropStateChangeAction>();

            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(typeof(InteropPropertyAttribute)) is InteropPropertyAttribute interopAttribute)
                {
                    actions.Add(BuildInteropPropertyHandler(propertyInfo, interopAttribute));
                }
                if (propertyInfo.GetCustomAttribute(typeof(OwnedBlobPropertyAttribute)) is OwnedBlobPropertyAttribute ownedAttribute)
                {
                    actions.Add(BuildOwendBlobPropertyHandler(propertyInfo, ownedAttribute));
                }
            }

            return actions;
        }

        /// <summary>
        /// Build handler delegate for an interop property
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected InteropStateChangeAction BuildInteropPropertyHandler(PropertyInfo sourceProperty, InteropPropertyAttribute attribute)
        {
            var targetProperty = GetType().GetProperty(attribute.BinaryPropertyName);

            void HandleToBinary(InteropEntityBase entity, IMarshalProvider marshalProvider)
            {
                if (sourceProperty.GetValue(entity) is InteropObject netObject)
                {
                    var buffer = new byte[netObject.BinarySize];
                    netObject.ToBinary(buffer, 0, marshalProvider);
                    targetProperty.SetValue(entity, buffer);
                }
            }

            void HandleFromBinary(InteropEntityBase entity, IMarshalProvider marshalProvider)
            {
                if (targetProperty.GetValue(entity) is byte[] binBuffer)
                {
                    var netObject = (InteropObject)Activator.CreateInstance(sourceProperty.PropertyType);
                    netObject.FromBinary(binBuffer, 0, marshalProvider);
                }
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
        /// Build ahndler delegate for an owned blob property
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected InteropStateChangeAction BuildOwendBlobPropertyHandler(PropertyInfo sourceProperty, OwnedBlobPropertyAttribute attribute)
        {
            var targetProperty = GetType().GetProperty(attribute.BlobProperyName);

            void HandleToBinary(InteropEntityBase entity, IMarshalProvider marshalProvider)
            {
                if (sourceProperty.GetValue(entity) is BlobEntityBase netObject)
                {
                    netObject.ChangeStateToBinary(marshalProvider);
                    targetProperty.SetValue(entity, netObject.BinaryState);
                }
            }

            void HandleFromBinary(InteropEntityBase entity, IMarshalProvider marshalProvider)
            {
                if (targetProperty.GetValue(entity) is byte[] binBuffer)
                {
                    var netObject = (BlobEntityBase)Activator.CreateInstance(sourceProperty.PropertyType);
                    netObject.BinaryState = binBuffer;
                    netObject.ChangeStateToObject(marshalProvider);
                    sourceProperty.SetValue(entity, netObject);
                }
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
