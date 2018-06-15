using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Attribute that marks a method as an event port connector method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class EventPortConnectorAttribute : Attribute
    {
        
    }
}
