using System;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The energy table entity alias class. Stores 2D energy lookup tables for the simulation database
    /// </summary>
    public class EnergyTableEntity : InteropBinaryArray<double>
    {
        /// <inheritdoc />
        public EnergyTableEntity()
        {
        }

        /// <inheritdoc />
        public EnergyTableEntity(Array array)
            : base(array)
        {
        }

        /// <inheritdoc />
        public override string BlobTypeName => "ENT";
    }
}