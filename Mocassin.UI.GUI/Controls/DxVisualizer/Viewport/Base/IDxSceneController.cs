using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Base.Objects;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Base
{
    /// <summary>
    ///     Represents a control interface for provision, setup and user interaction with SharpDX based 3D scene content
    /// </summary>
    public interface IDxSceneController : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     Get a boolean value if the controller can invalidate the current scene
        /// </summary>
        bool CanInvalidateScene { get; }

        /// <summary>
        ///     Get a boolean flag if the currently supplied scene is invalid and requires invalidation
        /// </summary>
        bool IsInvalidScene { get; }

        /// <summary>
        ///     Get the <see cref="IDxSceneHost" /> the controller supplies to
        /// </summary>
        IDxSceneHost SceneHost { get; }

        /// <summary>
        ///     Get the <see cref="ICommand"/> to invalidate the scene
        /// </summary>
        /// <returns></returns>
        ICommand InvalidateSceneCommand { get; }

        /// <summary>
        ///     Get a set of <see cref="VvmContainer"/> instances that supply scene interaction controls
        /// </summary>
        /// <returns></returns>
        IEnumerable<VvmContainer> GetControlContainers();
    }
}