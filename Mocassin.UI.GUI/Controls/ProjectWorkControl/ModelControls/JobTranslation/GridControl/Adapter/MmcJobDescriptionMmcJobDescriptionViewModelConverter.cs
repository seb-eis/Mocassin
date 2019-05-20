using System;
using System.Globalization;
using System.Windows.Data;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IValueConverter" /> to convert <see cref="MmcJobDescriptionGraph" /> instances into
    ///     <see cref="MmcJobDescriptionControlViewModel" /> instances
    /// </summary>
    public class MmcJobDescriptionMmcJobDescriptionViewModelConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MmcJobDescriptionGraph jobDescription) return new MmcJobDescriptionControlViewModel(jobDescription);
            return null;
        }
    }
}