using System;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Attribute to uniquely mark classes as components for specific Mocassin simulation extensions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MocsimExtensionComponentAttribute : Attribute
    {
        /// <summary>
        ///     Get the <see cref="Guid" /> that identifies the simulator extension
        /// </summary>
        public Guid ExtensionGuid { get; }

        /// <summary>
        ///     Get the extension alias <see cref="string" />
        /// </summary>
        public string ExtensionAlias { get; }

        /// <summary>
        ///     Creates new <see cref="MocsimExtensionComponentAttribute" /> from a guid and alias <see cref="string" />
        /// </summary>
        /// <param name="guidStr"></param>
        /// <param name="extensionAlias"></param>
        public MocsimExtensionComponentAttribute(string guidStr, string extensionAlias)
        {
            ExtensionAlias = extensionAlias ?? throw new ArgumentNullException(nameof(extensionAlias));
            ExtensionGuid = new Guid(guidStr);
        }
    }
}