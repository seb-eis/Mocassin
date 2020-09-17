using System;
using System.Globalization;
using System.Windows;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.ProjectBuilding;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding.DataControl.Converter
{
    /// <summary>
    ///     <see cref="ValueConverter" /> that converts the <see cref="LibraryBuildStatus" /> to a boolean value that defines
    ///     wherever the system is currently building
    /// </summary>
    public class BuildStatusVisibilityConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is LibraryBuildStatus buildStatus)) return Visibility.Collapsed;

            if (!(buildStatus == LibraryBuildStatus.Unknown ||
                  buildStatus == LibraryBuildStatus.BuildProcessCompleted ||
                  buildStatus == LibraryBuildStatus.Cancel ||
                  buildStatus.ToString().Contains("Error")))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }
    }
}