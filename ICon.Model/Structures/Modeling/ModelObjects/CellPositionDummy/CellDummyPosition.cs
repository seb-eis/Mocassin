using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="ICellDummyPosition" />
    public class CellDummyPosition : ModelObject, ICellDummyPosition
    {
        /// <summary>
        ///     The fractional position 3D vector data
        /// </summary>
        public Fractional3D Vector { get; set; }

        /// <inheritdoc />
        public override string ObjectName => "Dummy Position";

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<ICellDummyPosition>(obj) is { } dummy)) return null;

            Vector = dummy.Vector;
            return this;
        }
    }
}