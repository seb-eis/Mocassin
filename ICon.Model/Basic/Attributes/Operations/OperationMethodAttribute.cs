using System;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Enum for data operation types that can be applied to the model data
    /// </summary>
    public enum DataOperationType : int
    {
        NewObject, ObjectChange, ObjectRemoval, ParameterChange, ObjectCleaning
    }

    /// <summary>
    /// Attribute to mark methods as data operations that add or manipulate the model data
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OperationMethodAttribute : Attribute
    {
        /// <summary>
        /// The type of the operation the marked method performs
        /// </summary>
        public DataOperationType OperationType { get; }

        /// <summary>
        /// Creates new data operation method attribute with the type of operation that is performed by the marked operation
        /// </summary>
        /// <param name="operationType"></param>
        public OperationMethodAttribute(DataOperationType operationType)
        {
            OperationType = operationType;
        }
    }
}
