using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Attribute that marks a class as a data update event handler which provides handler pipelines and a port connection
    ///     function
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UpdateHandlerAttribute : Attribute
    {
        /// <summary>
        ///     Defines the type of the event port source that the handler is designed to handle
        /// </summary>
        public Type EventSourceType { get; }

        /// <summary>
        ///     Create new data update event handler attribute with the defines type of event port
        /// </summary>
        /// <param name="eventSourceType"></param>
        public UpdateHandlerAttribute(Type eventSourceType)
        {
            EventSourceType = eventSourceType ?? throw new ArgumentNullException(nameof(eventSourceType));
        }
    }
}