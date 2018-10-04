using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IStructureModelContextBuilder#"/>
    public class StructureModelContextBuilder : ModelContextBuilderBase<IStructureModelContext>, IStructureModelContextBuilder
    {
        /// <inheritdoc />
        public IEnvironmentModelBuilder EnvironmentModelBuilder { get; set; }

        /// <inheritdoc />
        public IPositionModelBuilder PositionModelBuilder { get; set; }

        /// <inheritdoc />
        public StructureModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder)
            : base(projectModelContextBuilder)
        {
            EnvironmentModelBuilder = new EnvironmentModelBuilder(ProjectServices);
            PositionModelBuilder = new PositionModelBuilder(ProjectServices);
        }

        /// <inheritdoc />
        protected override IStructureModelContext PopulateContext(IStructureModelContext modelContext)
        {
            var manager = ProjectServices.GetManager<IStructureManager>();
            var unitCellPositions = manager.QueryPort.Query(port => port.GetUnitCellPositions());
            var environmentModels = EnvironmentModelBuilder.BuildModels(unitCellPositions);
            var positionModels = PositionModelBuilder.BuildModels(environmentModels);

            modelContext.EnvironmentModels = environmentModels;
            modelContext.PositionModels = positionModels;
            return modelContext;
        }

        /// <inheritdoc />
        protected override IStructureModelContext GetEmptyDefaultContext()
        {
            return new StructureModelContext();
        }
    }
}
