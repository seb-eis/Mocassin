using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.Mathematics.Extensions;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Scene;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Data.Base;
using SharpDX;
using Color = System.Windows.Media.Color;
using Matrix = SharpDX.Matrix;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Scene
{
    /// <summary>
    ///     Implementation of the <see cref="DxProjectObjectSceneConfig" /> extended by the <see cref="IDxMeshItemConfig" />
    ///     interface
    /// </summary>
    public class DxProjectMeshObjectSceneConfig : DxProjectObjectSceneConfig, IDxMeshItemConfig
    {
        private bool canResizeMeshAtOrigin;
        private static string MaterialKey => Properties.Resources.ResourceKey_ModelObject_RenderMaterial;
        private static string ColorKey => Properties.Resources.ResourceKey_ModelObject_RenderColor;
        private static string MeshQualityKey => Properties.Resources.ResourceKey_ModelObject_MeshQuality;
        private static string UniformScalingKey => Properties.Resources.ResourceKey_ModelObject_RenderScaling;
        private static string IsWireframeVisibleKey => Properties.Resources.ResourceKey_ModelObject_RenderWireframeFlag;
        private static string WireframeColorKey => Properties.Resources.ResourceKey_ModelObject_RenderWireframeColor;

        /// <summary>
        ///     Get the <see cref="Dictionary{TKey,TValue}" /> of supported <see cref="Material" /> items
        /// </summary>
        internal static Dictionary<string, PhongMaterialCore> MaterialCatalog { get; }

        /// <inheritdoc />
        public bool CanResizeMeshAtOrigin
        {
            get => canResizeMeshAtOrigin;
            set => SetProperty(ref canResizeMeshAtOrigin, value);
        }

        /// <inheritdoc />
        public bool IsWireframeVisible
        {
            get => DataObject.Resources.TryGetResource(IsWireframeVisibleKey, out bool value) && value;
            set
            {
                if (value == IsWireframeVisible) return;
                DataObject.Resources.SetResource(IsWireframeVisibleKey, value);
                OnIsWireframeVisibleChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public MaterialCore Material
        {
            get => DataObject.Resources.TryGetResource(MaterialKey, out string name)
                ? FindMaterial(name)
                : MaterialCatalog[nameof(PhongMaterials.DefaultVRML)];
            set
            {
                if (value.Name.Equals(Material.Name)) return;
                DataObject.Resources.SetResource(MaterialKey, value.Name);
                OnMaterialChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public Color DiffuseColor
        {
            get => DataObject.Resources.TryGetResource(ColorKey, x => VisualExtensions.ParseRgbaHexToColor(x), out var color) ? color : Colors.Gray;
            set
            {
                if (value.Equals(DiffuseColor)) return;
                DataObject.Resources.SetResource(ColorKey, value, color => color.ToRgbaHex());
                OnDiffuseColorChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public Color WireframeColor
        {
            get => DataObject.Resources.TryGetResource(WireframeColorKey, x => VisualExtensions.ParseRgbaHexToColor(x), out var color) ? color : Colors.Gray;
            set
            {
                if (value.Equals(WireframeColor)) return;
                DataObject.Resources.SetResource(WireframeColorKey, value, color => color.ToRgbaHex());
                OnWireframeColorChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public double MeshQuality
        {
            get => DataObject.Resources.TryGetResource(MeshQualityKey, out double value) ? value : 1.0;
            set
            {
                if (value.AlmostEqualByRange(MeshQuality)) return;
                DataObject.Resources.SetResource(MeshQualityKey, value);
                OnMeshQualityChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public double MeshScaling
        {
            get => DataObject.Resources.TryGetResource(UniformScalingKey, out double value) ? value : GetDefaultScaling();
            set
            {
                var newValue = value < Settings.Default.Limit_Render_Scaling_Lower ? Settings.Default.Limit_Render_Scaling_Lower : value;
                var oldValue = MeshScaling;
                if (newValue.AlmostEqualByRange(oldValue)) return;
                DataObject.Resources.SetResource(UniformScalingKey, newValue);
                OnUniformScalingChanged(oldValue, newValue);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Static constructor that initializes the material catalog
        /// </summary>
        static DxProjectMeshObjectSceneConfig()
        {
            MaterialCatalog = Application.Current.Dispatcher?.Invoke(() => new PhongMaterialCollection()
                                                                           .Select(x => (PhongMaterialCore) x.Core).ToDictionary(x => x.Name));
            if (MaterialCatalog == null) throw new InvalidOperationException("Failed to setup the material catalog.");
        }

        /// <inheritdoc />
        public DxProjectMeshObjectSceneConfig(ExtensibleProjectDataObject dataObject, VisualObjectCategory visualCategory)
            : base(dataObject, visualCategory)
        {
        }

        /// <inheritdoc />
        public sealed override bool CheckSupport(SceneNode sceneNode) => sceneNode is MeshNode || sceneNode is BatchedMeshNode;

        /// <inheritdoc />
        public MaterialCore CreateMaterial()
        {
            var material = FindMaterial(Material.Name).Core;
            UpdateMaterialColor(material);
            return material;
        }

        /// <summary>
        ///     Finds a <see cref="MaterialCore" /> by its name. Returns a default value if the name cannot be found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Material FindMaterial(string name)
        {
            var material = MaterialCatalog.TryGetValue(name, out var result) ? result : MaterialCatalog[nameof(PhongMaterials.DefaultVRML)];
            return material.ConvertToPhongMaterial();
        }

        /// <summary>
        ///     Action that is called if the<see cref="DiffuseColor" /> property changed
        /// </summary>
        protected virtual void OnDiffuseColorChanged()
        {
            OnMaterialChanged();
        }

        /// <summary>
        ///     Action that is called if the<see cref="WireframeColor" /> property changed
        /// </summary>
        protected virtual void OnWireframeColorChanged()
        {
            var color = WireframeColor.ToColor4();
            foreach (var sceneNode in SceneNodes) ChangeNodeWireframeColor(sceneNode, color);
        }

        /// <summary>
        ///     Changes the color of the wireframe of a <see cref="SceneNode" />
        /// </summary>
        /// <param name="sceneNode"></param>
        /// <param name="color"></param>
        protected void ChangeNodeWireframeColor(SceneNode sceneNode, Color4 color)
        {
            UpdateNode(sceneNode, x => x.WireframeColor = color, x => x.WireframeColor = color);
        }

        /// <summary>
        ///     Action that is called if the<see cref="IsWireframeVisible" /> property changed
        /// </summary>
        protected virtual void OnIsWireframeVisibleChanged()
        {
            var flag = IsWireframeVisible;
            foreach (var sceneNode in SceneNodes) UpdateNode(sceneNode, x => x.RenderWireframe = flag, x => x.RenderWireframe = flag);
        }

        /// <summary>
        ///     Action that is called if the<see cref="MeshQuality" /> property changed
        /// </summary>
        protected virtual void OnMeshQualityChanged()
        {
            OnChangeInvalidatesNode?.Invoke();
        }

        /// <summary>
        ///     Action that is called if the<see cref="MeshScaling" /> property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnUniformScalingChanged(double oldValue, double newValue)
        {
            if (!CanResizeMeshAtOrigin)
            {
                OnChangeInvalidatesNode?.Invoke();
                return;
            }

            var rescalingMatrix = GetRescalingMatrix(oldValue, newValue);
            foreach (var sceneNode in SceneNodes) ChangeNodeScaling(sceneNode, ref rescalingMatrix);
        }

        /// <summary>
        ///     Changes the scaling of the provided <see cref="SceneNode" /> using a scaling <see cref="Matrix" /> and a temporary
        ///     origin shift
        /// </summary>
        /// <param name="sceneNode"></param>
        /// <param name="rescalingMatrix"></param>
        protected void ChangeNodeScaling(SceneNode sceneNode, ref Matrix rescalingMatrix)
        {
            switch (sceneNode)
            {
                case MeshNode meshNode:
                    meshNode.ModelMatrix = RescaleModelMatrixAtOrigin(meshNode.ModelMatrix, ref rescalingMatrix);
                    break;
                case BatchedMeshNode batchedNode:
                    var geometries = new BatchedMeshGeometryConfig[batchedNode.Geometries.Length];
                    for (var i = 0; i < geometries.Length; i++)
                    {
                        var config = batchedNode.Geometries[i];
                        var matrix = RescaleModelMatrixAtOrigin(config.ModelTransform, ref rescalingMatrix);
                        geometries[i] = new BatchedMeshGeometryConfig(config.Geometry, matrix, config.MaterialIndex);
                    }

                    batchedNode.Geometries = geometries;
                    break;
            }
        }

        /// <summary>
        ///     Incorporates a rescaling transform into a model <see cref="Matrix" /> using a temporary (0,0,0) translation
        ///     transform
        /// </summary>
        /// <param name="modelMatrix"></param>
        /// <param name="rescalingMatrix"></param>
        /// <returns></returns>
        protected Matrix RescaleModelMatrixAtOrigin(Matrix modelMatrix, ref Matrix rescalingMatrix)
        {
            var translationVector = modelMatrix.TranslationVector;
            modelMatrix.TranslationVector = Vector3.Zero;
            modelMatrix *= rescalingMatrix;
            modelMatrix.TranslationVector = translationVector;
            return modelMatrix;
        }

        /// <summary>
        ///     Action that is called if the <see cref="Material" /> property changed
        /// </summary>
        protected virtual void OnMaterialChanged()
        {
            var material = CreateMaterial();
            foreach (var sceneNode in SceneNodes) ChangeNodeMaterial(sceneNode, material);
        }

        /// <summary>
        ///     Changes the <see cref="MaterialCore" /> of the provided <see cref="SceneNode" />
        /// </summary>
        /// <param name="sceneNode"></param>
        /// <param name="material"></param>
        protected void ChangeNodeMaterial(SceneNode sceneNode, MaterialCore material)
        {
            var flag = DiffuseColor.A < byte.MaxValue;
            UpdateNode(sceneNode, x => x.Material = material, x => x.Material = material);
            UpdateNode(sceneNode, x => x.IsTransparent = flag, x => x.IsTransparent = flag);
        }

        /// <summary>
        ///     Updates the color of the passed <see cref="MaterialCore" /> to the current config settings
        /// </summary>
        /// <param name="core"></param>
        protected void UpdateMaterialColor(MaterialCore core)
        {
            if (core is PhongMaterialCore phongCore) phongCore.DiffuseColor = DiffuseColor.ToColor4();
        }

        /// <summary>
        ///     Calculates a model <see cref="Matrix" /> to change the scaling of the model
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected Matrix GetRescalingMatrix(double oldValue, double newValue)
        {
            var factor = newValue / oldValue;
            var matrix = Matrix.Scaling((float) factor);
            return matrix;
        }

        /// <inheritdoc />
        protected override void CopyCurrentValuesToNode(SceneNode sceneNode)
        {
            ChangeNodeMaterial(sceneNode, Material);
            ChangeNodeWireframeColor(sceneNode, WireframeColor.ToColor4());
            UpdateNode(sceneNode, x => x.RenderWireframe = IsWireframeVisible, x => x.RenderWireframe = IsWireframeVisible);
            base.CopyCurrentValuesToNode(sceneNode);
        }

        /// <summary>
        ///     Performs an action on a <see cref="SceneNode" /> depending on it being a <see cref="MeshNode" /> or a
        ///     <see cref="BatchedMeshNode" />
        /// </summary>
        /// <param name="sceneNode"></param>
        /// <param name="meshNodeAction"></param>
        /// <param name="batchedNodeAction"></param>
        protected void UpdateNode(SceneNode sceneNode, Action<MeshNode> meshNodeAction, Action<BatchedMeshNode> batchedNodeAction)
        {
            switch (sceneNode)
            {
                case MeshNode meshNode:
                    meshNodeAction(meshNode);
                    break;
                case BatchedMeshNode batchedMeshNode:
                    batchedNodeAction(batchedMeshNode);
                    break;
            }
        }

        /// <summary>
        ///     Get the default scaling value based on the <see cref="VisualObjectCategory" />
        /// </summary>
        /// <returns></returns>
        protected double GetDefaultScaling()
        {
            return VisualCategory switch
            {
                VisualObjectCategory.Sphere => Settings.Default.Default_Render_Sphere_Scaling,
                VisualObjectCategory.Cube => Settings.Default.Default_Render_Cube_Scaling,
                VisualObjectCategory.DoubleArrow => Settings.Default.Default_Render_Arrow_Scaling,
                VisualObjectCategory.SingleArrow => Settings.Default.Default_Render_Arrow_Scaling,
                VisualObjectCategory.Cylinder => Settings.Default.Default_Render_Cylinder_Scaling,
                _ => 1.0
            };
        }
    }
}