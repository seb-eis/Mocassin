using System.Collections.Generic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents an unspecified transition model that fully describes a simulation transition
    /// </summary>
    public interface ITransitionModel : IModelComponent
    {
        /// <summary>
        ///     Flag that indicates if this model has an inversion that is not itself
        /// </summary>
        bool HasInversion { get; }

        /// <summary>
        ///     The particle set of all selectable particles on the transition
        /// </summary>
        IParticleSet SelectableParticles { get; set; }

        /// <summary>
        ///     The selectable particle set long bitmask
        /// </summary>
        long SelectableParticleMask { get; set; }

        /// <summary>
        ///     Get all unspecified mapping models of the transition model
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITransitionMappingModel> GetMappingModels();

        /// <summary>
        ///     Get all unspecified rule models of the transition
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITransitionRuleModel> GetRuleModels();
    }
}