using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.Optimization
{
    /// <summary>
    ///     Optimizer that removes particles from the <see cref="EnvironmentDefinitionEntity" /> objects of a
    ///     <see cref="SimulationJobPackageModel" /> to improve simulation performance
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

        /// <summary>
        ///     The list of <see cref="IParticle" /> objects to remove on their affiliated <see cref="ICellReferencePosition" />
        /// </summary>
        public IList<(IParticle, ICellReferencePosition)> RemoveCombinations { get; set; }

        /// <inheritdoc />
        public SimulationExecutionFlags Run(IProjectModelContext modelContext, SimulationJobPackageModel jobPackage)
        {
            ModelContext = modelContext;
            JobPackage = jobPackage;

            var isReducedMask = false;
            foreach (var environmentDefinition in jobPackage.SimulationStructureModel.EnvironmentDefinitions)
            {
                var optimizedSetMask = GetOptimizedSelectionParticles(environmentDefinition).AsLong();
                isReducedMask |= optimizedSetMask == environmentDefinition.SelectionParticleMask;
                environmentDefinition.SelectionParticleMask = optimizedSetMask;
            }

            return isReducedMask ? SimulationExecutionFlags.UseDualDofCorrection : SimulationExecutionFlags.None;
        }

        /// <summary>
        ///     Optimizes the <see cref="IParticleSet" /> that is defined as selectable on each
        ///     <see cref="EnvironmentDefinitionEntity" />
        /// </summary>
        /// <param name="environmentDefinition"></param>
        /// <returns></returns>
        protected virtual IParticleSet GetOptimizedSelectionParticles(EnvironmentDefinitionEntity environmentDefinition)
        {
            var cellReferencePosition = ModelContext.StructureModelContext.PositionModels[environmentDefinition.ObjectId].CellReferencePosition;
            var rawSet = DecodeSelectionMask(environmentDefinition.SelectionParticleMask).ToList();
            rawSet.RemoveAll(x => RemoveCombinations.FirstOrDefault(pair => pair.Item1 == x && pair.Item2 == cellReferencePosition).Item1 != null);
            return ParticleSet.ToSortedSet(rawSet);
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
                if ((1L & mask) != 0) result.Particles.Add(particleManager.QueryPort.Query(port => port.GetParticle(index)));

                mask >>= 1;
                index++;
            }

            return result;
        }
    }
}