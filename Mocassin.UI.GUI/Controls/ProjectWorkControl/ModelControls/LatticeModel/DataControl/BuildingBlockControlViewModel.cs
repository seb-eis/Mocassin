using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.LatticeModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl
{
    public class BuildingBlockControlViewModel : ProjectGraphControlViewModel
    {
        private bool canBeUsed;
        private IModelProject ModelProject { get; }

        public CollectionControlViewModel<BuildingBlockGraph> BuildingBlockCollectionViewModel { get; }

        public IReadOnlyDictionary<int, IUnitCellPosition> PositionDictionary { get; set; }

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
                PositionDictionary = ModelProject.GetManager<IStructureManager>().QueryPort
                    .Query(x => x.GetExtendedIndexToPositionDictionary());
                if (reports.All(x => x.IsGood)) return true;
            }
            catch (Exception)
            {
                SendCallErrorMessage(new InvalidOperationException("Cannot define building block for invalid model"));
            }

            return false;
        }

        private BuildingBlockGraph CreateDefaultBuildingBlock()
        {
            var result = new BuildingBlockGraph();

            foreach (var keyValuePair in PositionDictionary)
            {
                var obj = GetParticleReferenceObject(keyValuePair.Value.OccupationSet.GetParticles().First());
                result.ParticleList.Add(obj);
            }

            return result;
        }

        private ModelObjectReferenceGraph<Particle> GetParticleReferenceObject(IParticle particle)
        {
            var objectGraph = ContentSource.ProjectModelGraph.ParticleModelGraph.ParticleSets.First(x => x.Key == particle.Key);
            return new ModelObjectReferenceGraph<Particle> {TargetGraph = objectGraph};
        }
    }
}