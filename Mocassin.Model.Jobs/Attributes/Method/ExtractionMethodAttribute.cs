using System;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Attribute that marks a method as an extraction method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExtractionMethodAttribute : Attribute
    {
        /// <summary>
        ///     Get or set the order the method should have in the extraction process
        /// </summary>
        public int Order { get; set; }
    }
}