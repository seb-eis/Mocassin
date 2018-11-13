namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Kmc job header interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class KmcJobHeader : InteropObject<CKmcJobHeader>
    {
        /// <inheritdoc />
        public KmcJobHeader()
        {
        }

        /// <inheritdoc />
        public KmcJobHeader(CKmcJobHeader structure)
            : base(structure)
        {
        }
    }
}