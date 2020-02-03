using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Represents a unit cell reference position that carries fractional position information and occupation as an index
    /// </summary>
    public interface ICellReferencePosition : IModelObject
    {
        /// <summary>
        ///     The fractional position vector
        /// </summary>
        Fractional3D Vector { get; }

        /// <summary>
        ///     The occupation set of this position
        /// </summary>
        IParticleSet OccupationSet { get; }

        /// <summary>
        ///     The status flag of the unit cell position
        /// </summary>
        PositionStability Stability { get; }

        /// <summary>
        ///     Creates a position struct from the unit cell position that supports fractional mass point and vector generic
        ///     methods
        /// </summary>
        /// <returns></returns>
        FractionalPosition AsPosition();

        /// <summary>
        ///     Checks if the position is stable and not deprecated
        /// </summary>
        /// <returns></returns>
        bool IsValidAndStable();

        /// <summary>
        ///     Checks if the position is unstable and not deprecated
        /// </summary>
        /// <returns></returns>
        bool IsValidAndUnstable();
    }
}