using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ICon.Framework.Extensions;
using System.Threading.Tasks;
using ICon.Framework.Collections;
using ICon.Model.Energies;
using ICon.Model.Particles;
using ICon.Model.ProjectServices;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IEnergyModelContextBuilder"/>
    public class EnergyModelContextBuilder : ModelContextBuilderBase<IEnergyModelContext>, IEnergyModelContextBuilder
    {
        /// <inheritdoc />
        public IGroupEnergyModelBuilder GroupEnergyModelBuilder { get; set; }

        /// <inheritdoc />
        public IPairEnergyModelBuilder PairEnergyModelBuilder { get; set; }

        /// <inheritdoc />
        public EnergyModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder)
            : base(projectModelContextBuilder)
        {
        }

        /// <inheritdoc />
        protected override IEnergyModelContext PopulateContext(IEnergyModelContext modelContext)
        {
            var manager = ProjectServices.GetManager<IEnergyManager>();
            var symmetricPairs = manager.QueryPort.Query(port => port.GetStablePairInteractions());
            var asymmetricPairs = manager.QueryPort.Query(port => port.GetUnstablePairInteractions());
            var groupInteractions = manager.QueryPort.Query(port => port.GetGroupInteractions());
            var allPairs = symmetricPairs.Cast<IPairInteraction>().Concat(asymmetricPairs);

            var pairTask = Task.Run(() => PairEnergyModelBuilder.BuildModels(allPairs));
            var groupTask = Task.Run(() => GroupEnergyModelBuilder.BuildModels(groupInteractions));

            modelContext.PairEnergyModels = pairTask.Result;
            modelContext.GroupEnergyModels = groupTask.Result;
            return modelContext;
        }

        /// <inheritdoc />
        protected override IEnergyModelContext GetEmptyDefaultContext()
        {
            return new EnergyModelContext();
        }

        /// <inheritdoc />
        protected override void SetNullBuildersToDefault()
        {
            GroupEnergyModelBuilder = GroupEnergyModelBuilder ?? new GroupEnergyModelBuilder(ProjectServices);
            PairEnergyModelBuilder = PairEnergyModelBuilder ?? new PairEnergyModelBuilder(ProjectServices);
        }
    }
}
