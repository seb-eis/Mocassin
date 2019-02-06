using System.Collections.Generic;
using Mocassin.Model.Particles;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.Optimization
{
    /// <summary>
    ///     Jump selection optimizer that analyzes jump collections and lattice configuration to optimize the selection
    ///     masks
    /// </summary>
    public class JumpSelectionOptimizer : IPostBuildOptimizer
    {
        /// <summary>
        ///     The current project model context that is used for the optimization process
        /// </summary>
        protected IProjectModelContext ModelContext { get; set; }

        /// <summary>
        ///     The current job package that the optimizer is handling
        /// </summary>
        protected SimulationJobPackageModel JobPackage { get; set; }

        /// <inheritdoc />
        public void Run(IProjectModelContext modelContext, SimulationJobPackageModel jobPackage)
        {
            ModelContext = modelContext;
            JobPackage = jobPackage;

            foreach (var environmentDefinition in jobPackage.SimulationStructureModel.EnvironmentDefinitions)
            {
                var optimizedSet = GetOptimizedSelectionParticles(environmentDefinition);
                environmentDefinition.SelectionParticleMask = optimizedSet.AsLong();
            }
        }

        /// <summary>
        ///     Optimizes the selection particle set for the passed environment definition (Currently placeholder system that does
        ///     nothing)
        /// </summary>
        /// <param name="environmentDefinition"></param>
        /// <returns></returns>
        protected virtual IParticleSet GetOptimizedSelectionParticles(EnvironmentDefinitionEntity environmentDefinition)
        {
            // ToDo: Implement an actual working optimization routine based on a jump analysis and averaged lattice conformation
            return DecodeSelectionMask(environmentDefinition.SelectionParticleMask);
        }

        /// <summary>
        ///     Decodes the passed particle selection mask into the affiliated particle set
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        protected IParticleSet DecodeSelectionMask(long mask)
        {
            var particleManager = ModelContext.ModelProject.GetManager<IParticleManager>();
            var result = new ParticleSet {Particles = new List<IParticle>()};

            if (mask == 0)
            {
                result.Particles.Add(particleManager.QueryPort.Query(port => port.GetParticle(0)));
                return result;
            }

            var index = 0;
            while (mask != 0)
            {
                if ((1L & mask) != 0)
                    result.Particles.Add(particleManager.QueryPort.Query(port => port.GetParticle(index)));

                mask >>= 1;
                index++;
            }

            return result;
        }
    }
}