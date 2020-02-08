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
    ///     creation of <see cref="BuildingBlockData" /> instances
    /// </summary>
    public class BuildingBlockControlViewModel : ProjectGraphControlViewModel
    {
        private bool canBeUsed;

        /// <summary>
        ///     Get the <see cref="IModelProject" /> that the view model uses for model dependency checks
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}" /> for the <see cref="BuildingBlockData" /> instances
        /// </summary>
        public CollectionControlViewModel<BuildingBlockData> BuildingBlockCollectionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="IUnitCellProvider{T1}" /> that translates linearized cell position indexing to their
        ///     affiliated <see cref="ICellReferencePosition" /> and vector information
        /// </summary>
        public IUnitCellProvider<ICellReferencePosition> UnitCellProvider { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if the model is in a state where <see cref="BuildingBlockData" /> creation is possible
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
            BuildingBlockCollectionViewModel = new CollectionControlViewModel<BuildingBlockData>();
            ModelProject = projectControl.CreateModelProject();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            BuildingBlockCollectionViewModel.SetCollection(ContentSource?.ProjectModelData?.LatticeModelData?.BuildingBlocks);
            if (contentSource == null) return;

            if (!TryPrepareModelProject())
            {
                PushInfoMessage("Cannot access invalid model");
                CanBeUsed = false;
            }

            CanBeUsed = true;
        }

        /// <summary>
        ///     Tries to prepare the <see cref="IModelProject" /> with the minimal data set required to generate
        ///     <see cref="BuildingBlockData" /> instances
        /// </summary>
        /// <returns></returns>
        private bool TryPrepareModelProject()
        {
            ModelProject.ResetProject();
            var particleData = ContentSource?.ProjectModelData?.ParticleModelData?.GetInputObjects();
            var structureData = ContentSource?.ProjectModelData?.StructureModelData?.GetInputParameters();
            var structureObjects = ContentSource?.ProjectModelData?.StructureModelData?.GetInputObjects();

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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                PushErrorMessage(new InvalidOperationException("Cannot define building block for invalid model", exception));
            }

            return false;
        }

        /// <summary>
        ///     Creates a  default initialized <see cref="List{T}" /> of <see cref="CellPositionData" /> instances that can be
        ///     converted to a <see cref="BuildingBlockData" />
        /// </summary>
        /// <returns></returns>
        public List<CellPositionData> CreateDefaultCellPositionList()
        {
            if (UnitCellProvider == null)
                throw new InvalidOperationException("Cannot create default list for invalid model");

            var result = new List<CellPositionData>(UnitCellProvider.CellSizeInfo.D);

            for (var i = 0; i < UnitCellProvider.CellSizeInfo.D; i++)
            {
                var cellEntry = UnitCellProvider.GetCellEntry(0, 0, 0, i);
                var cellPosition = new CellPositionData
                {
                    Particle = GetParticleReferenceObject(cellEntry.Entry.OccupationSet.GetParticles().First(), cellEntry.Entry),
                    ReferencePosition = GetCellReferencePositionReference(cellEntry.Entry),
                    Vector = VectorData3D.Create(cellEntry.AbsoluteVector),
                    Name = $"Pos.{i}"
                };
                result.Add(cellPosition);
            }

            return result;
        }

        /// <summary>
        ///     Translate the passed <see cref="IParticle" /> interface to the affiliated
        ///     <see cref="ModelObjectReference{T}" /> in the current content source
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private ModelObjectReference<Particle> GetParticleReferenceObject(IParticle particle, ICellReferencePosition position)
        {
            if (!position.IsValidAndStable()) return new ModelObjectReference<Particle>(ParticleData.VoidParticle);
            var objectGraph = ContentSource.ProjectModelData.ParticleModelData.Particles.First(x => x.Key == particle.Key);
            return new ModelObjectReference<Particle> {Target = objectGraph};
        }

        /// <summary>
        ///     Translate the passed <see cref="ICellReferencePosition" /> interface to the affiliated
        ///     <see cref="ModelObjectReference{T}" /> in the current content source
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private ModelObjectReference<CellReferencePosition> GetCellReferencePositionReference(ICellReferencePosition position)
        {
            var objectGraph = ContentSource.ProjectModelData.StructureModelData.CellReferencePositions.First(x => x.Key == position.Key);
            return new ModelObjectReference<CellReferencePosition> {Target = objectGraph};
        }
    }
}