using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Enum to describe the unit cell position status (Stable, unstable,...)
    /// </summary>
    public enum PositionStatus
    {
        Undefined,
        Stable,
        Unstable
    }

    /// <inheritdoc cref="Mocassin.Model.Structures.IUnitCellPosition"/>
    [DataContract]
    public class UnitCellPosition : ModelObject, IUnitCellPosition
    {
        /// <inheritdoc />
        [IgnoreDataMember]
        Fractional3D IUnitCellPosition.Vector => Vector?.AsFractional() ?? new Fractional3D();

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
        public PositionStatus Status { get; set; }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "Unit Cell Position";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IUnitCellPosition>(obj) is IUnitCellPosition position))
                return null;

            Status = position.Status;
            Vector = new DataVector3D(position.Vector);
            OccupationSet = position.OccupationSet;
            return this;

        }

        /// <inheritdoc />
        public FractionalPosition AsPosition()
        {
            return new FractionalPosition(Vector.AsFractional(), OccupationSet.Index, Status);
        }

        /// <inheritdoc />
        public bool IsValidAndStable()
        {
            return !IsDeprecated && Status == PositionStatus.Stable;
        }

        /// <inheritdoc />
        public bool IsValidAndUnstable()
        {
            return !IsDeprecated && Status == PositionStatus.Unstable;
        }
    }
}