using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a kinetic mapping model that fully describes a transitions geometric properties on a specific position
    /// </summary>
    public interface IKineticMappingModel : ITransitionMappingModel
    {
        /// <summary>
        ///     The kinetic transition model affiliated with the mapping model
        /// </summary>
        IKineticTransitionModel TransitionModel { get; set; }

        /// <summary>
        ///     Flag that indicates if the inverse mapping is already set
        /// </summary>
        bool InverseIsSet { get; }

        /// <summary>
        ///     The kinetic mapping object the model is based upon
        /// </summary>
        KineticMapping Mapping { get; set; }

        /// <summary>
        ///     The inverse mapping model that describes the neutralizing transition
        /// </summary>
        IKineticMappingModel InverseMapping { get; set; }

        /// <summary>
        ///     The encoded 4D position sequence in absolute coordinates
        /// </summary>
        IList<CrystalVector4D> PositionSequence4D { get; }

        /// <summary>
        ///     The fractional 3D position sequence in absolute coordinates
        /// </summary>
        IList<Fractional3D> PositionSequence3D { get; }

        /// <summary>
        ///     The encoded 4D transition sequence where each vector is relative to the start position
        /// </summary>
        IList<CrystalVector4D> TransitionSequence4D { get; set; }

        /// <summary>
        ///     The fractional 3D transition sequence where each vector is relative to the start position
        /// </summary>
        IList<Fractional3D> TransitionSequence3D { get; set; }

        /// <summary>
        ///     The position movement matrix. Describes how each involved position moves on transition in fractional coordinates
        /// </summary>
        Matrix2D PositionMovementMatrix { get; set; }

        /// <summary>
        ///     Links this model to the passed mapping model if it describes the inverse case. Returns false if no match
        /// </summary>
        /// <param name="inverseModel"></param>
        /// <returns></returns>
        bool LinkIfInverseMatch(IKineticMappingModel inverseModel);

        /// <summary>
        ///     Creates the inverted version of the mapping model
        /// </summary>
        /// <returns></returns>
        IKineticMappingModel CreateInverse();
    }
}