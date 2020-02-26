using System;
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

            var (maxCenterIndex, maxPartnerIndex) = CopyEnergyEntriesToModel(energyModel);
            energyModel.EnergyTable = CreateEnergyTable(energyModel.EnergyEntries, maxCenterIndex, maxPartnerIndex);
            return energyModel;
        }

        /// <summary>
        ///     Copies all energy entries of the set interaction into the energy model list an returns the largest found particle
        ///     indices for center and partner
        /// </summary>
        /// <param name="energyModel"></param>
        /// <returns></returns>
        protected (int MaxCenterIndex, int MaxPartnerIndex) CopyEnergyEntriesToModel(IPairEnergyModel energyModel)
        {
            var (maxCenterIndex, maxPartnerIndex, largestIndex) = (0, 0, 0);
            energyModel.EnergyEntries = new List<PairEnergyEntry>();

            foreach (var entry in energyModel.PairInteraction.GetEnergyEntries())
            {
                maxCenterIndex = Math.Max(maxCenterIndex, entry.ParticleInteractionPair.Particle0.Index);
                maxPartnerIndex = Math.Max(maxPartnerIndex, entry.ParticleInteractionPair.Particle1.Index);
                largestIndex = Math.Max(largestIndex, Math.Max(maxPartnerIndex, maxCenterIndex));
                energyModel.EnergyEntries.Add(entry);
            }

            return energyModel.IsAsymmetric ? (maxCenterIndex, maxPartnerIndex) : (largestIndex, largestIndex);
        }

        /// <summary>
        ///     Builds the energy table based upon the passed energy entry collection and index limits
        /// </summary>
        /// <param name="energyEntries"></param>
        /// <param name="maxCenterIndex"></param>
        /// <param name="maxPartnerIndex"></param>
        /// <returns></returns>
        protected double[,] CreateEnergyTable(IList<PairEnergyEntry> energyEntries, int maxCenterIndex, int maxPartnerIndex)
        {
            var table = new double[maxCenterIndex + 1, maxPartnerIndex + 1];

            foreach (var entry in energyEntries)
            {
                var id0 = entry.ParticleInteractionPair.Particle0.Index;
                var id1 = entry.ParticleInteractionPair.Particle1.Index;
                table[id0, id1] = entry.Energy;

                if (entry.ParticleInteractionPair is SymmetricParticleInteractionPair)
                    table[id1, id0] = entry.Energy;
            }

            return table;
        }
    }
}