using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Data.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="ValueConverter" /> to convert <see cref="KmcJobConfigData" /> instances into
    ///     <see cref="KmcJobDescriptionControlViewModel" /> instances
    /// </summary>
    public class KmcJobDescriptionKmcJobDescriptionViewModelConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is KmcJobConfigData jobDescription) return new KmcJobDescriptionControlViewModel(jobDescription);
            return null;
        }
    }
}