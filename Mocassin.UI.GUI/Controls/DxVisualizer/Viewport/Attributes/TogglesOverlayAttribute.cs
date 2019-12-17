using System;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Attributes
{
    /// <summary>
    ///     The <see cref="Attribute"/> to mark properties as having an overlay toggling effect. Use for boolean properties only
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TogglesOverlayAttribute : Attribute
    {
        /// <summary>
        ///     Get a boolean flag if the toggle should be unique
        /// </summary>
        public bool IsUnique { get; }

        /// <summary>
        ///     Creates a new <see cref="TogglesOverlayAttribute"/>. By default the toggle effect is marked as unique
        /// </summary>
        /// <param name="isUnique"></param>
        public TogglesOverlayAttribute(bool isUnique = true)
        {
            IsUnique = isUnique;
        }
    }
}