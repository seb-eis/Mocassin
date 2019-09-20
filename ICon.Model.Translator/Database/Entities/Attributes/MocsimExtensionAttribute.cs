using System;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Attribute to uniquely mark classes as components for specific Mocassin simulation extensions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MocsimExtensionAttribute : Attribute
    {
        /// <summary>
        ///     Get the <see cref="Guid"/> that identifies the simulator extension
        /// </summary>
        public Guid ExtensionGuid { get; }

        /// <summary>
        ///     Creates new <see cref="MocsimExtensionAttribute"/> from a guid <see cref="string"/>
        /// </summary>
        /// <param name="guidStr"></param>
        public MocsimExtensionAttribute(string guidStr)
        {
            ExtensionGuid = new Guid(guidStr);
        }
    }
}