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
    ///     of <see cref="DopingData" /> instances
    /// </summary>
    public sealed class DopingControlViewModel : CollectionControlViewModel<DopingData>, IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; private set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> for selectable
        ///     primary <see cref="DopingCombination" />
        /// </summary>
        public IEnumerable<ModelObjectReference<DopingCombination>> SelectablePrimaryDopingCombinations =>
            GetSelectablePrimaryDopingCombinations(SelectedItem);

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> for selectable
        ///     counter <see cref="DopingCombination" />
        /// </summary>
        public IEnumerable<ModelObjectReference<DopingCombination>> SelectableCounterDopingCombinations =>
            GetSelectableCounterDopingCombinations(SelectedItem);

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> for selectable
        ///     <see cref="BuildingBlock" />
        /// </summary>
        public IEnumerable<ModelObjectReference<BuildingBlock>> SelectableBuildingBlocks =>
            GetSelectableBuildingBlocks(SelectedItem);

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.LatticeModelData?.Dopings);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> for selectable
        ///     primary <see cref="DopingCombination" /> in the content of the passed current <see cref="DopingData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<DopingCombination>> GetSelectablePrimaryDopingCombinations(DopingData current)
        {
            var baseCollection = ContentSource?.ProjectModelData?.LatticeModelData?.DopingCombination;
            return baseCollection?.Select(x => new ModelObjectReference<DopingCombination>(x));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> for selectable
        ///     counter <see cref="DopingCombination" /> in the content of the passed current <see cref="DopingData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<DopingCombination>> GetSelectableCounterDopingCombinations(DopingData current)
        {
            var baseCollection = ContentSource?.ProjectModelData?.LatticeModelData?.DopingCombination;
            return baseCollection?.Select(x => new ModelObjectReference<DopingCombination>(x));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> for selectable
        ///     <see cref="BuildingBlock" /> in the content of the passed current <see cref="DopingData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<BuildingBlock>> GetSelectableBuildingBlocks(DopingData current)
        {
            var baseCollection = ContentSource?.ProjectModelData?.LatticeModelData?.BuildingBlocks;
            return baseCollection?.Select(x => new ModelObjectReference<BuildingBlock>(x));
        }
    }
}