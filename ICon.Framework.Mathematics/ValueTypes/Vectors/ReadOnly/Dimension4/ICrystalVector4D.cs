using System;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// General interface for all four dimensional 128 bit encoded linear supercell crystal position information
    /// </summary>
    public interface ICrystalVector4D : ILinearVector4D
    {
        /// <summary>
        /// Offset in A direction
        /// </summary>
        Int32 A { get; }

        /// <summary>
        /// Offset in B direction
        /// </summary>
        Int32 B { get; }

        /// <summary>
        /// Offset in C direction
        /// </summary>
        Int32 C { get; }

        /// <summary>
        /// The position ID within the unit cell
        /// </summary>
        Int32 P { get; }
    }

    /// <summary>
    /// Generic interface for all four dimensional 128 bit encoded linear supercell crystal position information of specific type
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface ICrystalVector4D<T1> : ICrystalVector4D, ILinearVector4D<T1> where T1 : struct, ICrystalVector4D<T1>
    {

    }
}
