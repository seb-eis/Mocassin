using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="MultiValueConverter" /> to wrap <see cref="LatticeConfigData" /> instances into
    ///     <see cref="DopingValueControlViewModel" /> instances
    /// </summary>
    public class LatticeConfigurationDopingValueControlViewModelConverter : MultiValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is MocassinProject projectGraph)) return null;
            if (!(values[1] is LatticeConfigData latticeConfiguration)) return null;

            var viewModel = new DopingValueControlViewModel(latticeConfiguration);
            viewModel.ChangeContentSource(projectGraph);
            return viewModel;
        }
    }
}