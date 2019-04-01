using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.SimulationModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="MetropolisSimulationControlView" /> that controls
    ///     <see cref="MetropolisSimulationGraph" /> instances
    /// </summary>
    public class MetropolisSimulationControlViewModel : CollectionControlViewModel<MetropolisSimulationGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelGraph?.SimulationModelGraph?.MetropolisSimulations);
        }
    }
}