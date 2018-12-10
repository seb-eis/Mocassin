namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Jump count table entity alias class. Defines a 2D jump count lookup table for the simulation database
    /// </summary>
    public class JumpCountTableEntity : InteropArray<int>
    {
        /// <inheritdoc />
        public JumpCountTableEntity()
        {
        }

        /// <inheritdoc />
        public JumpCountTableEntity(int[,] array)
            : base(array)
        {
        }
    }
}