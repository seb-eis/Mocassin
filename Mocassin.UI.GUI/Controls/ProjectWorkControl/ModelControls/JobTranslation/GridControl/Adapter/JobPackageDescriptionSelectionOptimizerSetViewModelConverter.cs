using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IMultiValueConverter" /> implementation to wrap <see cref="JobPackageDescriptionGraph" /> instances
    ///     into <see cref="SelectionOptimizerSetControlViewModel" /> objects
    /// </summary>
    public class JobPackageDescriptionSelectionOptimizerSetViewModelConverter : MarkupExtension, IMultiValueConverter
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is MocassinProjectGraph projectGraph)) return null;
            if (!(values[1] is JobPackageDescriptionGraph packageDescription)) return null;
            return new SelectionOptimizerSetControlViewModel(packageDescription, projectGraph);
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}