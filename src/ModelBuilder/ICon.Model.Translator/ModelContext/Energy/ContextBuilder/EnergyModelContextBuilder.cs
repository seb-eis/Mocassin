﻿using System.Linq;
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
        public override bool CheckBuildRequirements()
        {
            return ModelProject?.Managers().Any(x => x is IEnergyManager) ?? false;
        }

        /// <inheritdoc />
        public override bool CheckLinkDependentBuildRequirements() => true;

        /// <inheritdoc />
        protected override IEnergyModelContext PopulateContext(IEnergyModelContext modelContext)
        {
            if (!CheckBuildRequirements()) return modelContext;

            var manager = ModelProject.Manager<IEnergyManager>();
            var symmetricPairs = manager.DataAccess.Query(port => port.GetStablePairInteractions());
            var asymmetricPairs = manager.DataAccess.Query(port => port.GetUnstablePairInteractions());
            var groupInteractions = manager.DataAccess.Query(port => port.GetGroupInteractions());
            var allPairs = symmetricPairs.Cast<IPairInteraction>().Concat(asymmetricPairs);

            var defects = manager.DataAccess.Query(x => x.GetStableEnvironmentInfo().GetDefectEnergies().ToList());
            var pairTask = Task.Run(() => PairEnergyModelBuilder.BuildModels(allPairs));
            var groupTask = Task.Run(() => GroupEnergyModelBuilder.BuildModels(groupInteractions));

            modelContext.PairEnergyModels = pairTask.Result;
            modelContext.GroupEnergyModels = groupTask.Result;
            modelContext.DefectEnergies = defects;
            return modelContext;
        }

        /// <inheritdoc />
        protected override IEnergyModelContext GetEmptyDefaultContext() => new EnergyModelContext();

        /// <inheritdoc />
        protected override void SetNullBuildersToDefault()
        {
            GroupEnergyModelBuilder ??= new GroupEnergyModelBuilder(ModelProject);
            PairEnergyModelBuilder ??= new PairEnergyModelBuilder(ModelProject);
        }
    }
}