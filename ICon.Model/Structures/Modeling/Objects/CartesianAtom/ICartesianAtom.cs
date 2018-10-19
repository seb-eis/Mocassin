using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Represents an actual atom position with cartesian coordinates and a particle index to specify the occupation
    /// </summary>
    public interface ICartesianAtom : ICartesian3D
    {
        /// <summary>
        ///     The particle index
        /// </summary>
        int ParticleIndex { get; }
    }
}