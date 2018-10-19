namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Pair definition interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class PairDefinition : InteropObject<CPairDefinition>
    {
        /// <inheritdoc />
        public PairDefinition()
        {
        }

        /// <inheritdoc />
        public PairDefinition(CPairDefinition structure)
            : base(structure)
        {
        }
    }
}