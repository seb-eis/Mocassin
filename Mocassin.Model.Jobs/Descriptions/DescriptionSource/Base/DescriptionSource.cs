namespace Mocassin.Model.Mml.Descriptions
{
    /// <summary>
    ///     Abstract base class for description source implementations that convert object instances into human readable
    ///     strings without overwriting the to string method
    /// </summary>
    public abstract class DescriptionSource : IDescriptionSource
    {
        /// <summary>
        ///     Get or set the default description source that should be used
        /// </summary>
        public static IDescriptionSource Default { get; set; }

        /// <summary>
        /// Static constructor that sets the default description source
        /// </summary>
        static DescriptionSource()
        {
            Default = new EnglishDescriptionSource();
        }

        /// <inheritdoc />
        public abstract string CreateDescription(object obj);

        /// <inheritdoc />
        public virtual bool TryCreateDescription(object obj, out string description)
        {
            description = IsSupported(obj) ? CreateDescription(obj) : null;
            return description != null;
        }

        /// <inheritdoc />
        public abstract bool IsSupported(object obj);
    }
}