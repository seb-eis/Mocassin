using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Attribute to mark properties as conflict resolvers for a specific type of data operation
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConflictHandlerAttribute : Attribute
    {
        /// <summary>
        ///     The type of data operation the resolver is supposed to handle
        /// </summary>
        public DataOperationType DataOperationType { get; }

        /// <summary>
        ///     Creates new attribute class with the provided data operation type
        /// </summary>
        /// <param name="dataOperationType"></param>
        public ConflictHandlerAttribute(DataOperationType dataOperationType)
        {
            DataOperationType = dataOperationType;
        }
    }
}