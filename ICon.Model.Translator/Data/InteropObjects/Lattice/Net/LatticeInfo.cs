namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Lattice info interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class LatticeInfo : InteropObject<CLatticeInfo>
    {
        /// <inheritdoc />
        public LatticeInfo()
        {
        }

        /// <inheritdoc />
        public LatticeInfo(CLatticeInfo structure)
            : base(structure)
        {
        }
    }
}