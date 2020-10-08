using System;
using System.Globalization;
using System.Windows.Data;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Data.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IValueConverter" /> to convert <see cref="MmcJobConfigData" /> instances into
    ///     <see cref="MmcJobDescriptionControlViewModel" /> instances
    /// </summary>
    public class MmcJobDescriptionMmcJobDescriptionViewModelConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MmcJobConfigData jobDescription) return new MmcJobDescriptionControlViewModel(jobDescription);
            return null;
        }
    }
}