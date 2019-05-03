using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Lattices;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.LatticeModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="DopingControlView" /> that controls the collection
    ///     of <see cref="DopingGraph" /> instances
    /// </summary>
    public sealed class DopingControlViewModel : CollectionControlViewModel<DopingGraph>, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; private set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> for selectable
        ///     primary <see cref="DopingCombination" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<DopingCombination>> SelectablePrimaryDopingCombinations =>
            GetSelectablePrimaryDopingCombinations(SelectedCollectionItem);

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> for selectable
        ///     counter <see cref="DopingCombination" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<DopingCombination>> SelectableCounterDopingCombinations =>
            GetSelectableCounterDopingCombinations(SelectedCollectionItem);

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> for selectable
        ///     <see cref="BuildingBlock" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<BuildingBlock>> SelectableBuildingBlocks =>
            GetSelectableBuildingBlocks(SelectedCollectionItem);

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelGraph?.LatticeModelGraph?.Dopings);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> for selectable
        ///     primary <see cref="DopingCombination" /> in the content of the passed current <see cref="DopingGraph" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<DopingCombination>> GetSelectablePrimaryDopingCombinations(DopingGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.LatticeModelGraph?.DopingCombination;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<DopingCombination>(x));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> for selectable
        ///     counter <see cref="DopingCombination" /> in the content of the passed current <see cref="DopingGraph" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<DopingCombination>> GetSelectableCounterDopingCombinations(DopingGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.LatticeModelGraph?.DopingCombination;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<DopingCombination>(x));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> for selectable
        ///     <see cref="BuildingBlock" /> in the content of the passed current <see cref="DopingGraph" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<BuildingBlock>> GetSelectableBuildingBlocks(DopingGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.LatticeModelGraph?.BuildingBlocks;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<BuildingBlock>(x));
        }
    }
}