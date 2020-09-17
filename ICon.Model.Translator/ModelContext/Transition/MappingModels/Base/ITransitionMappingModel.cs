using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a transition mapping model without specializing the type of describe transition
    /// </summary>
    public interface ITransitionMappingModel
    {
        /// <summary>
        ///     Get the jump length of the mapping
        /// </summary>
        int PathLength { get; }

        /// <summary>
        ///     Get the inverse <see cref="ITransitionMappingModel" />
        /// </summary>
        ITransitionMappingModel InverseMappingBase { get; }

        /// <summary>
        ///     Defines the start vector of the mapping in encoded 4D crystal coordinates
        /// </summary>
        Vector4I StartVector4D { get; set; }

        /// <summary>
        ///     Get the movement sequence of the transition mapping model
        /// </summary>
        /// <returns></returns>
        IEnumerable<Fractional3D> GetMovementSequence();

        /// <summary>
        ///     Get the transition sequence of the transition mapping model
        /// </summary>
        /// <returns></returns>
        IEnumerable<Vector4I> GetTransitionSequence();

        /// <summary>
        ///     Get the transition model of the transition mapping model
        /// </summary>
        /// <returns></returns>
        ITransitionModel GetTransitionModel();
    }
}