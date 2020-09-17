using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;

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