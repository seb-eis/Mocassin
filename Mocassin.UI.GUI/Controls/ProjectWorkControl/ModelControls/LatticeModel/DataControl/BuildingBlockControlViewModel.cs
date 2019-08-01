using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.LatticeModel;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="BuildingBlockControlView" /> that controls the
    ///     creation of <see cref="BuildingBlockGraph" /> instances
    /// </summary>
    public class BuildingBlockControlViewModel : ProjectGraphControlViewModel
    {
        private bool canBeUsed;

        /// <summary>
        ///     Get the <see cref="IModelProject" /> that the view model uses for model dependency checks
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}" /> for the <see cref="BuildingBlockGraph" /> instances
        /// </summary>
        public CollectionControlViewModel<BuildingBlockGraph> BuildingBlockCollectionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="IUnitCellProvider{T1}" /> that translates linearized cell position indexing to their
        ///     affiliated <see cref="IUnitCellPosition" /> and vector information
        /// </summary>
        public IUnitCellProvider<IUnitCellPosition> UnitCellProvider { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if the model is in a state where <see cref="BuildingBlockGraph" /> creation is possible
        /// </summary>
        public bool CanBeUsed
        {
            get => canBeUsed;
            set => SetProperty(ref canBeUsed, value);
        }

        /// <inheritdoc />
        public BuildingBlockControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            BuildingBlockCollectionViewModel = new CollectionControlViewModel<BuildingBlockGraph>();
            ModelProject = projectControl.CreateModelProject();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            BuildingBlockCollectionViewModel.SetCollection(ContentSource?.ProjectModelGraph?.LatticeModelGraph?.BuildingBlocks);
            if (contentSource == null) return;

            if (!TryPrepareModelProject())
            {
                SendCallInfoMessage("Cannot access invalid model");
                CanBeUsed = false;
            }

            CanBeUsed = true;
        }

        /// <summary>
        ///     Tries to prepare the <see cref="IModelProject" /> with the minimal data set required to generate
        ///     <see cref="BuildingBlockGraph" /> instances
        /// </summary>
        /// <returns></returns>
        private bool TryPrepareModelProject()
        {
            ModelProject.ResetProject();
            var particleData = ContentSource?.ProjectModelGraph?.ParticleModelGraph?.GetInputObjects();
            var structureData = ContentSource?.ProjectModelGraph?.StructureModelGraph?.GetInputParameters();
            var structureObjects = ContentSource?.ProjectModelGraph?.StructureModelGraph?.GetInputObjects();

            if (particleData == null || structureData == null || structureObjects == null) return false;

            try
            {
                var sequence = particleData.Cast<object>().Concat(structureData).Concat(structureObjects);
                var reports = ModelProject.InputPipeline.PushToProject(sequence);
                UnitCellProvider = ModelProject
                    .GetManager<IStructureManager>().QueryPort
                    .Query(x => x.GetFullUnitCellProvider());

                if (reports.All(x => x.IsGood)) return true;
            }
            catch (Exception)
            {
                SendCallErrorMessage(new InvalidOperationException("Cannot define building block for invalid model"));
            }

            return false;
        }

        /// <summary>
        ///     Creates a  default initialized <see cref="List{T}" /> of <see cref="CellPositionGraph" /> instances that can be
        ///     converted to a <see cref="BuildingBlockGraph" />
        /// </summary>
        /// <returns></returns>
        public List<CellPositionGraph> CreateDefaultCellPositionList()
        {
            if (UnitCellProvider == null) 
                throw new InvalidOperationException("Cannot create default list for invalid model");

            var result = new List<CellPositionGraph>(UnitCellProvider.CellSizeInfo.D);

            for (var i = 0; i < UnitCellProvider.CellSizeInfo.D; i++)
            {
                var cellEntry = UnitCellProvider.GetCellEntry(0, 0, 0, i);
                var cellPosition = new CellPositionGraph
                {
                    Occupation = GetParticleReferenceObject(cellEntry.Entry.OccupationSet.GetParticles().First(), cellEntry.Entry),
                    WyckoffPosition = GetUnitCellPositionReferenceObject(cellEntry.Entry),
                    Vector = VectorGraph3D.Create(cellEntry.AbsoluteVector),
                    Name = $"Pos.{i}"
                };
                result.Add(cellPosition);
            }

            return result;
        }

        /// <summary>
        ///     Translate the passed <see cref="IParticle" /> interface to the affiliated
        ///     <see cref="ModelObjectReferenceGraph{T}" /> in the current content source
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private ModelObjectReferenceGraph<Particle> GetParticleReferenceObject(IParticle particle, IUnitCellPosition position)
        {
            if (!position.IsValidAndStable()) return new ModelObjectReferenceGraph<Particle>(ParticleGraph.VoidParticle);
            var objectGraph = ContentSource.ProjectModelGraph.ParticleModelGraph.Particles.First(x => x.Key == particle.Key);
            return new ModelObjectReferenceGraph<Particle> {TargetGraph = objectGraph};
        }

        /// <summary>
        ///     Translate the passed <see cref="IUnitCellPosition" /> interface to the affiliated
        ///     <see cref="ModelObjectReferenceGraph{T}" /> in the current content source
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private ModelObjectReferenceGraph<UnitCellPosition> GetUnitCellPositionReferenceObject(IUnitCellPosition position)
        {
            var objectGraph = ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions.First(x => x.Key == position.Key);
            return new ModelObjectReferenceGraph<UnitCellPosition> {TargetGraph = objectGraph};
        }
    }
}