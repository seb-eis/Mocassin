using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl;
using Mocassin.UI.Xml.LatticeModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="MultiValueConverter" /> to wrap <see cref="BuildingBlockGraph" /> and
    ///     <see cref="BuildingBlockControlViewModel" /> instances into <see cref="BuildingBlockContentControlViewModel" />
    ///     instances
    /// </summary>
    public class BuildingBlockBuildingBlockContentViewModelConverter : MultiValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is BuildingBlockControlViewModel blockControlViewModel)) return null;
            if (!(values[1] is BuildingBlockGraph buildingBlock)) return null;

            var viewModel = new BuildingBlockContentControlViewModel(blockControlViewModel);
            viewModel.ChangeContentSource(buildingBlock);
            return viewModel;
        }
    }
}