using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Abstract base class for implementations of simulation models
    /// </summary>
    public abstract class SimulationModel : ModelComponentBase, ISimulationModel
    {
        /// <inheritdoc />
        public abstract double MaxAttemptFrequency { get; set; }

        /// <inheritdoc />
        public ISimulationEncodingModel SimulationEncodingModel { get; set; }

        /// <inheritdoc />
        public ISimulationTrackingModel SimulationTrackingModel { get; set; }

        /// <inheritdoc />
        public abstract IEnumerable<ITransitionModel> GetTransitionModels();

        /// <inheritdoc />
        public abstract IEnumerable<ILocalJumpModel> GetLocalJumpModels();

        /// <inheritdoc />
        public IParticleSet GetMobileParticles()
        {
            return ParticleSet.ToSortedSet(GetTransitionModels().SelectMany(x => x.GetMobileParticles()));
        }
    }
}