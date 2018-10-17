using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Enum to describe the unit cell position status (Stable, unstable,...)
    /// </summary>
    public enum PositionStatus : int
    {
        Undefined, Stable, Unstable
    }

    /// <summary>
    /// Unit cell position that is describes occupation and fractional coordinates of a unit cell entry
    /// </summary>
    [DataContract]
    public class UnitCellPosition : ModelObject, IUnitCellPosition
    {
        /// <summary>
        /// Interface access to the fractional vector as a value type
        /// </summary>
        [IgnoreDataMember]
        Fractional3D IUnitCellPosition.Vector => Vector.AsFractional();

        /// <summary>
        /// The fractional position vector of the unit cell position
        /// </summary>
        [DataMember]
        public DataVector3D Vector { get; set; }

        /// <summary>
        /// The particle set that describes the occupation
        /// </summary>
        [DataMember]
        [IndexResolved]
        public IParticleSet OccupationSet { get; set; }

        /// <summary>
        /// The status flag of the position
        /// </summary>
        [DataMember]
        public PositionStatus Status { get; set; }

        /// <summary>
        /// Get a string that represents the name of the object type
        /// </summary>
        /// <returns></returns>
        public override string GetObjectName()
        {
            return "'Unit Cell Position'";
        }

        /// <summary>
        /// Consumes the passed model interface and returns this object as model object (Retruns null if the consume failed)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastIfNotDeprecated<IUnitCellPosition>(obj) is var position)
            {
                Status = position.Status;
                Vector = new DataVector3D(position.Vector);
                OccupationSet = position.OccupationSet;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Creates a fractional position struct from the unit cell position information
        /// </summary>
        /// <returns></returns>
        public FractionalPosition AsPosition()
        {
            return new FractionalPosition(Vector.AsFractional(), OccupationSet.Index, Status);
        }

        /// <summary>
        /// Checks if the position is stable and not deprecated
        /// </summary>
        /// <returns></returns>
        public bool IsValidAndStable()
        {
            return !IsDeprecated && Status == PositionStatus.Stable;
        }

        /// <summary>
        /// Cehcks if the position is unstable and not deprecated
        /// </summary>
        /// <returns></returns>
        public bool IsValidAndUnstable()
        {
            return !IsDeprecated && Status == PositionStatus.Unstable;
        }
    }
}
