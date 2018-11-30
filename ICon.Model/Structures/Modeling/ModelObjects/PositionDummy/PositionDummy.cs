using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.IPositionDummy"/>
    [DataContract(Name = "DummyPosition")]
    public class PositionDummy : ModelObject, IPositionDummy
    {
        /// <summary>
        ///     The fractional position 3D vector data
        /// </summary>
        [DataMember]
        public DataVector3D Vector { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        Fractional3D IPositionDummy.Vector => Vector.AsFractional();

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "Dummy Position";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IPositionDummy>(obj) is IPositionDummy dummy))
                return null;

            Vector = new DataVector3D(dummy.Vector);
            return this;

        }
    }
}