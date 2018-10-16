using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Attribute to mark methods as a provider method for conflict resolvers for a specific data operation.
    ///     Should be used to enforce overwrite of default handlers in auto generated pipeline systems
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HandlerFactoryMethodAttribute : Attribute
    {
        /// <summary>
        ///     The type of data operation the resolver is supposed to handle
        /// </summary>
        public DataOperationType DataOperationType { get; }

        /// <summary>
        ///     Creates new attribute class with the provided data operation type
        /// </summary>
        /// <param name="dataOperationType"></param>
        public HandlerFactoryMethodAttribute(DataOperationType dataOperationType)
        {
            DataOperationType = dataOperationType;
        }
    }
}