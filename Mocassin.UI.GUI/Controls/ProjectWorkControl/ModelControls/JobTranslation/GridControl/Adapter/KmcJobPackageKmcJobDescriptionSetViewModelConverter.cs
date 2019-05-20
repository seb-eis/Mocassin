using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="MultiValueConverter" /> to wrap <see cref="KmcJobPackageDescriptionGraph" /> instances into
    ///     <see cref="KmcJobDescriptionSetControlViewModel" /> instances
    /// </summary>
    public class KmcJobPackageKmcJobDescriptionSetViewModelConverter : MultiValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is MocassinProjectGraph projectGraph)) return null;
            if (!(values[1] is KmcJobPackageDescriptionGraph packageDescription)) return null;
            return new KmcJobDescriptionSetControlViewModel(packageDescription, projectGraph);
        }
    }
}