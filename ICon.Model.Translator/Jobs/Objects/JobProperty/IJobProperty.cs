namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Job property object that defines a single setting for a settable simulation property
    /// </summary>
    public interface IJobProperty
    {
        /// <summary>
        ///     Get the name of the property that should be set
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Get the string representation of the value
        /// </summary>
        string Value { get; }
    }
}