using System;
using System.Globalization;
using Mocassin.UI.GUI.Base.Converter;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl
{
    /// <summary>
    ///     Base class for <see cref="MultiValueConverter" /> implementations that wrap <see cref="ModelDataObject" /> into
    ///     host view models for model object references
    /// </summary>
    /// <typeparam name="THost"></typeparam>
    public abstract class HostGraphGuestSelectionVmConverter<THost> : MultiValueConverter
        where THost : ModelDataObject
    {
        /// <inheritdoc />
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is IContentSupplier<MocassinProject> contentSupplier)) return null;
            if (!(values[1] is THost graph)) return null;

            var viewModel = CreateSelectionViewModel(graph);
            viewModel.ChangeContentSource(contentSupplier.ContentSource);
            return viewModel;
        }

        /// <summary>
        ///     Creates a new view model for the host that implements <see cref="IContentSupplier{T}" /> for
        ///     <see cref="MocassinProject" />
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        protected abstract IContentSupplier<MocassinProject> CreateSelectionViewModel(THost host);
    }
}