using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IValueConverter" /> to wrap <see cref="MmcJobPackageDescriptionGraph" /> instances into
    ///     <see cref="MmcJobDescriptionSetControlViewModel" /> instances
    /// </summary>
    public class MmcJobPackageMmcJobDescriptionSetViewModelConverter : MarkupExtension, IMultiValueConverter
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
            if (!(values[1] is MmcJobPackageDescriptionGraph packageDescription)) return null;
            return new MmcJobDescriptionSetControlViewModel(packageDescription, projectGraph);
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}