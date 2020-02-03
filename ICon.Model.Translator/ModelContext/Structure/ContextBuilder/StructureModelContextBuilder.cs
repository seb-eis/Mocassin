using System.Linq;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IStructureModelContextBuilder" />
    public class StructureModelContextBuilder : ModelContextBuilderBase<IStructureModelContext>, IStructureModelContextBuilder
    {
        /// <inheritdoc />
        public IEnvironmentModelBuilder EnvironmentModelBuilder { get; set; }

        /// <inheritdoc />
        public IPositionModelBuilder PositionModelBuilder { get; set; }

        /// <inheritdoc />
        public IInteractionRangeModelBuilder InteractionRangeModelBuilder { get; set; }

        /// <inheritdoc />
        public StructureModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder)
            : base(projectModelContextBuilder)
        {
        }

        /// <inheritdoc />
        protected override IStructureModelContext PopulateContext(IStructureModelContext modelContext)
        {
            if (!CheckBuildRequirements()) return modelContext;

            var manager = ModelProject.GetManager<IStructureManager>();
            var cellReferencePositions = manager.QueryPort.Query(port => port.GetCellReferencePositions());
            var environmentModels = EnvironmentModelBuilder.BuildModels(cellReferencePositions);
            var positionModels = PositionModelBuilder.BuildModels(environmentModels);
            var rangeModel = InteractionRangeModelBuilder.BuildModel(ModelProject);

            modelContext.EnvironmentModels = environmentModels;
            modelContext.PositionModels = positionModels;
            modelContext.InteractionRangeModel = rangeModel;
            return modelContext;
        }

        /// <inheritdoc />
        protected override IStructureModelContext GetEmptyDefaultContext()
        {
            return new StructureModelContext();
        }

        /// <inheritdoc />
        protected override void SetNullBuildersToDefault()
        {
            EnvironmentModelBuilder ??= new EnvironmentModelBuilder(ModelProject);
            PositionModelBuilder ??= new PositionModelBuilder(ModelProject);
            InteractionRangeModelBuilder ??= new InteractionRangeModelBuilder();
        }

        /// <inheritdoc />
        public override bool CheckBuildRequirements()
        {
            return ModelProject?.GetAllManagers().Any(x => x is IStructureManager) ?? false;
        }
    }
}