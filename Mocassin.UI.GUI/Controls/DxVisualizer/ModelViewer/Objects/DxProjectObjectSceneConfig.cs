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
    public abstract class DxProjectObjectSceneConfig : ViewModelBase, IDxSceneItemConfig
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
            set => SetProperty(ref sceneNode, EnsureSupported(value));
        }

        /// <inheritdoc />
        public abstract VisualObjectCategory VisualCategory { get; }

        /// <inheritdoc />
        public string Name
        {
            get => ObjectGraph.Resources.TryGetResource(NameKey, out string value) ? value : ObjectGraph.Name;
            set
            {
                if (Equals(Name, value)) return;
                ObjectGraph.Resources.SetResource(NameKey, value);
                OnPropertyChanged();
                SceneNode.Name = value;
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
                OnPropertyChanged();
                SceneNode.Visible = value;
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
                OnPropertyChanged();
                OnChangeInvalidatesNode?.Invoke();
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
        protected DxProjectObjectSceneConfig(ExtensibleProjectObjectGraph objectGraph)
        {
            ObjectGraph = objectGraph ?? throw new ArgumentNullException(nameof(objectGraph));
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
        ///     Ensures that the provided object is actually supported or throws an exception otherwise
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private SceneNode EnsureSupported(SceneNode node)
        {
            return CheckSupport(node) ? node : throw new NotSupportedException("The provided scene node is not supported by this class.");
        }

        /// <summary>
        ///     Performs implementation based behavior to check if the scene node is supported
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected abstract bool CheckSupport(SceneNode node);
    }
}