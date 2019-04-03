using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl
{
    /// <summary>
    ///     Base class for <see cref="IMultiValueConverter" /> implementations that wrap <see cref="ModelObjectGraph" /> into
    ///     host view models for model object references
    /// </summary>
    /// <typeparam name="THost"></typeparam>
    public abstract class HostGraphGuestSelectionVmConverter<THost> : MarkupExtension, IMultiValueConverter
        where THost : ModelObjectGraph
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is IContentSupplier<MocassinProjectGraph> contentSupplier)) return null;
            if (!(values[1] is THost graph)) return null;

            var viewModel = CreateSelectionViewModel(graph);
            viewModel.ChangeContentSource(contentSupplier.ContentSource);
            return viewModel;
        }

        /// <inheritdoc />
        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Creates a new view model for the host that implements <see cref="IContentSupplier{T}" /> for
        ///     <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        protected abstract IContentSupplier<MocassinProjectGraph> CreateSelectionViewModel(THost host);
    }
}