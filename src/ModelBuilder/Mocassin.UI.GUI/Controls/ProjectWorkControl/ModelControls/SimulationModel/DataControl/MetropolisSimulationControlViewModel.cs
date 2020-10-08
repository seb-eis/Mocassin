using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.SimulationModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="MetropolisSimulationControlView" /> that controls
    ///     <see cref="MetropolisSimulationData" /> instances
    /// </summary>
    public class MetropolisSimulationControlViewModel : CollectionControlViewModel<MetropolisSimulationData>,
        IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.SimulationModelData?.MetropolisSimulations);
        }
    }
}