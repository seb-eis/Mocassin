using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Data.Jobs;
using Mocassin.UI.Data.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="MultiValueConverter" /> to wrap <see cref="MmcJobPackageData" /> instances into
    ///     <see cref="MmcJobDescriptionSetControlViewModel" /> instances
    /// </summary>
    public class MmcJobPackageMmcJobDescriptionSetViewModelConverter : MultiValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is MocassinProject projectGraph)) return null;
            if (!(values[1] is MmcJobPackageData packageDescription)) return null;
            return new MmcJobDescriptionSetControlViewModel(packageDescription, projectGraph);
        }
    }
}