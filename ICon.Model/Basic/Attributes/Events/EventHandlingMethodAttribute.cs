using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Attribute that marks a method as an event handler method which can handle a specific type of data operation
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlingMethodAttribute : Attribute
    {
    }
}