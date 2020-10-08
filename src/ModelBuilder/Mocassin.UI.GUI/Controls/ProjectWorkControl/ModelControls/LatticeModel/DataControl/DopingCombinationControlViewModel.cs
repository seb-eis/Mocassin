using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.LatticeModel;
using Mocassin.UI.Data.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="DopingCombinationControlView" /> that controls the
    ///     collection of <see cref="DopingAbstractData" /> instances
    /// </summary>
    public sealed class DopingCombinationControlViewModel : CollectionControlViewModel<DopingAbstractData>,
        IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; private set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> of selectable
        ///     <see cref="CellSite" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReference<CellSite>> SelectablePositions =>
            GetSelectablePositions(SelectedItem);

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> of selectable dopant
        ///     <see cref="Particle" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReference<Particle>> SelectableDopantParticles =>
            GetSelectableDopantParticles(SelectedItem);

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> of selectable dopable
        ///     <see cref="Particle" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReference<Particle>> SelectableDopableParticles =>
            GetSelectableDopableParticles(SelectedItem);

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.LatticeModelData?.DopingCombination);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> of selectable
        ///     <see cref="CellSite" /> instances in the context of the passed <see cref="DopingAbstractData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<CellSite>> GetSelectablePositions(DopingAbstractData current)
        {
            var baseCollection = ContentSource?.ProjectModelData?.StructureModelData?.CellReferencePositions;
            return baseCollection?.Select(x => new ModelObjectReference<CellSite>(x));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> of selectable dopant
        ///     <see cref="Particle" /> instances in the context of the passed <see cref="DopingAbstractData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<Particle>> GetSelectableDopantParticles(DopingAbstractData current)
        {
            var baseCollection = ContentSource?.ProjectModelData?.ParticleModelData?.Particles;
            return baseCollection?.Select(x => new ModelObjectReference<Particle>(x));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> of selectable dopable
        ///     <see cref="Particle" /> instances in the context of the passed <see cref="DopingAbstractData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<Particle>> GetSelectableDopableParticles(DopingAbstractData current)
        {
            var baseCollection = ContentSource?.ProjectModelData?.ParticleModelData?.Particles;
            return baseCollection?.Select(x => new ModelObjectReference<Particle>(x));
        }
    }
}