using System;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     The energy table entity alias class. Stores 2D energy lookup tables for the simulation database
    /// </summary>
    public class EnergyTableEntity : InteropArray<double>
    {
        /// <inheritdoc />
        public EnergyTableEntity(double[,] array)
            : base(array)
        {
        }
    }
}