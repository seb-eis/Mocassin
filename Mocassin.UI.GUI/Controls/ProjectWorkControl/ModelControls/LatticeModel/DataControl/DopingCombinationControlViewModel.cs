using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.LatticeModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="DopingCombinationControlView" /> that controls the
    ///     collection of <see cref="DopingCombinationGraph" /> instances
    /// </summary>
    public sealed class DopingCombinationControlViewModel : CollectionControlViewModel<DopingCombinationGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; private set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> of selectable
        ///     <see cref="UnitCellPosition" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<UnitCellPosition>> SelectablePositions =>
            GetSelectablePositions(SelectedCollectionItem);

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> of selectable dopant
        ///     <see cref="Particle" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<Particle>> SelectableDopantParticles =>
            GetSelectableDopantParticles(SelectedCollectionItem);

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> of selectable dopable
        ///     <see cref="Particle" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<Particle>> SelectableDopableParticles =>
            GetSelectableDopableParticles(SelectedCollectionItem);

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelGraph?.LatticeModelGraph?.DopingCombination);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> of selectable
        ///     <see cref="UnitCellPosition" /> instances in the context of the passed <see cref="DopingCombinationGraph" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<UnitCellPosition>> GetSelectablePositions(DopingCombinationGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.StructureModelGraph?.UnitCellPositions;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<UnitCellPosition>(x));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> of selectable dopant
        ///     <see cref="Particle" /> instances in the context of the passed <see cref="DopingCombinationGraph" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<Particle>> GetSelectableDopantParticles(DopingCombinationGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.ParticleModelGraph?.Particles;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<Particle>(x));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReferenceGraph{T}" /> of selectable dopable
        ///     <see cref="Particle" /> instances in the context of the passed <see cref="DopingCombinationGraph" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<Particle>> GetSelectableDopableParticles(DopingCombinationGraph current)
        {
            var baseCollection = ContentSource?.ProjectModelGraph?.ParticleModelGraph?.Particles;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<Particle>(x));
        }
    }
}