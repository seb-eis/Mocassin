using System;
using System.Windows.Media;
using Mocassin.Framework.Constraints;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     A <see cref="ViewModelBase" /> for managing display data resources on <see cref="ExtensibleProjectObjectGraph" />
    ///     instances
    /// </summary>
    public class ObjectVisualResourcesViewModel : ViewModelBase, IObjectSceneConfig
    {
        private static string ColorResourceKey => Resources.ResourceKey_ModelObject_RenderColor;
        private static string ScalingResourceKey => Resources.ResourceKey_ModelObject_RenderScaling;
        private static string VisibilityResourceKey => Resources.ResourceKey_ModelObject_RenderVisibilityFlag;
        private static string MeshQualityResourceKey => Resources.ResourceKey_ModelObject_MeshQuality;
        private static string MaterialResourceKey => Resources.ResourceKey_ModelObject_RenderMaterial;

        /// <summary>
        ///     Get the <see cref="ExtensibleProjectObjectGraph" /> that the formatting is valid for
        /// </summary>
        public ExtensibleProjectObjectGraph ObjectGraph { get; }

        /// <summary>
        ///     Get the <see cref="VisualObjectCategory" /> of the view model
        /// </summary>
        public VisualObjectCategory VisualCategory { get; }

        /// <summary>
        ///     Get or set the <see cref="System.Windows.Media.Color" /> for the object
        /// </summary>
        public Color Color
        {
            get => ObjectGraph.Resources.TryGetResource(ColorResourceKey, x => VisualExtensions.ParseRgbaHexToColor(x), out var color)
                ? color
                : GetRenderDefaults(VisualCategory).Color;
            set
            {
                ObjectGraph.Resources.SetResource(ColorResourceKey, value, x => x.ToRgbaHex());
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the name of the used material (Null is the default value)
        /// </summary>
        public string MaterialName
        {
            get => ObjectGraph.Resources.TryGetResource(MaterialResourceKey, out string value) ? value : null;
            set
            {
                ObjectGraph.Resources.SetResource(MaterialResourceKey, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the scaling factor for the object
        /// </summary>
        public double Scaling
        {
            get => ObjectGraph.Resources.TryGetResource(ScalingResourceKey, out double x) ? x : GetRenderDefaults(VisualCategory).Scaling;
            set
            {
                ObjectGraph.Resources.SetResource(ScalingResourceKey, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the quality of the used object mesh as a fraction of the internal default
        /// </summary>
        public double Quality
        {
            get => ObjectGraph.Resources.TryGetResource(MeshQualityResourceKey, out double x) ? x : GetRenderDefaults(VisualCategory).MeshQuality;
            set
            {
                var tmp = ValueConstraint<double>.EnsureLimit(value, Settings.Default.Limit_Render_MeshQuality_Lower,
                    Settings.Default.Limit_Render_MeshQuality_Upper);
                ObjectGraph.Resources.SetResource(MeshQualityResourceKey, tmp);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set a boolean flag if the object should be visible
        /// </summary>
        public bool IsVisible
        {
            get => ObjectGraph.Resources.TryGetResource(VisibilityResourceKey, out bool value) && value;
            set
            {
                ObjectGraph.Resources.SetResource(VisibilityResourceKey, value);
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public bool IsApplicable(object obj)
        {
            if (obj == null || !(obj is ExtensibleProjectObjectGraph objectGraph)) return false;
            return ReferenceEquals(ObjectGraph, objectGraph);
        }

        /// <summary>
        ///     Creates new <see cref="ObjectVisualResourcesViewModel" /> from the provided
        ///     <see cref="ExtensibleProjectObjectGraph" />
        ///     and <see cref="VisualObjectCategory" />
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <param name="visualCategory"></param>
        public ObjectVisualResourcesViewModel(ExtensibleProjectObjectGraph objectGraph, VisualObjectCategory visualCategory)
        {
            ObjectGraph = objectGraph ?? throw new ArgumentNullException(nameof(objectGraph));
            VisualCategory = visualCategory;
        }

        /// <summary>
        ///     Gets the default render resource information of a <see cref="VisualObjectCategory" /> used as the fallback values
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static (Color Color, double Scaling, double MeshQuality) GetRenderDefaults(VisualObjectCategory category)
        {
            return category switch
            {
                VisualObjectCategory.Unknown => (Colors.Gray, 1.0, 1.0),
                VisualObjectCategory.Frame => (Colors.Black, 1.0, 0),
                VisualObjectCategory.Sphere => (Colors.Gray, .5, .5),
                VisualObjectCategory.DoubleArrow => (Colors.Gray, .2, .5),
                VisualObjectCategory.Line => (Colors.Transparent, 2.0, 0),
                VisualObjectCategory.PolygonSet => (Colors.Gray, 0, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }

        /// <inheritdoc />
        public bool Equals(IObjectSceneConfig other)
        {
            if (other == null) return false;
            return ReferenceEquals(this, other) || ReferenceEquals(ObjectGraph, (other as ObjectVisualResourcesViewModel)?.ObjectGraph);
        }
    }
}