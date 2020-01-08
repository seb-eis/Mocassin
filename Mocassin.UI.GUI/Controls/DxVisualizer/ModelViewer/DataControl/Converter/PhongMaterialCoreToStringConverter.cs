using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Scene;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.DataControl.Converter
{
    /// <summary>
    ///     A <see cref="ValueConverter"/> implementation to convert from strings to phong materials 
    /// </summary>
    public class PhongMaterialCoreToStringConverter : ValueConverter
    {
        /// <summary>
        ///     Get the <see cref="IReadOnlyDictionary{TKey,TValue}"/> of known material names and <see cref="PhongMaterialCore"/> instances
        /// </summary>
        public static IReadOnlyDictionary<string, PhongMaterialCore> MaterialCatalog { get; }

        /// <summary>
        ///     Get the <see cref="IReadOnlyCollection{T}"/> of known material names
        /// </summary>
        public static IReadOnlyCollection<string> MaterialNameCollection { get; }

        /// <summary>
        ///         Static constructor that initializes the material catalog
        /// </summary>
        static PhongMaterialCoreToStringConverter()
        {
            MaterialCatalog = new ReadOnlyDictionary<string, PhongMaterialCore>(DxProjectMeshObjectSceneConfig.MaterialCatalog);
            MaterialNameCollection = MaterialCatalog.Keys.ToList(MaterialCatalog.Count);
        }

        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as PhongMaterialCore)?.Name ?? nameof(PhongMaterials.DefaultVRML);
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MaterialCatalog[value as string ?? nameof(PhongMaterials.DefaultVRML)];
        }
    }
}