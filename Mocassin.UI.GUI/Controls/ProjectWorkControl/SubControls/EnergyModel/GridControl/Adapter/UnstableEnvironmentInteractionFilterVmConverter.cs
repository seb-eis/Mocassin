using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.EnergyModel.GridControl.Adapter
{
    public class UnstableEnvironmentInteractionFilterVmConverter : MarkupExtension, IMultiValueConverter
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) throw new InvalidOperationException("Two conversion objects expected");

            return MakeViewModel(values[0] as MocassinProjectGraph, values[1] as UnstableEnvironmentGraph);
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
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