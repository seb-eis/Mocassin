using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Structures;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="InteractionFilterGridControlView" /> that controls
    ///     <see cref="RadialInteractionFilterData" /> creation for environments
    /// </summary>
    public class InteractionFilterGridControlViewModel : CollectionControlViewModel<RadialInteractionFilterData>,
        IContentSupplier<MocassinProject>
    {
        /// <summary>
        ///     Get a boolean flag if the target environment is stable
        /// </summary>
        public bool IsStableEnvironment { get; }

        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReference{T}" /> for
        ///     <see cref="CellReferencePosition" /> instances that can be used as center
        ///     <see cref="RadialInteractionFilterData" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReference<CellReferencePosition>> CenterPositionOptions =>
            IsStableEnvironment ? EnumerateReferencePositionOptions() : null;

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReference{T}" /> for
        ///     <see cref="CellReferencePosition" /> instances that can be used as partner
        ///     <see cref="RadialInteractionFilterData" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReference<CellReferencePosition>> PartnerPositionOptions => EnumerateReferencePositionOptions();

        /// <summary>
        ///     Creates a new <see cref="InteractionFilterGridControlViewModel" /> with the passed stability flag
        /// </summary>
        /// <param name="isStableEnvironment"></param>
        public InteractionFilterGridControlViewModel(bool isStableEnvironment)
        {
            IsStableEnvironment = isStableEnvironment;
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReference{T}" /> for
        ///     <see cref="CellReferencePosition" /> instances that can be used a filter center or partner position
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<CellReferencePosition>> EnumerateReferencePositionOptions()
        {
            return ContentSource?.ProjectModelData?.StructureModelData?.CellReferencePositions?
                .Where(x => x.Stability == PositionStability.Stable)
                .Select(x => new ModelObjectReference<CellReferencePosition>(x));
        }
    }
}