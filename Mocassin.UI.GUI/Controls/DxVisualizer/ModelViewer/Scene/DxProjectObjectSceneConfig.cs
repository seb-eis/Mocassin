using System;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Scene;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Scene
{
    /// <summary>
    ///     Base class for <see cref="IDxSceneItemConfig" /> implementations for <see cref="ExtensibleProjectDataObject" />
    /// </summary>
    public abstract class DxProjectObjectSceneConfig : ViewModelBase, IDxSceneItemConfig, IDisposable
    {
        private string description;
        private Action onChangeInvalidatesNode;
        private static string IsInactiveKey => Properties.Resources.ResourceKey_ModelObject_RenderInactiveFlag;
        private static string IsVisibleKey => Properties.Resources.ResourceKey_ModelObject_RenderVisibilityFlag;
        private static string NameKey => Properties.Resources.ResourceKey_ModelObject_RenderDisplayName;

        /// <summary>
        ///     Get the <see cref="ExtensibleProjectDataObject" /> that provides the scene resources
        /// </summary>
        protected ExtensibleProjectDataObject DataObject { get; }

        /// <summary>
        ///     Get the <see cref="HashSet{T}" /> of managed <see cref="SceneNode" /> instances
        /// </summary>
        protected HashSet<SceneNode> SceneNodes { get; }

        /// <inheritdoc />
        public VisualObjectCategory VisualCategory { get; }

        /// <inheritdoc />
        public string Name
        {
            get => DataObject.Resources.TryGetResource(NameKey, out string value) ? value : DataObject.Name;
            set
            {
                if (Equals(Name, value)) return;
                DataObject.Resources.SetResource(NameKey, value);
                OnNameChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        /// <inheritdoc />
        public bool IsVisible
        {
            get => DataObject.Resources.TryGetResource(IsVisibleKey, out bool value) && value;
            set
            {
                if (value == IsVisible) return;
                DataObject.Resources.SetResource(IsVisibleKey, value);
                OnIsVisibleChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public bool IsInactive
        {
            get => !DataObject.Resources.TryGetResource(IsInactiveKey, out bool value) || value;
            set
            {
                if (value == IsInactive) return;
                DataObject.Resources.SetResource(IsInactiveKey, value);
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
        ///     Creates a new <see cref="DxProjectObjectSceneConfig" /> for a <see cref="ExtensibleProjectDataObject" />
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="visualCategory"></param>
        protected DxProjectObjectSceneConfig(ExtensibleProjectDataObject dataObject, VisualObjectCategory visualCategory)
        {
            DataObject = dataObject ?? throw new ArgumentNullException(nameof(dataObject));
            VisualCategory = visualCategory;
            SceneNodes = new HashSet<SceneNode>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DetachAll();
            OnChangeInvalidatesNode = null;
        }

        /// <inheritdoc />
        public virtual bool Equals(IDxSceneItemConfig other) => other != null && ReferenceEquals(this, other);

        /// <inheritdoc />
        public virtual bool CheckAccess(object model) => model != null && ReferenceEquals(model, DataObject);

        /// <summary>
        ///     Performs implementation based behavior to check if the scene node is supported
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract bool CheckSupport(SceneNode node);

        /// <inheritdoc />
        public void AttachNode(SceneNode sceneNode, bool isPreConfigured = false)
        {
            ThrowIfNodeNullOrNotSupported(sceneNode);
            if (SceneNodes.Contains(sceneNode)) return;
            if (!isPreConfigured) CopyCurrentValuesToNode(sceneNode);
            SceneNodes.Add(sceneNode);
        }

        /// <inheritdoc />
        public void DetachNode(SceneNode sceneNode)
        {
            SceneNodes.Remove(sceneNode);
        }

        /// <inheritdoc />
        public void DetachAll()
        {
            SceneNodes.Clear();
        }

        /// <summary>
        ///     Ensures that the provided <see cref="SceneNode" /> is not null and a supported a type. Throws an exception if the
        ///     condition is not met
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void ThrowIfNodeNullOrNotSupported(SceneNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!CheckSupport(node)) throw new NotSupportedException($"The provided scene node ({node.GetType()}) is not supported.");
        }

        /// <summary>
        ///     Action that is called when the <see cref="Name" /> property changed
        /// </summary>
        protected virtual void OnNameChanged()
        {
            foreach (var sceneNode in SceneNodes) sceneNode.Name = Name;
        }

        /// <summary>
        ///     Action that is called when the <see cref="IsVisible" /> property changed
        /// </summary>
        protected virtual void OnIsVisibleChanged()
        {
            foreach (var sceneNode in SceneNodes) sceneNode.Visible = IsVisible;
        }

        /// <summary>
        ///     Action that is called when the <see cref="IsInactive" /> property changed
        /// </summary>
        protected virtual void OnIsInactiveChanged()
        {
            OnChangeInvalidatesNode?.Invoke();
        }

        /// <summary>
        ///     Copies the currently set properties to the passed <see cref="SceneNode" />
        /// </summary>
        /// <param name="sceneNode"></param>
        protected virtual void CopyCurrentValuesToNode(SceneNode sceneNode)
        {
            if (sceneNode == null) throw new ArgumentNullException(nameof(sceneNode));
            sceneNode.Name = Name;
            sceneNode.Visible = IsVisible;
        }
    }
}