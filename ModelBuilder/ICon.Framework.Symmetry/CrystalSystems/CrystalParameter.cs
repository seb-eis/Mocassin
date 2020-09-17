namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Defines a parameter of a crystal containing a value and an information if the value is fixed in its current context
    /// </summary>
    public readonly struct CrystalParameter
    {
        /// <summary>
        ///     The value of the parameter
        /// </summary>
        public double Value { get; }

        /// <summary>
        ///     Defines if the value is immutable in te current context
        /// </summary>
        public bool IsContextImmutable { get; }

        /// <summary>
        ///     Creates new <see cref="CrystalParameter" /> from value and optional constant flag
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isContextImmutable"></param>
        public CrystalParameter(double value, bool isContextImmutable = false)
        {
            Value = value;
            IsContextImmutable = isContextImmutable;
        }

        /// <summary>
        ///     Creates a new <see cref="CrystalParameter" /> with a changed value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CrystalParameter ChangeValue(double value) => new CrystalParameter(value, IsContextImmutable);
    }
}