using System;
using System.Windows.Media;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     A <see cref="ViewModelBase" /> for managing display data of <see cref="ModelObjectGraph" /> instances
    /// </summary>
    public class ModelObject3DViewModel : ViewModelBase
    {
        private static string ResourceKey_Color => Resources.ResourceKey_ModelObject_RenderColor;
        private static string ResourceKey_Scaling => Resources.ResourceKey_ModelObject_RenderScaling;
        private static string ResourceKey_IsVisible => Resources.ResourceKey_ModelObject_RenderVisibilityFlag;
        private static Color ResourceDefault_Color => Colors.Gray;
        private static double ResourceDefault_Scaling => 1.0;

        /// <summary>
        ///     Get the <see cref="ModelObjectGraph" /> that the formatting is valid for
        /// </summary>
        public ModelObjectGraph ObjectGraph { get; }

        /// <summary>
        ///     Get or set the <see cref="System.Windows.Media.Color" /> for the object
        /// </summary>
        public Color Color
        {
            get => ObjectGraph.Resources.TryGetResource(ResourceKey_Color, x => VisualExtensions.ParseArgbHex(x), out var color)
                ? color
                : ResourceDefault_Color;
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
            get => ObjectGraph.Resources.TryGetResource(ResourceKey_Scaling, out double x) ? x : ResourceDefault_Scaling;
            set
            {
                ObjectGraph.Resources.SetResource(ResourceKey_Scaling, value);
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
        ///     Creates new <see cref="ModelObject3DViewModel" />
        /// </summary>
        /// <param name="objectGraph"></param>
        public ModelObject3DViewModel(ModelObjectGraph objectGraph)
        {
            ObjectGraph = objectGraph ?? throw new ArgumentNullException(nameof(objectGraph));
        }
    }
}