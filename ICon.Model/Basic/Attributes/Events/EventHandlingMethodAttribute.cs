using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Attribute that marks a method as an event handler method which can handle a specififc type of data operation
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlingMethodAttribute : Attribute
    {

    }
}
