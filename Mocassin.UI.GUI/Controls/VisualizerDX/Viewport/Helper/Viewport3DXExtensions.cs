using System.Windows;
using HelixToolkit.Wpf.SharpDX;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper
{
    /// <summary>
    ///     Provides extensions for the helix toolkit SharpDX viewport
    /// </summary>
    public static class Viewport3DXExtensions
    {
        /// <summary>
        ///  Attached <see cref="DependencyProperty"/> to disable MSAA during user interaction with a <see cref="Viewport3DX"/>
        /// </summary>
        public static readonly DependencyProperty DisableMsaaOnInteractionProperty = DependencyProperty.RegisterAttached("DisableMssaOnInteraction", typeof(bool), typeof(Viewport3DXExtensions), new PropertyMetadata(true));

        /// <summary>
        ///  Attached <see cref="DependencyProperty"/> to define if a user is interacting with a <see cref="Viewport3DX"/>
        /// </summary>
        public static readonly DependencyProperty IsInteractingProperty = DependencyProperty.RegisterAttached("IsInteracting", typeof(bool), typeof(Viewport3DXExtensions), new PropertyMetadata(false));

        /// <summary>
        ///     Set the <see cref="DisableMsaaOnInteractionProperty"/> for a <see cref="Viewport3DX"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetDisableMssaOnInteraction(DependencyObject element, bool value)
        {
            element.SetValue(DisableMsaaOnInteractionProperty, value);
        }

        /// <summary>
        ///     Get the <see cref="DisableMsaaOnInteractionProperty"/> for a <see cref="Viewport3DX"/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetDisableMssaOnInteraction(DependencyObject element)
        {
            return (bool) element.GetValue(DisableMsaaOnInteractionProperty);
        }

        /// <summary>
        ///     Set the <see cref="IsInteractingProperty"/> for a <see cref="Viewport3DX"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetIsInteracting(DependencyObject element, bool value)
        {
            element.SetValue(IsInteractingProperty, value);
        }

        /// <summary>
        ///     Get the <see cref="IsInteractingProperty"/> for a <see cref="Viewport3DX"/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetIsInteracting(DependencyObject element)
        {
            return (bool) element.GetValue(IsInteractingProperty);
        }
    }
}