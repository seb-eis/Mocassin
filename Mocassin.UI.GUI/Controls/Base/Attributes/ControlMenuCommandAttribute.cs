using System;

namespace Mocassin.UI.GUI.Controls.Base.Attributes
{
    /// <summary>
    ///     <see cref="Attribute"/> to mark commands for inclusion in the control menu
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ControlMenuCommandAttribute : Attribute
    {
        /// <summary>
        ///     Get a display name <see cref="string"/>
        /// </summary>
        public string DisplayName { get; }

        /// <inheritdoc />
        public ControlMenuCommandAttribute(string displayName)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }
    }
}