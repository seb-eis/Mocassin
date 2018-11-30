using System;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Attribute that marks a method as a data injection method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InjectionMethodAttribute : Attribute
    {
        /// <summary>
        ///     Get or set the order the method should have in the injection process
        /// </summary>
        public int Order { get; set; }
    }
}