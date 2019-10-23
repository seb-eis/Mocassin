using System;
using System.Globalization;
using System.Windows.Data;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IValueConverter" /> implementation to wrap <see cref="PairInteractionGraph" /> instances into
    ///     <see cref="PairEnergySetControlViewModel" /> instances
    /// </summary>
    public class PairEnergySetPairEnergySetViewModelConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PairInteractionGraph pairEnergySet) return new PairEnergySetControlViewModel(pairEnergySet);
            return value;
        }
    }
}