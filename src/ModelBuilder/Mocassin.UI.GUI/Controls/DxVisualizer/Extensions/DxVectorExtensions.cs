﻿using Mocassin.Mathematics.ValueTypes;
using SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Extensions
{
    /// <summary>
    ///     Provides extensions methods to handle interaction between SharpDX single precision vectors and Mocassin.Mathematics
    ///     double precision vectors
    /// </summary>
    public static class DxVectorExtensions
    {
        /// <summary>
        ///     Narrowing conversion of a double precision <see cref="Cartesian3D" /> to a SharpDX single precision
        ///     <see cref="Vector3" />
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Vector3 ToDxVector(this Cartesian3D source) => source.Coordinates.ToDxVector();

        /// <summary>
        ///     Narrowing conversion of a double precision <see cref="Fractional3D" /> to a SharpDX single precision
        ///     <see cref="Vector3" />
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Vector3 ToDxVector(this Fractional3D source) => source.Coordinates.ToDxVector();

        /// <summary>
        ///     Narrowing conversion of a double precision <see cref="Coordinates3D" /> to a SharpDX single precision
        ///     <see cref="Vector3" />
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Vector3 ToDxVector(this Coordinates3D source) => new Vector3((float) source.A, (float) source.B, (float) source.C);

        /// <summary>
        ///     Converts a <see cref="Vector3"/> vector to a <see cref="Fractional3D"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Fractional3D ToFractional3D(this Vector3 source) => new Fractional3D(source.X, source.Y, source.Z);
        
        /// <summary>
        ///     Converts a <see cref="Vector4"/> vector to a <see cref="Fractional3D"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Fractional3D ToFractional3D(this Vector4 source) => new Fractional3D(source.X, source.Y, source.Z);
        
        /// <summary>
        ///     Converts a <see cref="Vector3"/> vector to a <see cref="Fractional3D"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Cartesian3D ToCartesian3D(this Vector3 source) => new Cartesian3D(source.X, source.Y, source.Z);
        
        /// <summary>
        ///     Converts a <see cref="Vector4"/> vector to a <see cref="Fractional3D"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Cartesian3D ToCartesian3D(this Vector4 source) => new Cartesian3D(source.X, source.Y, source.Z);
    }
}