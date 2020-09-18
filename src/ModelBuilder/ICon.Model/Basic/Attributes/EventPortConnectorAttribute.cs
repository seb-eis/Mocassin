using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Attribute that marks a method as an event port connector method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class EventPortConnectorAttribute : Attribute
    {
    }
}