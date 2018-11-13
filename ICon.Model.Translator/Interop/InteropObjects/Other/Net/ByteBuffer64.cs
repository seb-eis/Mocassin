namespace Mocassin.Model.Translator
{
    /// <summary>
    ///    Fixed 64 byte buffer interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class ByteBuffer64 : InteropObject<CByteBuffer64>
    {
        /// <inheritdoc />
        public ByteBuffer64()
        {
        }

        /// <inheritdoc />
        public ByteBuffer64(CByteBuffer64 structure)
            : base(structure)
        {
        }
    }
}