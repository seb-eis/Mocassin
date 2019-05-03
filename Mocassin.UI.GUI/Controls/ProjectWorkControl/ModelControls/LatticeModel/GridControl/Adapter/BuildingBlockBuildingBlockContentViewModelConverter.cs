using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl;
using Mocassin.UI.Xml.LatticeModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="IMultiValueConverter" /> to wrap <see cref="BuildingBlockGraph" /> and
    ///     <see cref="BuildingBlockControlViewModel" /> instances into <see cref="BuildingBlockContentControlViewModel" />
    ///     instances
    /// </summary>
    public class BuildingBlockBuildingBlockContentViewModelConverter : MarkupExtension, IMultiValueConverter
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is BuildingBlockControlViewModel blockControlViewModel)) return null;
            if (!(values[1] is BuildingBlockGraph buildingBlock)) return null;

            var viewModel = new BuildingBlockContentControlViewModel(blockControlViewModel);
            viewModel.ChangeContentSource(buildingBlock);
            return viewModel;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}