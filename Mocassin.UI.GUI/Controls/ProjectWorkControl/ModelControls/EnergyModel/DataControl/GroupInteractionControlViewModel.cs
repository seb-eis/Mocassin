using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Structures;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.DataControl
{
    /// <summary>
    ///     Th <see cref="CollectionControlViewModel{T}" /> for <see cref="GroupInteractionControlView" /> that controls the
    ///     set of <see cref="GroupInteractionData" /> instances
    /// </summary>
    public class GroupInteractionControlViewModel : CollectionControlViewModel<GroupInteractionData>, IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReference{T}" /> for
        ///     <see cref="CellSite" /> instances that support a interaction group
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<CellSite>> CenterPositionOptions => EnumerateReferencePositionOptions();

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.EnergyModelData?.GroupInteractions);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReference{T}" /> for
        ///     <see cref="CellSite" /> instances that support a interaction group
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<CellSite>> EnumerateReferencePositionOptions()
        {
            return ContentSource?.ProjectModelData?.StructureModelData?.CellReferencePositions
                ?.Select(x => new ModelObjectReference<CellSite>(x));
        }
    }
}