using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a static tracker model that describes a statically tracked movement property of the simulation
    /// </summary>
    public interface IStaticTrackerModel : IModelComponent
    {
        /// <summary>
        ///     The tracker id within the local set of static trackers per unit cell
        /// </summary>
        int TrackerId { get; set; }

        /// <summary>
        ///     The id of the cell position the tracker model belongs to
        /// </summary>
        int CellPositionId { get; set; }

        /// <summary>
        ///     The particle tracked by this static tracker model
        /// </summary>
        IParticle TrackedParticle { get; set; }
    }
}