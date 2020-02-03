using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="ICellDummyPosition"/>
    [DataContract(Name = "DummyPosition")]
    public class CellDummyPosition : ModelObject, ICellDummyPosition
    {
        /// <summary>
        ///     The fractional position 3D vector data
        /// </summary>
        [DataMember]
        public DataVector3D Vector { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        Fractional3D ICellDummyPosition.Vector => Vector.AsFractional();

		/// <inheritdoc />
		public override string ObjectName => "Dummy Position";

		/// <inheritdoc />
		public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ICellDummyPosition>(obj) is ICellDummyPosition dummy))
                return null;

            Vector = new DataVector3D(dummy.Vector);
            return this;

        }
    }
}