using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl.Adapter
{
    /// <summary>
    ///     <see cref="MultiValueConverter" /> that wraps <see cref="UnstableEnvironmentGraph" /> instances into
    ///     <see cref="InteractionFilterGridControlViewModel" /> instances
    /// </summary>
    public class UnstableEnvironmentInteractionFilterVmConverter : MultiValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) throw new InvalidOperationException("Two conversion objects expected");

            return MakeViewModel(values[0] as MocassinProjectGraph, values[1] as UnstableEnvironmentGraph);
        }

        /// <summary>
        ///     Wraps the passed <see cref="MocassinProjectGraph" /> and <see cref="UnstableEnvironmentGraph" /> into a
        ///     <see cref="InteractionFilterGridControlViewModel" /> with set content information
        /// </summary>
        /// <param name="projectGraph"></param>
        /// <param name="environmentGraph"></param>
        /// <returns></returns>
        public object MakeViewModel(MocassinProjectGraph projectGraph, UnstableEnvironmentGraph environmentGraph)
        {
            if (projectGraph == null || environmentGraph == null) return null;
            var viewModel = new InteractionFilterGridControlViewModel(false);
            viewModel.ChangeContentSource(projectGraph);
            viewModel.SetCollection(environmentGraph.InteractionFilters);
            return viewModel;
        }
    }
}