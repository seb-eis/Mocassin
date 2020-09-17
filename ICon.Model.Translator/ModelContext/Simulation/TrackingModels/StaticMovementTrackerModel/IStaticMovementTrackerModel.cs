using System;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Static movement tracker model that extends the movement tracker model by position information
    /// </summary>
    public interface IStaticMovementTrackerModel : IMovementTrackerModel, IEquatable<IStaticMovementTrackerModel>, IComparable<IStaticMovementTrackerModel>
    {
        /// <summary>
        ///     The index of the tracked particle
        /// </summary>
        int TrackedParticleIndex { get; }

        /// <summary>
        ///     The index P in the 4D encoded system (0,0,0,P) of the tracked position set
        /// </summary>
        int TrackedPositionIndex { get; set; }
    }
}