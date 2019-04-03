using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Structures;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="InteractionFilterGridControlView" /> that controls
    ///     <see cref="InteractionFilterGraph" /> creation for environments
    /// </summary>
    public class InteractionFilterGridControlViewModel : CollectionControlViewModel<InteractionFilterGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <summary>
        ///     Get a boolean flag if the target environment is stable
        /// </summary>
        public bool IsStableEnvironment { get; }

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of <see cref="UnitCellPositionGraph" /> instances that can be used as
        ///     centers <see cref="InteractionFilterGraph" /> instances
        /// </summary>
        public IEnumerable<UnitCellPositionGraph> CenterWyckoffOptions => IsStableEnvironment ? GetWyckoffOptions() : null;

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of <see cref="UnitCellPositionGraph" /> instances that can be used as
        ///     partners <see cref="InteractionFilterGraph" /> instances
        /// </summary>
        public IEnumerable<UnitCellPositionGraph> PartnerWyckoffOptions => GetWyckoffOptions();

        /// <summary>
        ///     Creates a new <see cref="InteractionFilterGridControlViewModel" /> with the passed stability flag
        /// </summary>
        /// <param name="isStableEnvironment"></param>
        public InteractionFilterGridControlViewModel(bool isStableEnvironment)
        {
            IsStableEnvironment = isStableEnvironment;
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of possible <see cref="UnitCellPositionGraph" /> that can be used a filter
        ///     center or partner
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UnitCellPositionGraph> GetWyckoffOptions()
        {
            return ContentSource?.ProjectModelGraph?.StructureModelGraph?.UnitCellPositions
                ?.Where(x => x.PositionStatus == PositionStatus.Stable);
        }
    }
}