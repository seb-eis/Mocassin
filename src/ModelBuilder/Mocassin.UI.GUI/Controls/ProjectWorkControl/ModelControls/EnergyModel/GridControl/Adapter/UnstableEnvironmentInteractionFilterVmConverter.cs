using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.Data.EnergyModel;
using Mocassin.UI.Data.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl.Adapter
{
    /// <summary>
    ///     <see cref="MultiValueConverter" /> that wraps <see cref="UnstableEnvironmentData" /> instances into
    ///     <see cref="InteractionFilterGridControlViewModel" /> instances
    /// </summary>
    public class UnstableEnvironmentInteractionFilterVmConverter : MultiValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) throw new InvalidOperationException("Two conversion objects expected");

            return MakeViewModel(values[0] as MocassinProject, values[1] as UnstableEnvironmentData);
        }

        /// <summary>
        ///     Wraps the passed <see cref="MocassinProject" /> and <see cref="UnstableEnvironmentData" /> into a
        ///     <see cref="InteractionFilterGridControlViewModel" /> with set content information
        /// </summary>
        /// <param name="project"></param>
        /// <param name="environmentData"></param>
        /// <returns></returns>
        public object MakeViewModel(MocassinProject project, UnstableEnvironmentData environmentData)
        {
            if (project == null || environmentData == null) return null;
            var viewModel = new InteractionFilterGridControlViewModel(false);
            viewModel.ChangeContentSource(project);
            viewModel.SetCollection(environmentData.InteractionFilters);
            return viewModel;
        }
    }
}