using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.GUI.Controls.Visualizer.DataControl
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> that manages the <see cref="ResourcesGraph" /> of typical render resources
    /// </summary>
    public class ModelRenderResourcesViewModel : ViewModelBase
    {
        private static string RenderAreaResourceKey => Resources.ResourceKey_ModelViewport_RenderArea;
        private static CultureInfo CultureDefault => CultureInfo.InvariantCulture;
        private static double[] RenderAreaDefault => new double[] {0, 0, 0, 1, 1, 1};
        private double[] renderAreaValues { get; }

        /// <summary>
        ///     Get the <see cref="ResourcesGraph" /> that is used as a data source
        /// </summary>
        private ResourcesGraph DataSource { get; set; }

        /// <summary>
        ///     Get the minimal render area value for direction 'A' (Fractional coordinates)
        /// </summary>
        public double RenderAreaMinA
        {
            get => renderAreaValues[0];
            set => SetRenderAreaValue(value, 0);
        }

        /// <summary>
        ///     Get the minimal render area value for direction 'B' (Fractional coordinates)
        /// </summary>
        public double RenderAreaMinB
        {
            get => renderAreaValues[1];
            set => SetRenderAreaValue(value, 1);
        }

        /// <summary>
        ///     Get the minimal render area value for direction 'C' (Fractional coordinates)
        /// </summary>
        public double RenderAreaMinC
        {
            get => renderAreaValues[2];
            set => SetRenderAreaValue(value, 2);
        }

        /// <summary>
        ///     Get the max render area value for direction 'A' (Fractional coordinates)
        /// </summary>
        public double RenderAreaMaxA
        {
            get => renderAreaValues[3];
            set => SetRenderAreaValue(value, 3);
        }

        /// <summary>
        ///     Get the max render area value for direction 'A' (Fractional coordinates)
        /// </summary>
        public double RenderAreaMaxB
        {
            get => renderAreaValues[4];
            set => SetRenderAreaValue(value, 4);
        }

        /// <summary>
        ///     Get the max render area value for direction 'A' (Fractional coordinates)
        /// </summary>
        public double RenderAreaMaxC
        {
            get => renderAreaValues[5];
            set => SetRenderAreaValue(value, 5);
        }

        /// <summary>
        ///     Creates a new <see cref="ModelRenderResourcesViewModel" />
        /// </summary>
        public ModelRenderResourcesViewModel()
        {
            DataSource = new ResourcesGraph();
            renderAreaValues = RenderAreaDefault;
        }

        /// <summary>
        ///     Changes the data source to another <see cref="ResourcesGraph"/>
        /// </summary>
        /// <param name="resources"></param>
        public void ChangeDataSource(ResourcesGraph resources)
        {
            DataSource = resources ?? new ResourcesGraph();
            SetRenderAreaNoSaving(DataSource.TryGetResource(RenderAreaResourceKey, ParseRenderAreaString, out var values) ? values : RenderAreaDefault);
        }

        /// <summary>
        ///     Returns the floored values of the render area using the provided <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public (int MinA, int MinB, int MinC, int MaxA, int MaxB, int MaxC) GetFlooredRenderArea(IEqualityComparer<double> comparer = null)
        {
            comparer ??= EqualityComparer<double>.Default;
            var minA = MocassinMath.FloorToInt(RenderAreaMinA, comparer);
            var minB = MocassinMath.FloorToInt(RenderAreaMinB, comparer);
            var minC = MocassinMath.FloorToInt(RenderAreaMinC, comparer);
            var maxA = MocassinMath.FloorToInt(RenderAreaMaxA, comparer);
            var maxB = MocassinMath.FloorToInt(RenderAreaMaxB, comparer);
            var maxC = MocassinMath.FloorToInt(RenderAreaMaxC, comparer);
            return (minA, minB, minC, maxA, maxB, maxC);
        }

        /// <summary>
        ///     Get the <see cref="Fractional3D"/> vector pair that describes the render cuboid
        /// </summary>
        /// <returns></returns>
        public (Fractional3D StartVector, Fractional3D EndVector) GetRenderCuboidVectors()
        {
            var startVector = new Fractional3D(RenderAreaMinA, RenderAreaMinB, RenderAreaMinC);
            var endVector = new Fractional3D(RenderAreaMaxA, RenderAreaMaxB, RenderAreaMaxC);
            return (startVector, endVector);
        }

        /// <summary>
        ///     Sets a new render area value at the specified index [0,5]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="callerMemberName"></param>
        private void SetRenderAreaValue(double value, int index, [CallerMemberName] string callerMemberName = default)
        {
            renderAreaValues[index] = value;
            DataSource.SetResource(Resources.ResourceKey_ModelViewport_RenderArea, renderAreaValues, CreateRenderAreaString);
            OnPropertyChanged(callerMemberName);
        }

        /// <summary>
        ///     Sets the complete render area to a new value without invoking the resource save operations
        /// </summary>
        /// <param name="values"></param>
        private void SetRenderAreaNoSaving(IReadOnlyList<double> values)
        {
            renderAreaValues[0] = values[0];
            renderAreaValues[1] = values[1];
            renderAreaValues[2] = values[2];
            renderAreaValues[3] = values[3];
            renderAreaValues[4] = values[4];
            renderAreaValues[5] = values[5];
            OnPropertyChanged(nameof(RenderAreaMinA));
            OnPropertyChanged(nameof(RenderAreaMinB));
            OnPropertyChanged(nameof(RenderAreaMinC));
            OnPropertyChanged(nameof(RenderAreaMaxA));
            OnPropertyChanged(nameof(RenderAreaMaxB));
            OnPropertyChanged(nameof(RenderAreaMaxC));
        }

        /// <summary>
        ///     Converts the render area double array to its string representation
        /// </summary>
        /// <returns></returns>
        private string CreateRenderAreaString(double[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length != 6) throw new ArgumentException("Render area contains more than 6 values.", nameof(values));
            var builder = new StringBuilder(100);
            foreach (var value in values)
            {
                builder.Append(value.ToString(CultureDefault));
                builder.Append(';');
            }

            builder.PopBack(1);
            return builder.ToString();
        }

        /// <summary>
        ///     Parses the render area string representation into a double array. Returns the defaults if the conversion fails
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static double[] ParseRenderAreaString(string str)
        {
            if (str == null) return RenderAreaDefault;
            var strValues = str.Split(';');
            if (strValues.Length != 6) return RenderAreaDefault;
            var result = new double[6];

            for (var i = 0; i < strValues.Length; i++)
            {
                if (!double.TryParse(strValues[i], NumberStyles.Float | NumberStyles.AllowThousands, CultureDefault, out var value))
                    return RenderAreaDefault;

                result[i] = value;
            }

            return result;
        }
    }
}