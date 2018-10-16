using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Attribute to mark a method as a data event handler method of a specific type
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerMethodAttribute : Attribute
    {
        /// <summary>
        ///     The type of the model data event the method can handle
        /// </summary>
        private DataOperationType OperationType { get; }

        /// <summary>
        ///     Creates new model event attribute of the specified event type
        /// </summary>
        /// <param name="dataOperationType"></param>
        public EventHandlerMethodAttribute(DataOperationType dataOperationType)
        {
            OperationType = dataOperationType;
        }
    }
}