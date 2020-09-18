using System;
using System.Collections.Generic;
using System.ComponentModel;
using HelixToolkit.Wpf.SharpDX;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Enums;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Base
{
    /// <summary>
    ///     Represents a host for a SharpDX based 3D scene elements that provides basic scene manipulation functionality
    /// </summary>
    public interface IDxSceneHost : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="DxSceneBatchingMode" /> preference the scene host has
        /// </summary>
        DxSceneBatchingMode SceneBatchingMode { get; }

        /// <summary>
        ///     Get the <see cref="IDxSceneController" /> that manages the scene
        /// </summary>
        IDxSceneController SceneController { get; }

        /// <summary>
        ///     Resets the scene to default settings and clears all non-default elements
        /// </summary>
        void ResetScene(bool resetCamera);

        /// <summary>
        ///     Clears the scene removing all elements without resetting
        /// </summary>
        void ClearScene();

        /// <summary>
        ///     Resets the camera of the host
        /// </summary>
        void ResetCamera();

        /// <summary>
        ///     Adds an <see cref="Element3D" /> to the scene
        /// </summary>
        /// <param name="element"></param>
        void AddSceneItem(Element3D element);

        /// <summary>
        ///     Adds a sequence of <see cref="Element3D" /> to the scene
        /// </summary>
        /// <param name="elements"></param>
        void AddSceneItems(IEnumerable<Element3D> elements);

        /// <summary>
        ///     Removes a <see cref="Element3D" /> from the scene. Returns true if an element was actually removed
        /// </summary>
        /// <param name="element"></param>
        bool RemoveSceneItem(Element3D element);

        /// <summary>
        ///     Attaches the provided <see cref="IDxSceneController" /> to the host and detaches the old if required
        /// </summary>
        /// <param name="controller"></param>
        void AttachController(IDxSceneController controller);

        /// <summary>
        ///     Detaches the current <see cref="IDxSceneController" /> from the host
        /// </summary>
        void DetachController();
    }
}