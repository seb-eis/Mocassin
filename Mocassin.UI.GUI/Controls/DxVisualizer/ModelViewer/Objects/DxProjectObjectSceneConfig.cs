using System;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Objects;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Objects
{
    /// <summary>
    ///     Base class for <see cref="IDxSceneItemConfig"/> implementations for <see cref="ExtensibleProjectObjectGraph"/>
    /// </summary>
    public abstract class DxProjectObjectSceneConfig : ViewModelBase, IDxSceneItemConfig, IDisposable
    {
        private Action onChangeInvalidatesNode;
        private SceneNode sceneNode;
        private static string IsInactiveKey => Resources.ResourceKey_ModelObject_RenderInactiveFlag;
        private static string IsVisibleKey => Resources.ResourceKey_ModelObject_RenderVisibilityFlag;
        private static string NameKey => Resources.ResourceKey_ModelObject_RenderDisplayName;

        /// <summary>
        ///     Get the <see cref="ExtensibleProjectObjectGraph"/> that provides the scene resources
        /// </summary>
        protected ExtensibleProjectObjectGraph ObjectGraph { get; }

        /// <inheritdoc />
        public SceneNode SceneNode
        {
            get => sceneNode;
            set => SetProperty(ref sceneNode, EnsureNullOrSupported(value), OnSceneNodeChanged);
        }

        /// <inheritdoc />
        public VisualObjectCategory VisualCategory { get; }

        /// <inheritdoc />
        public string Name
        {
            get => ObjectGraph.Resources.TryGetResource(NameKey, out string value) ? value : ObjectGraph.Name;
            set
            {
                if (Equals(Name, value)) return;
                ObjectGraph.Resources.SetResource(NameKey, value);
                OnNameChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public bool IsVisible
        {
            get => !ObjectGraph.Resources.TryGetResource(IsVisibleKey, out bool value) || value;
            set
            {
                if (value == IsVisible) return;
                ObjectGraph.Resources.SetResource(IsVisibleKey, value);
                OnIsVisibleChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public bool IsInactive
        {
            get => !ObjectGraph.Resources.TryGetResource(IsInactiveKey, out bool value) || value;
            set
            {
                if (value == IsInactive) return;
                ObjectGraph.Resources.SetResource(IsInactiveKey, value);
                OnIsInactiveChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public Action OnChangeInvalidatesNode
        {
            get => onChangeInvalidatesNode;
            set => SetProperty(ref onChangeInvalidatesNode, value);
        }

        /// <summary>
        ///     Creates a new <see cref="DxProjectObjectSceneConfig"/> for a <see cref="ExtensibleProjectObjectGraph"/>
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <param name="visualCategory"></param>
        protected DxProjectObjectSceneConfig(ExtensibleProjectObjectGraph objectGraph, VisualObjectCategory visualCategory)
        {
            ObjectGraph = objectGraph ?? throw new ArgumentNullException(nameof(objectGraph));
            VisualCategory = visualCategory;
        }

        /// <inheritdoc />
        public virtual bool Equals(IDxSceneItemConfig other)
        {
            return other != null && ReferenceEquals(this, other);
        }

        /// <inheritdoc />
        public virtual bool CheckAccess(object model)
        {
            return model != null && ReferenceEquals(model, ObjectGraph);
        }

        /// <summary>
        ///     Ensures that the provided object is null or an actually supported type. Throws an exception if the condition is not met
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private SceneNode EnsureNullOrSupported(SceneNode node)
        {
            if (node == null) return null;
            return CheckSupport(node) ? node : throw new NotSupportedException("The provided scene node is not supported by this class.");
        }

        /// <summary>
        ///     Performs implementation based behavior to check if the scene node is supported
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract bool CheckSupport(SceneNode node);

        /// <summary>
        ///     Action that is called when the <see cref="Name"/> property changed
        /// </summary>
        protected virtual void OnNameChanged()
        {
            SceneNode.Name = Name;
        }

        /// <summary>
        ///     Action that is called when the <see cref="IsVisible"/> property changed
        /// </summary>
        protected virtual void OnIsVisibleChanged()
        {
            SceneNode.Visible = IsVisible;
        }

        /// <summary>
        ///     Action that is called when the <see cref="IsInactive"/> property changed
        /// </summary>
        protected virtual void OnIsInactiveChanged()
        {
            OnChangeInvalidatesNode?.Invoke();
        }

        /// <summary>
        ///     Copies all data to the currently set scene node
        /// </summary>
        protected virtual void CopyValuesToSceneNode()
        {
            if (SceneNode == null) return;
            SceneNode.Name = Name;
            SceneNode.Visible = IsVisible;
        }

        /// <summary>
        ///     Action that is called when the <see cref="SceneNode"/> property changed
        /// </summary>
        private void OnSceneNodeChanged()
        {
            CopyValuesToSceneNode();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            OnChangeInvalidatesNode = null;
            SceneNode = null;
        }
    }
}