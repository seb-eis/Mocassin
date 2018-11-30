namespace Mocassin.Model.Mml.Descriptions
{
    /// <summary>
    ///     Description source that converts objects into string descriptions
    /// </summary>
    public interface IDescriptionSource
    {
        /// <summary>
        ///     Takes any object instance and creates a human readable descriptive string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string CreateDescription(object obj);

        /// <summary>
        ///     Tries to create a human readable string description for the passed instance. Returns false if the object type is
        ///     not supported
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        bool TryCreateDescription(object obj, out string description);

        /// <summary>
        ///     Checks if the passed object instance conversion into a description is supported
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool IsSupported(object obj);
    }
}