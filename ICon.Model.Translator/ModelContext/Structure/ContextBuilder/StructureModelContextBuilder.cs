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
            var manager = ModelProject.GetManager<IStructureManager>();
            var unitCellPositions = manager.QueryPort.Query(port => port.GetUnitCellPositions());
            var environmentModels = EnvironmentModelBuilder.BuildModels(unitCellPositions);
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
            EnvironmentModelBuilder = EnvironmentModelBuilder ?? new EnvironmentModelBuilder(ModelProject);
            PositionModelBuilder = PositionModelBuilder ?? new PositionModelBuilder(ModelProject);
            InteractionRangeModelBuilder = InteractionRangeModelBuilder ?? new InteractionRangeModelBuilder();
        }
    }
}