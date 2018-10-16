using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Attribute that marks a property as indexed model data which supports reindexing operations and a specific model
    ///     object interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexedModelDataAttribute : Attribute
    {
        /// <summary>
        ///     Flag that marks the indexed model data aus automatically generated and managed by the manager (No custom input
        ///     support)
        /// </summary>
        public bool IsAutoManaged { get; set; }

        /// <summary>
        ///     Type of the model object interface that is supported by the container (Required for event distribution)
        /// </summary>
        public Type InterfaceType { get; protected set; }

        /// <summary>
        ///     Create new indexed model data attribute with the specified type of the indexed objects access interface
        /// </summary>
        /// <param name="interfaceType"></param>
        public IndexedModelDataAttribute(Type interfaceType)
        {
            if (!typeof(IModelObject).IsAssignableFrom(interfaceType))
                throw new ArgumentException("The interface type is not assignable to a model object interface");

            InterfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
        }
    }
}