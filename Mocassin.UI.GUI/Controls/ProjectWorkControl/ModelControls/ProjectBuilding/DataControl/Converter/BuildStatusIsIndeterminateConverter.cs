using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.ProjectBuilding;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding.DataControl.Converter
{
    /// <summary>
    ///     <see cref="ValueConverter" /> that converts the <see cref="LibraryBuildStatus" /> to a progress bar determinate
    ///     boolean value if the jobs are being build
    /// </summary>
    public class BuildStatusIsIndeterminateConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LibraryBuildStatus buildStatus)
            {
                return !(buildStatus == LibraryBuildStatus.Unknown ||
                         buildStatus == LibraryBuildStatus.BuildProcessCompleted ||
                         buildStatus == LibraryBuildStatus.BuildingLibrary ||
                         buildStatus == LibraryBuildStatus.Cancel ||
                         buildStatus.ToString().Contains("Error"));
            }

            return false;
        }
    }
}