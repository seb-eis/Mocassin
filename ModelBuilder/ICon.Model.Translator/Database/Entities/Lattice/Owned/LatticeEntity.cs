namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Lattice entity alias class. Stores the 4D simulation lattice as a linear byte array for the simulation database
    /// </summary>
    public class LatticeEntity : InteropArray<byte>
    {
        /// <inheritdoc />
        public LatticeEntity()
        {
        }

        /// <inheritdoc />
        public LatticeEntity(byte[,,,] array)
            : base(array)
        {
        }
    }
}