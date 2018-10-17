using System;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    /// General interface for all 64 bit encoded linear 4D informations
    /// </summary>
    public interface ILinearVector4S
    {
        /// <summary>
        /// The 32 bit integer coordinate tuple
        /// </summary>
        Coordinates<Int16, Int16, Int16, Int16> Coordinates { get; }
    }

    /// <summary>
    /// General interface for all 64 bit encoded linear 4D informations that support copy factories
    /// </summary>
    /// <typeparam name="TVector"></typeparam>
    public interface ILinearVector4S<TVector> : ILinearVector4S where TVector : struct, ILinearVector4S<TVector>
    {
        /// <summary>
        /// Factory function to create new 4D vectors of this type
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        TVector CreateNew(Int16 a, Int16 b, Int16 c, Int16 p);
    }
}
