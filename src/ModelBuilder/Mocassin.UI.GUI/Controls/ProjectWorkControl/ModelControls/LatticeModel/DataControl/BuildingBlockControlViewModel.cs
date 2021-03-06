﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.LatticeModel;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ParticleModel;

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
        ///     affiliated <see cref="ICellSite" /> and vector information
        /// </summary>
        public IUnitCellProvider<ICellSite> UnitCellProvider { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if the model is in a state where <see cref="BuildingBlockData" /> creation is possible
        /// </summary>
        public bool CanBeUsed
        {
            get => canBeUsed;
            set => SetProperty(ref canBeUsed, value);
        }

        /// <inheritdoc />
        public BuildingBlockControlViewModel(IProjectAppControl projectControl)
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
                PushInfoMessage("Cannot access invalid model.");
                CanBeUsed = false;
                return;
            }

            CanBeUsed = true;
            EnsureFirstBlockIsDefaultBlock(contentSource);
        }

        /// <summary>
        ///     Temporary workaround that ensures that a default <see cref="BuildingBlockData" /> exists and is the first block
        /// </summary>
        /// <param name="contentSource"></param>
        private void EnsureFirstBlockIsDefaultBlock(MocassinProject contentSource)
        {
            var positionList = CreateDefaultCellPositionList();
            var blocks = ContentSource?.ProjectModelData?.LatticeModelData?.BuildingBlocks ?? throw new InvalidOperationException();
            if (blocks.Count == 0)
            {
                var defaultBlock = new BuildingBlockData {Name = "Default"};
                blocks.Add(defaultBlock);
            }

            blocks.First().ParticleList.Clear();
            blocks.First().ParticleList.AddRange(positionList.Select(x => x.Particle));
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
                                   .Manager<IStructureManager>().DataAccess
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

            var result = new List<CellPositionData>(UnitCellProvider.CellSize.D);

            for (var i = 0; i < UnitCellProvider.CellSize.D; i++)
            {
                var cellEntry = UnitCellProvider.GetCellEntry(0, 0, 0, i);
                var cellPosition = new CellPositionData
                {
                    Particle = GetParticleReferenceObject(cellEntry.Content.OccupationSet.GetParticles().First(), cellEntry.Content),
                    ReferencePosition = GetCellReferencePositionReference(cellEntry.Content),
                    Vector = VectorData3D.Create(cellEntry.Fractional),
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
        private ModelObjectReference<Particle> GetParticleReferenceObject(IParticle particle, ICellSite position)
        {
            if (!position.IsValidAndStable()) return new ModelObjectReference<Particle>(ParticleData.VoidParticle);
            var objectGraph = ContentSource.ProjectModelData.ParticleModelData.Particles.First(x => x.Key == particle.Key);
            return new ModelObjectReference<Particle> {Target = objectGraph};
        }

        /// <summary>
        ///     Translate the passed <see cref="ICellSite" /> interface to the affiliated
        ///     <see cref="ModelObjectReference{T}" /> in the current content source
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private ModelObjectReference<CellSite> GetCellReferencePositionReference(ICellSite position)
        {
            var objectGraph = ContentSource.ProjectModelData.StructureModelData.CellReferencePositions.First(x => x.Key == position.Key);
            return new ModelObjectReference<CellSite> {Target = objectGraph};
        }
    }
}