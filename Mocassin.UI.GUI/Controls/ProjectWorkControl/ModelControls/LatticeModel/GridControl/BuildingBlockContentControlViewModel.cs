using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.LatticeModel;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="BuildingBlockContentControlView" /> that controls
    ///     the content of a <see cref="BuildingBlockGraph" /> instance
    /// </summary>
    public sealed class BuildingBlockContentControlViewModel : CollectionControlViewModel<CellPositionGraph>,
        IContentSupplier<BuildingBlockGraph>
    {
        /// <summary>
        ///     Get the <see cref="BuildingBlockControlViewModel" /> that supplies the <see cref="BuildingBlockGraph" /> instances
        /// </summary>
        public BuildingBlockControlViewModel BlockControlViewModel { get; }

        /// <inheritdoc />
        public BuildingBlockGraph ContentSource { get; private set; }

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReferenceGraph{T}" /> to <see cref="Particle" /> model objects that are
        ///     selectable in the context of currently selected <see cref="CellPositionGraph" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<Particle>> SelectableOccupationParticles =>
            GetSelectableOccupationParticles(SelectedItem);

        /// <summary>
        ///     Creates new <see cref="BuildingBlockContentControlViewModel" /> that targets the provided
        ///     <see cref="BuildingBlockControlViewModel" />
        /// </summary>
        /// <param name="blockControlViewModel"></param>
        public BuildingBlockContentControlViewModel(BuildingBlockControlViewModel blockControlViewModel)
        {
            BlockControlViewModel = blockControlViewModel ?? throw new ArgumentNullException(nameof(blockControlViewModel));
        }

        /// <inheritdoc />
        public void ChangeContentSource(BuildingBlockGraph contentSource)
        {
            ContentSource = contentSource;
            var positions = BlockControlViewModel.CreateDefaultCellPositionList();

            if (!CheckCurrentPositionListValidity(positions))
                ContentSource.ParticleList = positions.Select(x => x.Occupation).ToObservableCollection();
            else
            {
                for (var i = 0; i < positions.Count; i++)
                    positions[i].Occupation = ContentSource.ParticleList[i];
            }

            SetCollection(positions);
        }

        /// <summary>
        ///     Checks if the current <see cref="BuildingBlockGraph" /> state is valid in the context of the passed new
        ///     <see cref="CellPositionGraph" /> list
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        private bool CheckCurrentPositionListValidity(IList<CellPositionGraph> positions)
        {
            // ToDo: Change this to detect all possible invalidity reasons 
            return positions.Count == ContentSource.ParticleList.Count;
        }

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReferenceGraph{T}" /> to <see cref="Particle" /> model objects that are
        ///     selectable in the context of the passed <see cref="CellPositionGraph" />
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <returns></returns>
        private IEnumerable<ModelObjectReferenceGraph<Particle>> GetSelectableOccupationParticles(CellPositionGraph cellPosition)
        {
            var voidResult = new ModelObjectReferenceGraph<Particle>(ParticleGraph.VoidParticle).AsSingleton();
            if (cellPosition == null)
            {
                return BlockControlViewModel.ContentSource?.ProjectModelGraph?.ParticleModelGraph?.Particles?
                    .Select(x => new ModelObjectReferenceGraph<Particle>(x)).Concat(voidResult);
            }

            var positionGraph = (UnitCellPositionGraph) cellPosition.WyckoffPosition.TargetGraph;

            if (positionGraph.PositionStatus == PositionStatus.Unstable) return voidResult;

            return BlockControlViewModel.ContentSource?.ProjectModelGraph?.ParticleModelGraph?.ParticleSets
                ?.Single(x => x.Key == positionGraph.OccupationKey).Particles;
        }
    }
}