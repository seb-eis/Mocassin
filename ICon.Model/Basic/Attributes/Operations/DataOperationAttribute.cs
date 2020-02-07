using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Enum for data operation types that can be applied to the model data and are supported by the automated pipeline build system
    /// </summary>
    public enum DataOperationType
    {
        /// <summary>
        ///     Defines an operation that registers a new object
        /// </summary>
        NewObject,

        /// <summary>
        ///     Defines an operation that changes an existing object
        /// </summary>
        ObjectChange,

        /// <summary>
        ///     Defines an operation that removes an existing object
        /// </summary>
        ObjectRemoval,

        /// <summary>
        ///     Defines an operation that changes a unique parameter
        /// </summary>
        ParameterChange,

        /// <summary>
        ///     Defines an operation that performs object cleanup
        /// </summary>
        ObjectCleaning
    }

    /// <summary>
    ///     Attribute to mark methods as data operations that add or manipulate the model data
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DataOperationAttribute : Attribute
    {
        /// <summary>
        ///     The type of the operation the marked method performs
        /// </summary>
        public DataOperationType OperationType { get; }

        /// <summary>
        ///     Creates new data operation method attribute with the type of operation that is performed by the marked operation
        /// </summary>
        /// <param name="operationType"></param>
        public DataOperationAttribute(DataOperationType operationType)
        {
            OperationType = operationType;
        }
    }
}