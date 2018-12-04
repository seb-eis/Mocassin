using System;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Jump count table entity alias class. Defines a 2D jump count lookup table for the simulation database
    /// </summary>
    public class JumpCountTableEntity : InteropBinaryArray<int>
    {
        /// <inheritdoc />
        public JumpCountTableEntity()
        {
        }

        /// <inheritdoc />
        public JumpCountTableEntity(Array array)
            : base(array)
        {
        }
    }
}