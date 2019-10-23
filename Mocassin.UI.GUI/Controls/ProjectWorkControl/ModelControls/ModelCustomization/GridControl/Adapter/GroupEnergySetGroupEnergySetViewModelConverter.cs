using System;
using System.Globalization;
using System.Windows.Data;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IValueConverter" /> to wrap <see cref="GroupInteractionGraph" /> instances into
    ///     <see cref="GroupEnergySetControlViewModel" /> instances for manipulation
    /// </summary>
    public class GroupEnergySetGroupEnergySetViewModelConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GroupInteractionGraph energySet) return new GroupEnergySetControlViewModel(energySet);
            return value;
        }
    }
}