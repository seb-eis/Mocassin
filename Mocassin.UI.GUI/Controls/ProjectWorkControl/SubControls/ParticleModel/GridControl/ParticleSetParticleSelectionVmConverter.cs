using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel.GridControl
{
    /// <summary>
    ///     <see cref="IMultiValueConverter" /> that wraps a <see cref="ParticleSetGraph" /> and
    ///     <see cref="MocassinProjectGraph" /> supplier into a <see cref="ParticleSetParticleSelectionViewModel" />
    /// </summary>
    public class ParticleSetParticleSelectionVmConverter : MarkupExtension, IMultiValueConverter
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
            if (!(values[1] is ParticleSetGraph graph)) return null;

            var viewModel = new ParticleSetParticleSelectionViewModel(graph);
            viewModel.ChangeContentSource(contentSupplier.ContentSource);
            return viewModel;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}