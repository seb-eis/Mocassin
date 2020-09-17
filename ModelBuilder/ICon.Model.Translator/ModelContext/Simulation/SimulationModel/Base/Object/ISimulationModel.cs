using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents an unspecified simulation model that holds the relationship information between the basic model context
    ///     information and a simulation definition
    /// </summary>
    public interface ISimulationModel : IModelComponent
    {
        /// <summary>
        ///     Get or set the maximum attempt frequency for the simulation model
        /// </summary>
        double MaxAttemptFrequency { get; set; }

        /// <summary>
        ///     Get or set the cartesian normalized electric field vector. Describes [1 V/m] field strength
        /// </summary>
        Cartesian3D NormalizedElectricFieldVector { get; set; }

        /// <summary>
        ///     Get the simulation encoding model
        /// </summary>
        ISimulationEncodingModel SimulationEncodingModel { get; set; }

        /// <summary>
        ///     The movement tracking model that describes how particle transition combinations are tracked
        /// </summary>
        ISimulationTrackingModel SimulationTrackingModel { get; set; }

        /// <summary>
        ///     Get the transition models of the simulation model
        /// </summary>
        IEnumerable<ITransitionModel> GetTransitionModels();

        /// <summary>
        ///     Get the local jump models of the simulation model
        /// </summary>
        IEnumerable<ILocalJumpModel> GetLocalJumpModels();

        /// <summary>
        ///     Get the particle set that contains all particles that can potentially move within the simulation
        /// </summary>
        /// <returns></returns>
        IParticleSet GetMobileParticles();
    }
}