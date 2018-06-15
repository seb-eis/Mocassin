using System;

using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    /// Abstract class that defines a coordinate system in N dimensional space
    /// </summary>
    public abstract class CoordinateSystem
    {
        /// <summary>
        /// Dimensions value of the coordinate system
        /// </summary>
        public abstract Int32 Dimension { get; }

        /// <summary>
        /// String literal name of the coordinate system
        /// </summary>
        public abstract Type CoordinateType { get; }
    }

    public abstract class CoordinateSystem<T1> : CoordinateSystem where T1 : struct, ICoordinates
    {
        /// <summary>
        /// Stores the dimension value that is defined in the coordinate tuple type
        /// </summary>
        private static Int32 DimensionValue = default(T1).Size;

        /// <summary>
        /// The coordinate type value
        /// </summary>
        public static Type CoordinateTypeValue = typeof(T1);

        /// <summary>
        /// Return the dimension specified by the coordinate tuple type
        /// </summary>
        public override Int32 Dimension => DimensionValue;

        /// <summary>
        /// Returns the type of the coordinate tuple the system is valid for
        /// </summary>
        public override Type CoordinateType => CoordinateTypeValue;

        /// <summary>
        /// Get the type of the refernce affine coordinate system this system is based upon
        /// </summary>
        public abstract Type ReferenceCoorSystemType { get; }

        /// <summary>
        /// Threats a coordinate tuple as beeing of the system type and transforms it to the specified reference type
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public abstract T1 TransformToReference(T1 original);

        /// <summary>
        /// Threats a coordinate tuple as beeing of the refernce type and transforms it to the specified system type
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public abstract T1 TransformToSystem(T1 original);
    }
}
