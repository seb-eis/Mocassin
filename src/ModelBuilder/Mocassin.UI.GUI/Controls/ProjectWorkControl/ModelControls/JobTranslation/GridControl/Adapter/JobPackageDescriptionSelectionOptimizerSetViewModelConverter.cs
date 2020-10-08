using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Data.Jobs;
using Mocassin.UI.Data.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="MultiValueConverter" /> implementation to wrap <see cref="JobPackageData" /> instances
    ///     into <see cref="SelectionOptimizerSetControlViewModel" /> objects
    /// </summary>
    public class JobPackageDescriptionSelectionOptimizerSetViewModelConverter : MultiValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is MocassinProject projectGraph)) return null;
            if (!(values[1] is JobPackageData packageDescription)) return null;
            return new SelectionOptimizerSetControlViewModel(packageDescription, projectGraph);
        }
    }
}