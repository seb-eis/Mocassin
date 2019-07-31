using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     A <see cref="ViewModelBase" /> for managing display data of <see cref="ModelObjectGraph" /> instances
    /// </summary>
    public class ModelObject3DViewModel : ViewModelBase
    {
        private static string ColorPropertyName { get; } = "Color3D";
        private static string ScalingPropertyName { get; } = "Scaling3D";
        private static Color ColorPropertyDefault { get; } = Colors.Gray;
        private static double ScalingPropertyDefault { get; } = 1.0;

        /// <summary>
        ///     Get the <see cref="ModelObjectGraph" /> that the formatting is valid for
        /// </summary>
        public ModelObjectGraph ObjectGraph { get; }

        /// <summary>
        ///     Get or set the <see cref="Color" /> for the object
        /// </summary>
        public Color ObjectColor
        {
            get
            {
                var value = ObjectGraph.GetAttachedProperty(ColorPropertyName);
                return VisualExtensions.TryParseArgbHex(value as string, out var x) ? x : ColorPropertyDefault;
            }
            set
            {
                ObjectGraph.SetAttachedProperty(ColorPropertyName, value.ToArgbHex());
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the scaling factor for the object
        /// </summary>
        public double ObjectScaling
        {
            get
            {
                var value = ObjectGraph.GetAttachedProperty(ScalingPropertyName);
                if (value is double x) return x;
                return ScalingPropertyDefault;
            }
            set
            {
                ObjectGraph.SetAttachedProperty(ScalingPropertyName, value);
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