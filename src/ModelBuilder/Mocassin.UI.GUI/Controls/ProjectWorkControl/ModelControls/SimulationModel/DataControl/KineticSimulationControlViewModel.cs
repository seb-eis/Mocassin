using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="KineticSimulationControlView" /> that controls
    ///     <see cref="KineticSimulationData" /> instances
    /// </summary>
    public class KineticSimulationControlViewModel : CollectionControlViewModel<KineticSimulationData>,
        IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.SimulationModelData?.KineticSimulations);
        }
    }
}