using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Enum to describe the unit cell position status (Stable, unstable,...)
    /// </summary>
    public enum PositionStability
    {
        Undefined,
        Stable,
        Unstable
    }

    /// <inheritdoc cref="ICellReferencePosition"/>
    [DataContract]
    public class CellReferencePosition : ModelObject, ICellReferencePosition
    {
        /// <inheritdoc />
        [IgnoreDataMember]
        Fractional3D ICellReferencePosition.Vector => Vector?.AsFractional() ?? new Fractional3D();

        /// <summary>
        ///     The fractional position vector of the unit cell position
        /// </summary>
        [DataMember]
        public DataVector3D Vector { get; set; }

        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public IParticleSet OccupationSet { get; set; }

        /// <inheritdoc />
        [DataMember]
        public PositionStability Stability { get; set; }

		/// <inheritdoc />
		public override string ObjectName => "Unit Cell Position";

		/// <inheritdoc />
		public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ICellReferencePosition>(obj) is ICellReferencePosition position))
                return null;

            Stability = position.Stability;
            Vector = new DataVector3D(position.Vector);
            OccupationSet = position.OccupationSet;
            return this;

        }

        /// <inheritdoc />
        public FractionalPosition AsPosition()
        {
            return new FractionalPosition(Vector.AsFractional(), OccupationSet.Index, Stability);
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
    }
}