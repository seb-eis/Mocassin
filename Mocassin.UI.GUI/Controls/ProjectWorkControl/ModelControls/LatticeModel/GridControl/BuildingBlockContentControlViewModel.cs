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
    ///     the content of a <see cref="BuildingBlockData" /> instance
    /// </summary>
    public sealed class BuildingBlockContentControlViewModel : CollectionControlViewModel<CellPositionData>,
        IContentSupplier<BuildingBlockData>
    {
        /// <summary>
        ///     Get the <see cref="BuildingBlockControlViewModel" /> that supplies the <see cref="BuildingBlockData" /> instances
        /// </summary>
        public BuildingBlockControlViewModel BlockControlViewModel { get; }

        /// <inheritdoc />
        public BuildingBlockData ContentSource { get; private set; }

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReference{T}" /> to <see cref="Particle" /> model objects that are
        ///     selectable in the context of currently selected <see cref="CellPositionData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<Particle>> SelectableOccupationParticles =>
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
        public void ChangeContentSource(BuildingBlockData contentSource)
        {
            ContentSource = contentSource;
            var positions = BlockControlViewModel.CreateDefaultCellPositionList();

            if (!CheckCurrentPositionListValidity(positions))
                ContentSource.ParticleList = positions.Select(x => x.Particle).ToObservableCollection();
            else
            {
                for (var i = 0; i < positions.Count; i++)
                    positions[i].Particle = ContentSource.ParticleList[i];
            }

            SetCollection(positions);
        }

        /// <summary>
        ///     Checks if the current <see cref="BuildingBlockData" /> state is valid in the context of the passed new
        ///     <see cref="CellPositionData" /> list
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        // ToDo: Change this to detect all possible invalidity reasons 
        private bool CheckCurrentPositionListValidity(IList<CellPositionData> positions) => positions.Count == ContentSource.ParticleList.Count;

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReference{T}" /> to <see cref="Particle" /> model objects that are
        ///     selectable in the context of the passed <see cref="CellPositionData" />
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <returns></returns>
        private IEnumerable<ModelObjectReference<Particle>> GetSelectableOccupationParticles(CellPositionData cellPosition)
        {
            var voidResult = new ModelObjectReference<Particle>(ParticleData.VoidParticle).AsSingleton();
            if (cellPosition == null)
            {
                return BlockControlViewModel.ContentSource?.ProjectModelData?.ParticleModelData?.Particles?
                    .Select(x => new ModelObjectReference<Particle>(x)).Concat(voidResult);
            }

            var positionGraph = (CellReferencePositionData) cellPosition.ReferencePosition.Target;

            if (positionGraph.Stability == PositionStability.Unstable) return voidResult;

            return BlockControlViewModel.ContentSource?.ProjectModelData?.ParticleModelData?.ParticleSets
                                        ?.Single(x => x.Key == positionGraph.Occupation.Key).Particles;
        }
    }
}