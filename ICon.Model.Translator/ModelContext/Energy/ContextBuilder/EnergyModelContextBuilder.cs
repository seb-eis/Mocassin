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
    /// <summary>
    /// Builder for the energy model context. Expands the reference energy data to a full data context for simulation generation/evaluation
    /// </summary>
    public class EnergyModelContextBuilder : ModelContextBuilderBase<IEnergyModelContext>
    {
        /// <summary>
        /// The builder instance for group energy models
        /// </summary>
        public IGroupEnergyModelBuilder GroupEnergyModelBuilder { get; set; }

        /// <summary>
        /// The builder instance for pair energy models
        /// </summary>
        public IPairEnergyModelBuilder PairEnergyModelBuilder { get; set; }

        /// <inheritdoc />
        public EnergyModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder)
            : base(projectModelContextBuilder)
        {
            GroupEnergyModelBuilder = new GroupEnergyModelBuilder(ProjectServices);
            PairEnergyModelBuilder = new PairEnergyModelBuilder(ProjectServices);
        }

        /// <inheritdoc />
        protected override void PopulateContext()
        {
            var manager = ProjectServices.GetManager<IEnergyManager>();
            var symmetricPairs = manager.QueryPort.Query(port => port.GetStablePairInteractions());
            var asymmetricPairs = manager.QueryPort.Query(port => port.GetUnstablePairInteractions());
            var groupInteractions = manager.QueryPort.Query(port => port.GetGroupInteractions());
            var allPairs = symmetricPairs.Cast<IPairInteraction>().Concat(asymmetricPairs);

            var pairTask = Task.Run(() => PairEnergyModelBuilder.BuildModels(allPairs));
            var groupTask = Task.Run(() => GroupEnergyModelBuilder.BuildModels(groupInteractions));

            ModelContext.PairEnergyModels = pairTask.Result;
            ModelContext.GroupEnergyModels = groupTask.Result;
        }
    }
}
