namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Mmc job header interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class MmcJobHeader : InteropObject<CMmcJobHeader>
    {
        /// <inheritdoc />
        public MmcJobHeader()
        {
        }

        /// <inheritdoc />
        public MmcJobHeader(CMmcJobHeader structure)
            : base(structure)
        {
        }
    }
}