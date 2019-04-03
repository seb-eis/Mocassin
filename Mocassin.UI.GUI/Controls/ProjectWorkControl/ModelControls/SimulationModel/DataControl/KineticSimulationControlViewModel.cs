using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="KineticSimulationControlView" /> that controls
    ///     <see cref="KineticSimulationGraph" /> instances
    /// </summary>
    public class KineticSimulationControlViewModel : CollectionControlViewModel<KineticSimulationGraph>,
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
            SetCollection(ContentSource?.ProjectModelGraph?.SimulationModelGraph?.KineticSimulations);
        }
    }
}