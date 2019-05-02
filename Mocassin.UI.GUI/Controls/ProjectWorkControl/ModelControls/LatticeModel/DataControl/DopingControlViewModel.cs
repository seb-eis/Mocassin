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
	public sealed class DopingControlViewModel : CollectionControlViewModel<DopingGraph>, IContentSupplier<MocassinProjectGraph>
	{
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; private set; }

        public IEnumerable<ModelObjectReferenceGraph<DopingCombination>> SelectablePrimaryDopingCombinations =>
            GetSelectablePrimaryDopingCombinations(SelectedCollectionItem);

        public IEnumerable<ModelObjectReferenceGraph<DopingCombination>> SelectableCounterDopingCombinations =>
            GetSelectableCounterDopingCombinations(SelectedCollectionItem);

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

        public IEnumerable<ModelObjectReferenceGraph<DopingCombination>> GetSelectablePrimaryDopingCombinations(DopingGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.LatticeModelGraph?.DopingCombination;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<DopingCombination>(x));
        }

        public IEnumerable<ModelObjectReferenceGraph<DopingCombination>> GetSelectableCounterDopingCombinations(DopingGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.LatticeModelGraph?.DopingCombination;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<DopingCombination>(x));
        }

        public IEnumerable<ModelObjectReferenceGraph<BuildingBlock>> GetSelectableBuildingBlocks(DopingGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.LatticeModelGraph?.BuildingBlocks;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<BuildingBlock>(x));
        }
    }
}