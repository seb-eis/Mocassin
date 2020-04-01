using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="ICellSite" />
    public class CellSite : ModelObject, ICellSite
    {
        /// <summary>
        ///     The fractional position vector of the unit cell position
        /// </summary>
        public Fractional3D Vector { get; set; }

        /// <inheritdoc />
        [UseTrackedData]
        public IParticleSet OccupationSet { get; set; }

        /// <inheritdoc />
        public PositionStability Stability { get; set; }

        /// <inheritdoc />
        public override string ObjectName => "Unit Cell Position";

        /// <inheritdoc />
        public FractionalPosition AsPosition()
        {
            return new FractionalPosition(Vector, OccupationSet.Index, Stability);
        }

        /// <inheritdoc />
        public bool IsValidAndStable()
        {
            return !IsDeprecated && Stability == PositionStability.Stable;
        }

        /// <inheritdoc />
        public bool IsValidAndUnstable()
        {
            return !IsDeprecated && Stability == PositionStability.Unstable;
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ICellSite>(obj) is { } position)) return null;

            Stability = position.Stability;
            Vector = position.Vector;
            OccupationSet = position.OccupationSet;
            return this;
        }
    }
}