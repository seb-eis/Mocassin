namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     General interface for objects that support logical duplication with some reference preservation
    /// </summary>
    public interface IDuplicable
    {
        /// <summary>
        ///     Duplicates the object
        /// </summary>
        /// <returns></returns>
        object Duplicate();
    }

    /// <summary>
    ///     General interface for objects that support logical duplication with some reference preservation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDuplicable<out T> : IDuplicable
    {
        /// <summary>
        ///     Duplicates the object
        /// </summary>
        new T Duplicate();
    }
}