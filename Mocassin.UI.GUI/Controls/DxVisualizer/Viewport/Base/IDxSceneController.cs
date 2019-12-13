using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Base
{
    /// <summary>
    ///     Represents a controller for provision, setup and interaction with SharpDX based 3D scenes
    /// </summary>
    public interface IDxSceneController : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     Get a boolean value if the controller can invalidate the current scene
        /// </summary>
        bool CanInvalidateScene { get; }

        /// <summary>
        ///     Get the <see cref="IDxSceneHost" /> the controller manages
        /// </summary>
        IDxSceneHost SceneHost { get; }

        /// <summary>
        ///     Get the <see cref="ICommand"/> to invalidate the scene
        /// </summary>
        /// <returns></returns>
        ICommand InvalidateSceneCommand { get; }
    }
}