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
using Mocassin.UI.Xml.Base;
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
        private static string MaterialKey => Resources.ResourceKey_ModelObject_RenderMaterial;
        private static string ColorKey => Resources.ResourceKey_ModelObject_RenderColor;
        private static string MeshQualityKey => Resources.ResourceKey_ModelObject_MeshQuality;
        private static string UniformScalingKey => Resources.ResourceKey_ModelObject_RenderScaling;

        /// <summary>
        ///     Get the <see cref="Dictionary{TKey,TValue}" /> of supported <see cref="Material" /> items
        /// </summary>
        private static Dictionary<string, PhongMaterialCore> MaterialCatalog { get; }

        /// <inheritdoc />
        public MaterialCore Material
        {
            get => ObjectGraph.Resources.TryGetResource(MaterialKey, out string name)
                ? FindMaterial(name)
                : MaterialCatalog[nameof(PhongMaterials.DefaultVRML)];
            set
            {
                if (value.Name.Equals(Material.Name)) return;
                ObjectGraph.Resources.SetResource(MaterialKey, value.Name);
                OnMaterialChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public Color4 DxDiffuseColor
        {
            get => DiffuseColor.ToColor4();
            set
            {
                if (value.Equals(DxDiffuseColor)) return;
                OnDxDiffuseColorChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public Color DiffuseColor
        {
            get => ObjectGraph.Resources.TryGetResource(ColorKey, x => VisualExtensions.ParseRgbaHexToColor(x), out var color) ? color : Colors.Gray;
            set
            {
                if (value.Equals(DiffuseColor)) return;
                ObjectGraph.Resources.SetResource(ColorKey, value, color => color.ToRgbaHex());
                OnDiffuseColorChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public double MeshQuality
        {
            get => ObjectGraph.Resources.TryGetResource(MeshQualityKey, out double value) ? value : 1.0;
            set
            {
                if (value.AlmostEqualByRange(MeshQuality)) return;
                ObjectGraph.Resources.SetResource(MeshQualityKey, value);
                OnMaterialChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public double UniformScaling
        {
            get => ObjectGraph.Resources.TryGetResource(UniformScalingKey, out double value) ? value : 1.0;
            set
            {
                var oldValue = UniformScaling;
                if (value.AlmostEqualByRange(oldValue)) return;
                ObjectGraph.Resources.SetResource(UniformScalingKey, value);
                OnUniformScalingChanged(oldValue);
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
        public DxProjectMeshObjectSceneConfig(ExtensibleProjectObjectGraph objectGraph, VisualObjectCategory visualCategory)
            : base(objectGraph, visualCategory)
        {
        }

        /// <inheritdoc />
        public sealed override bool CheckSupport(SceneNode sceneNode)
        {
            return sceneNode is MeshNode || sceneNode is BatchedMeshNode;
        }

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
        ///     Action that is called if the <see cref="DxDiffuseColor " /> property changed
        /// </summary>
        protected virtual void OnDxDiffuseColorChanged()
        {
            DiffuseColor = DxDiffuseColor.ToColor();
        }

        /// <summary>
        ///     Action that is called if the<see cref="DiffuseColor" /> property changed
        /// </summary>
        protected virtual void OnDiffuseColorChanged()
        {
            OnMaterialChanged();
        }

        /// <summary>
        ///     Action that is called if the<see cref="MeshQuality" /> property changed
        /// </summary>
        protected virtual void OnMeshQualityChanged()
        {
            OnChangeInvalidatesNode?.Invoke();
        }

        /// <summary>
        ///     Action that is called if the<see cref="UniformScaling" /> property changed
        /// </summary>
        protected virtual void OnUniformScalingChanged(double oldValue)
        {
            //OnChangeInvalidatesNode?.Invoke();
            //return;
            // Note: Direct scaling currently not supported as the scaling operation is much more complex

            var rescalingMatrix = GetRescalingMatrix(oldValue, UniformScaling);
            foreach (var sceneNode in SceneNodes) ChangeNodeScaling(sceneNode, ref rescalingMatrix);
        }

        /// <summary>
        ///     Changes the scaling of the provided <see cref="SceneNode" /> using a scaling <see cref="Matrix" />
        /// </summary>
        /// <param name="sceneNode"></param>
        /// <param name="rescalingMatrix"></param>
        protected void ChangeNodeScaling(SceneNode sceneNode, ref Matrix rescalingMatrix)
        {
            switch (sceneNode)
            {
                case MeshNode meshNode:
                    var originShiftMatrix = Matrix.Translation(-meshNode.ModelMatrix.TranslationVector);
                    var backShift = Matrix.Translation(meshNode.ModelMatrix.TranslationVector);
                    meshNode.ModelMatrix *= originShiftMatrix * rescalingMatrix * backShift;
                    break;
                case BatchedMeshNode batchedNode:
                    var geometries = new BatchedMeshGeometryConfig[batchedNode.Geometries.Length];
                    for (var i = 0; i < geometries.Length; i++)
                    {
                        var config = batchedNode.Geometries[i];
                        geometries[i] = new BatchedMeshGeometryConfig(config.Geometry, config.ModelTransform * rescalingMatrix, config.MaterialIndex);
                    }

                    batchedNode.Geometries = geometries;
                    break;
            }
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
            switch (sceneNode)
            {
                case MeshNode meshNode:
                    meshNode.Material = material;
                    break;
                case BatchedMeshNode batchedNode:
                    batchedNode.Material = material;
                    break;
            }
        }

        /// <summary>
        ///     Updates the color of the passed <see cref="MaterialCore" /> to the current config settings
        /// </summary>
        /// <param name="core"></param>
        protected void UpdateMaterialColor(MaterialCore core)
        {
            if (core is PhongMaterialCore phongCore) phongCore.DiffuseColor = DxDiffuseColor;
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
            base.CopyCurrentValuesToNode(sceneNode);
        }
    }
}