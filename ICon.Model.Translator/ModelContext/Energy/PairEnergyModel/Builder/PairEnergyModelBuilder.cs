using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IPairEnergyModelBuilder" />
    public class PairEnergyModelBuilder : ModelBuilderBase, IPairEnergyModelBuilder
    {
        /// <inheritdoc />
        public PairEnergyModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <inheritdoc />
        public IList<IPairEnergyModel> BuildModels(IEnumerable<IPairInteraction> pairInteractions)
        {
            var index = 0;
            var pairModels = pairInteractions
                .Select(pair => CreateEnergyModel(pair, ref index))
                .ToList();

            return pairModels;
        }

        /// <summary>
        ///     Creates a pair energy model from a pair interaction
        /// </summary>
        /// <param name="pairInteraction"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected IPairEnergyModel CreateEnergyModel(IPairInteraction pairInteraction, ref int index)
        {
            var energyModel = new PairEnergyModel(pairInteraction)
            {
                ModelId = index++
            };

            var maxParticleIndex = CopyEnergyEntriesToModel(energyModel);
            energyModel.EnergyTable = CreateEnergyTable(energyModel.EnergyEntries, maxParticleIndex);
            return energyModel;
        }

        /// <summary>
        ///     Copies all energy entries of the set interaction into the energy model list an returns the largest found particle
        ///     index
        /// </summary>
        /// <param name="energyModel"></param>
        /// <returns></returns>
        protected int CopyEnergyEntriesToModel(IPairEnergyModel energyModel)
        {
            var lastIndex = 0;
            energyModel.EnergyEntries = new List<PairEnergyEntry>();

            foreach (var entry in energyModel.PairInteraction.GetEnergyEntries())
            {
                var checkIndex = entry.ParticlePair.Particle0.Index;
                lastIndex = lastIndex > checkIndex ? lastIndex : checkIndex;
                energyModel.EnergyEntries.Add(entry);
            }

            return lastIndex;
        }

        /// <summary>
        ///     Builds the energy table based upon the passed energy entry collection
        /// </summary>
        /// <param name="energyEntries"></param>
        /// <param name="largestIndex"></param>
        /// <returns></returns>
        protected double[,] CreateEnergyTable(IList<PairEnergyEntry> energyEntries, int largestIndex)
        {
            var table = new double[largestIndex + 1, largestIndex + 1];

            foreach (var entry in energyEntries)
            {
                var id0 = entry.ParticlePair.Particle0.Index;
                var id1 = entry.ParticlePair.Particle1.Index;
                table[id0, id1] = entry.Energy;

                if (entry.ParticlePair is SymmetricParticlePair)
                    table[id1, id0] = entry.Energy;
            }

            return table;
        }
    }
}