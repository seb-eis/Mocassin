using System;
using System.ComponentModel;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Objects
{
    /// <summary>
    ///     Represents a view model for manipulation and configuration of display/generation of DX scene items
    /// </summary>
    public interface IDxSceneItemConfig : IEquatable<IDxSceneItemConfig>, INotifyPropertyChanged
    {
        /// <summary>
        ///     Get or set a name for the item
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Get or set boolean flag if the scene item is visible
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if the item is inactive
        /// </summary>
        bool IsInactive { get; set; }

        /// <summary>
        ///     Get the <see cref="VisualObjectCategory" /> that describes the contents of the scene node
        /// </summary>
        VisualObjectCategory VisualCategory { get; }

        /// <summary>
        ///     Get or set a callback <see cref="Action" /> that the config should invoke if a change is made that invalidates the
        ///     scene node
        /// </summary>
        Action OnChangeInvalidatesNode { get; set; }

        /// <summary>
        ///     Check if the <see cref="IDxLineItemConfig" /> has access to the model data stored in the provided object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool CheckAccess(object model);

        /// <summary>
        ///     Check if a specific type of <see cref="SceneNode" /> is supported
        /// </summary>
        /// <param name="sceneNode"></param>
        /// <returns></returns>
        bool CheckSupport(SceneNode sceneNode);

        /// <summary>
        ///     Attaches the provided <see cref="SceneNode" /> and begins managing its properties
        /// </summary>
        /// <param name="sceneNode"></param>
        void AttachNode(SceneNode sceneNode);

        /// <summary>
        ///     Detaches the provided <see cref="SceneNode" /> and stops managing its properties
        /// </summary>
        /// <param name="sceneNode"></param>
        void DetachNode(SceneNode sceneNode);

        /// <summary>
        ///     Detaches all currently attached <see cref="SceneNode" /> instances
        /// </summary>
        void DetachAll();
    }
}