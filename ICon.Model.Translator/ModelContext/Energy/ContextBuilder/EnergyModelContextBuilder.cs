using System.Linq;
using System.Threading.Tasks;
using Mocassin.Model.Energies;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IEnergyModelContextBuilder" />
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
            var manager = ModelProject.GetManager<IEnergyManager>();
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
            GroupEnergyModelBuilder = GroupEnergyModelBuilder ?? new GroupEnergyModelBuilder(ModelProject);
            PairEnergyModelBuilder = PairEnergyModelBuilder ?? new PairEnergyModelBuilder(ModelProject);
        }
    }
}