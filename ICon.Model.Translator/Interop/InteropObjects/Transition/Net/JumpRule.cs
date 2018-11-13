namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Jump rule interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class JumpRule : InteropObject<CJumpRule>
    {
        /// <inheritdoc />
        public JumpRule()
        {
        }

        /// <inheritdoc />
        public JumpRule(CJumpRule structure)
            : base(structure)
        {
        }
    }
}