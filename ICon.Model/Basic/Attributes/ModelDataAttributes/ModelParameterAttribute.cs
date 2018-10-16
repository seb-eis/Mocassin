using System;
using System.Linq;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Attribute to mark a property as a model parameter which supports a specific model parameter interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class ModelParameterAttribute : Attribute
    {
        /// <summary>
        ///     The type of the parameter interface supported by the parameter
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        ///     Create new model parameter attribute with the specified parameter interface type
        /// </summary>
        /// <param name="interfaceType"></param>
        public ModelParameterAttribute(Type interfaceType)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            if (!interfaceType.GetInterfaces().Contains(typeof(IModelParameter)))
                throw new ArgumentException("Specified interface type does not implement general model parameter interface",
                    nameof(interfaceType));

            InterfaceType = interfaceType;
        }
    }
}