namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="Mocassin.Model.Translator.ModelContext.IStaticMovementTrackerModel" />
    public class StaticMovementTrackerModel : MovementTrackerModel, IStaticMovementTrackerModel
    {
        /// <inheritdoc />
        public int TrackedParticleIndex => TrackedParticle?.Index ?? -1;

        /// <inheritdoc />
        public int TrackedPositionIndex { get; set; }

        /// <inheritdoc />
        public bool Equals(IStaticMovementTrackerModel other) => CompareTo(other) == 0;

        /// <inheritdoc />
        public int CompareTo(IStaticMovementTrackerModel other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (other == null)
                return 1;

            var positionCompare = TrackedPositionIndex.CompareTo(other.TrackedPositionIndex);
            return positionCompare != 0
                ? positionCompare
                : TrackedParticleIndex.CompareTo(other.TrackedParticleIndex);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = -561440981;
            hashCode = hashCode * -1521134295 + TrackedParticleIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + TrackedPositionIndex.GetHashCode();
            return hashCode;
        }
    }
}