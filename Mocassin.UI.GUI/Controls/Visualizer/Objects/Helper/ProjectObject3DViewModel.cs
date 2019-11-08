using System;
using System.Collections.Generic;
using System.Windows.Media;
using Mocassin.Framework.Constraints;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Enum for defining the visual category of a project object 3D item
    /// </summary>
    public enum VisualObjectCategory
    {
        Unknown,
        Frame,
        Position,
        Transition,
        Interaction,
        Cluster
    }

    /// <summary>
    ///     A <see cref="ViewModelBase" /> for managing display data resources on <see cref="ExtensibleProjectObjectGraph" /> instances
    /// </summary>
    public class ProjectObject3DViewModel : ViewModelBase
    {
        private static Dictionary<VisualObjectCategory, (Color, double, double)> ResourceDefaults { get; }

        private static string ResourceKey_Color => Resources.ResourceKey_ModelObject_RenderColor;
        private static string ResourceKey_Scaling => Resources.ResourceKey_ModelObject_RenderScaling;
        private static string ResourceKey_IsVisible => Resources.ResourceKey_ModelObject_RenderVisibilityFlag;
        private static string ResourceKey_MeshQuality => Resources.ResourceKey_ModelObject_MeshQuality;

        /// <summary>
        ///     Get the <see cref="ExtensibleProjectObjectGraph" /> that the formatting is valid for
        /// </summary>
        public ExtensibleProjectObjectGraph ObjectGraph { get; }

        /// <summary>
        ///     Get the <see cref="VisualObjectCategory"/> of the view model
        /// </summary>
        public VisualObjectCategory ObjectCategory { get; }

        /// <summary>
        ///     Get or set the <see cref="System.Windows.Media.Color" /> for the object
        /// </summary>
        public Color Color
        {
            get => ObjectGraph.Resources.TryGetResource(ResourceKey_Color, x => VisualExtensions.ParseArgbHex(x), out var color)
                ? color
                : GetRenderDefaults(ObjectCategory).Color;
            set
            {
                ObjectGraph.Resources.SetResource(ResourceKey_Color, value, x => x.ToArgbHex());
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the scaling factor for the object
        /// </summary>
        public double Scaling
        {
            get => ObjectGraph.Resources.TryGetResource(ResourceKey_Scaling, out double x) ? x : GetRenderDefaults(ObjectCategory).Scaling;
            set
            {
                ObjectGraph.Resources.SetResource(ResourceKey_Scaling, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the quality of the used object mesh as a fraction of the internal default
        /// </summary>
        public double MeshQuality
        {
            get => ObjectGraph.Resources.TryGetResource(ResourceKey_MeshQuality, out double x) ? x : GetRenderDefaults(ObjectCategory).MeshQuality;
            set
            {
                var tmp = ValueConstraint<double>.EnsureLimit(value, Settings.Default.Limit_Render_MeshQuality_Lower,
                    Settings.Default.Limit_Render_MeshQuality_Upper);
                ObjectGraph.Resources.SetResource(ResourceKey_MeshQuality, tmp);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set a boolean flag if the object should be visible
        /// </summary>
        public bool IsVisible
        {
            get => ObjectGraph.Resources.TryGetResource(ResourceKey_IsVisible, out bool value) && value;
            set
            {
                ObjectGraph.Resources.SetResource(ResourceKey_IsVisible, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Creates new <see cref="ProjectObject3DViewModel" /> from the provided <see cref="ExtensibleProjectObjectGraph"/> and <see cref="VisualObjectCategory"/>
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <param name="objectCategory"></param>
        public ProjectObject3DViewModel(ExtensibleProjectObjectGraph objectGraph, VisualObjectCategory objectCategory)
        {
            ObjectGraph = objectGraph ?? throw new ArgumentNullException(nameof(objectGraph));
            ObjectCategory = objectCategory;
        }

        /// <summary>
        ///     Gets the default render resource information of a <see cref="VisualObjectCategory"/> used as the fallback values
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static (Color Color, double Scaling, double MeshQuality) GetRenderDefaults(VisualObjectCategory category)
        {
            switch (category)
            {
                case VisualObjectCategory.Unknown:
                    return (Colors.Gray, 1.0, 1.0);
                case VisualObjectCategory.Frame:
                    return (Colors.Black, 1.0, 0);
                case VisualObjectCategory.Position:
                    return (Colors.Gray, .5, .5);
                case VisualObjectCategory.Transition:
                    return (Colors.Gray, .2, .5);
                case VisualObjectCategory.Interaction:
                    return (Colors.Transparent, 2.0, 0);
                case VisualObjectCategory.Cluster:
                    return (Colors.Gray, 0, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}