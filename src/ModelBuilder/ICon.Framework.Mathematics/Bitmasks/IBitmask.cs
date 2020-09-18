namespace Mocassin.Mathematics.Bitmask
{
    /// <summary>
    ///     Abstract base class for all bitmask of arbitrary size
    /// </summary>
    public interface IBitmask
    {
        /// <summary>
        ///     Index access of the entries
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool this[int index] { get; set; }
    }
}