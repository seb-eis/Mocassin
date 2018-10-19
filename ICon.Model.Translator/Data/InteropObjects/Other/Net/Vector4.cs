namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     4D crystal vector interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class Vector4 : InteropObject<CVector4>
    {
        /// <inheritdoc />
        public Vector4(CVector4 structure)
            : base(structure)
        {
        }

        /// <inheritdoc />
        protected Vector4()
        {
        }
    }
}