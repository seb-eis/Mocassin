using System;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Attributes
{
    /// <summary>
    ///     Custom <see cref="Attribute" /> that marks a property change should cause render invalidation. Should be used for
    ///     view model attributes that are nor automatically handled by the viewport
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RaiseInvalidateRenderAttribute : Attribute
    {
        /// <summary>
        ///     Get the delay <see cref="TimeSpan" /> after which render invalidation should be raised. Default is 100ms
        /// </summary>
        public TimeSpan Delay { get; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        ///     Creates a new <see cref="RaiseInvalidateRenderAttribute" /> that delays the raise by 100 ms
        /// </summary>
        public RaiseInvalidateRenderAttribute()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="RaiseInvalidateRenderAttribute" /> that delays the raise by the provided
        ///     <see cref="TimeSpan" />
        /// </summary>
        /// <param name="delay"></param>
        public RaiseInvalidateRenderAttribute(TimeSpan delay)
        {
            Delay = delay;
        }
    }
}