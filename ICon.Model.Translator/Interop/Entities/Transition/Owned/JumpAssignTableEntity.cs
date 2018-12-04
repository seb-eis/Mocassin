using System;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Jump assign table entity alias class. Defines a 3D jump direction id mapping for the simulation database
    /// </summary>
    public class JumpAssignTableEntity : InteropBinaryArray<int>
    {
        /// <inheritdoc />
        public JumpAssignTableEntity()
        {
        }

        /// <inheritdoc />
        public JumpAssignTableEntity(Array array) : base(array)
        {
        }
    }
}