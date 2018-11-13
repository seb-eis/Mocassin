namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Interaction range interop object. Boxes a marshal struct into a .NET object
    /// </summary>
    public class InteractionRange : InteropObject<CInteractionRange>
    {
        /// <inheritdoc />
        public InteractionRange()
        {
        }

        /// <inheritdoc />
        public InteractionRange(CInteractionRange structure)
            : base(structure)
        {
        }
    }
}