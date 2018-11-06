namespace Mocassin.Framework.Constraints
{
    /// <summary>
    ///     General generic interface for all object constraints of an unrestricted source type to a restricted target type
    ///     (target can be identical to source if no value object is used)
    /// </summary>
    public interface IObjectConstraint<in TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        /// <summary>
        ///     Parses source to target if the internal constraint is not violated (Returns false on violation)
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        bool TryParse(TSource sourceValue, out TTarget targetValue);

        /// <summary>
        ///     Validates if the given source object violates the internal constraints
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        bool IsValid(TSource sourceValue);
    }
}