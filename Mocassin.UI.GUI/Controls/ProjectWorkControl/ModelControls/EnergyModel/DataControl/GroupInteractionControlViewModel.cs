using System.Collections.Generic;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.DataControl
{
    /// <summary>
    ///     Th <see cref="CollectionControlViewModel{T}" /> for <see cref="GroupInteractionControlView" /> that controls the
    ///     set of <see cref="GroupInteractionGraph" /> instances
    /// </summary>
    public class GroupInteractionControlViewModel : CollectionControlViewModel<GroupInteractionGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of possible center atoms for <see cref="GroupInteractionGraph" />
        ///     instances
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UnitCellPositionGraph> PositionOptions => GetPositionOptions();

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelGraph?.EnergyModelGraph?.GroupInteractions);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of possible center atoms for <see cref="GroupInteractionGraph" />
        ///     instances
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UnitCellPositionGraph> GetPositionOptions()
        {
            return ContentSource?.ProjectModelGraph?.StructureModelGraph?.UnitCellPositions;
        }
    }
}