using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Symmetry.SpaceGroups;
using SharpDX;
using Color = System.Windows.Media.Color;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Provides extension methods for visual information
    /// </summary>
    public static class VisualExtensions
    {
        /// <summary>
        ///     Get the <see cref="Regex" /> to match the contents of the ARGB color format #FFFFFFFF
        /// </summary>
        public static Regex RgbaHexRegex { get; } =
            new Regex(@"#(?<a>[a-fA-F0-9]{2})(?<r>[a-fA-F0-9]{2})(?<g>[a-fA-F0-9]{2})(?<b>[a-fA-F0-9]{2})");

        /// <summary>
        ///     Converts a <see cref="System.Windows.Media.Color" /> to a ARGB #FFFFFFFF format
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToRgbaHex(this Color color) => color.ToString();

        /// <summary>
        ///     Converts a <see cref="Color4" /> to a ARGB #FFFFFFFF format
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToRgbaHex(this Color4 color) => $"#{color.Alpha:X2}{color.Red:X2}{color.Green:X2}{color.Blue:X2}";

        /// <summary>
        ///     Tries to convert a <see cref="string" /> to a <see cref="Color" /> if in the  ARGB #FFFFFFFF format
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool TryParseRgbaHex(string str, out Color color)
        {
            if (str is null)
            {
                color = default;
                return false;
            }

            var match = RgbaHexRegex.Match(str);
            if (!match.Success)
            {
                color = default;
                return false;
            }

            const NumberStyles numberStyle = NumberStyles.HexNumber;

            var a = byte.Parse(match.Groups["a"].Value, numberStyle);
            var r = byte.Parse(match.Groups["r"].Value, numberStyle);
            var g = byte.Parse(match.Groups["g"].Value, numberStyle);
            var b = byte.Parse(match.Groups["b"].Value, numberStyle);
            color = Color.FromArgb(a, r, g, b);
            return true;
        }

        /// <summary>
        ///     Tries to convert a <see cref="string" /> to a <see cref="Color4" /> if in the  ARGB #FFFFFFFF format
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool TryParseRgbaHex(string str, out Color4 color)
        {
            if (str is null)
            {
                color = default;
                return false;
            }

            var match = RgbaHexRegex.Match(str);
            if (!match.Success)
            {
                color = default;
                return false;
            }

            const NumberStyles numberStyle = NumberStyles.HexNumber;

            var a = byte.Parse(match.Groups["a"].Value, numberStyle);
            var r = byte.Parse(match.Groups["r"].Value, numberStyle);
            var g = byte.Parse(match.Groups["g"].Value, numberStyle);
            var b = byte.Parse(match.Groups["b"].Value, numberStyle);
            color = new Color4(r, g, b, a);
            return true;
        }

        /// <summary>
        ///     Converts a <see cref="string" /> to a <see cref="Color" /> if in the ARGB #FFFFFFFF format (With option to suppress
        ///     conversion errors) and uses a default over an exception on parsing error
        /// </summary>
        /// <param name="str"></param>
        /// <param name="noException"></param>
        /// <returns></returns>
        public static Color ParseRgbaHexToColor(string str, bool noException = true)
        {
            if (TryParseRgbaHex(str, out Color color)) return color;
            if (noException) return Color.FromArgb(byte.MaxValue, 0, 0, 0);
            throw new FormatException("Invalid color format.");
        }

        /// <summary>
        ///     Converts a <see cref="string" /> to a <see cref="Color4" /> if in the ARGB #FFFFFFFF format (With option to
        ///     suppress
        ///     conversion errors) and uses a default over an exception on parsing error
        /// </summary>
        /// <param name="str"></param>
        /// <param name="noException"></param>
        /// <returns></returns>
        public static Color4 ParseRgbaHexToColor4(string str, bool noException = true)
        {
            if (TryParseRgbaHex(str, out Color4 color)) return color;
            if (noException) return Color4.White;
            throw new FormatException("Invalid color format.");
        }

        /// <summary>
        ///     Creates a <see cref="Transform3D" /> instance from a <see cref="ISymmetryOperation" /> and
        ///     <see cref="FractionalCoordinateSystem3D" />
        ///     that can directly transform cartesian 3D information in cartesian space
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="coordinateSystem3D"></param>
        /// <returns></returns>
        public static Transform3D ToTransform3D(this ISymmetryOperation operation, FractionalCoordinateSystem3D coordinateSystem3D)
        {
            var toFractionalMatrix = coordinateSystem3D.ToFractionalMatrix.ToTransformMatrix3D();
            var toCartesianMatrix = coordinateSystem3D.ToCartesianMatrix.ToTransformMatrix3D();
            var operationMatrix = operation.ToTransformMatrix3D();
            return new MatrixTransform3D(toFractionalMatrix * operationMatrix * toCartesianMatrix);
        }

        /// <summary>
        ///     Creates a <see cref="Matrix" /> that represents the provided <see cref="ISymmetryOperation" /> as a transform in
        ///     cartesian space
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="fractionalSystem"></param>
        /// <returns></returns>
        public static Matrix ToDxMatrix(this ISymmetryOperation operation, FractionalCoordinateSystem3D fractionalSystem)
        {
            var toFractionalMatrix = fractionalSystem.ToFractionalMatrix.ToDxMatrix();
            var toCartesianMatrix = fractionalSystem.ToCartesianMatrix.ToDxMatrix();
            var operationMatrix = operation.ToDxMatrix();
            return toFractionalMatrix * operationMatrix * toCartesianMatrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix" /> that represents the provided <see cref="ISymmetryOperation" /> as a transform in
        ///     cartesian space specifically for cases where the input coordinates are in the fractional context and not the
        ///     cartesian
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="fractionalSystem"></param>
        /// <returns></returns>
        public static Matrix ToDxMatrixForFractional(this ISymmetryOperation operation, FractionalCoordinateSystem3D fractionalSystem)
        {
            var toCartesianMatrix = fractionalSystem.ToCartesianMatrix.ToDxMatrix();
            var operationMatrix = operation.ToDxMatrix();
            return operationMatrix * toCartesianMatrix;
        }

        /// <summary>
        ///     Creates a <see cref="Transform3D" /> instance from a <see cref="ISymmetryOperation" /> and
        ///     <see cref="FractionalCoordinateSystem3D" />
        ///     with a <see cref="Fractional3D" /> offset that can directly transform cartesian coordinates using the space group
        ///     information
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="fractionalSystem"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Transform3D ToTransform3D(this ISymmetryOperation operation, FractionalCoordinateSystem3D fractionalSystem, in Fractional3D offset)
        {
            var toFractionalMatrix = fractionalSystem.ToFractionalMatrix.ToTransformMatrix3D();
            var toCartesianMatrix = fractionalSystem.ToCartesianMatrix.ToTransformMatrix3D();
            var operationMatrix = operation.ToTransformMatrix3D(offset.A, offset.B, offset.C);
            return new MatrixTransform3D(toFractionalMatrix * operationMatrix * toCartesianMatrix);
        }

        /// <summary>
        ///     Creates a <see cref="Matrix" /> that represents the provided <see cref="ISymmetryOperation" /> with offset
        ///     <see cref="Fractional3D" /> as a transform in cartesian space
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="fractionalSystem"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Matrix ToDxMatrix(this ISymmetryOperation operation, FractionalCoordinateSystem3D fractionalSystem, in Fractional3D offset)
        {
            var toFractionalMatrix = fractionalSystem.ToFractionalMatrix.ToDxMatrix();
            var toCartesianMatrix = fractionalSystem.ToCartesianMatrix.ToDxMatrix();
            var operationMatrix = operation.ToDxMatrix(offset.A, offset.B, offset.C);
            return toFractionalMatrix * operationMatrix * toCartesianMatrix;
        }

        /// <summary>
        ///     Translates a <see cref="ISymmetryOperation" /> (M * v row-layout, 3x4) into the <see cref="Matrix3D" />
        ///     equivalent (v * M row-layout, 4x4)
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static Matrix3D ToTransformMatrix3D(this ISymmetryOperation operation)
        {
            ref var core = ref operation.Core;
            var result = new Matrix3D
            {
                M11 = core.M11, M12 = core.M21, M13 = core.M31, M14 = 0,
                M21 = core.M12, M22 = core.M22, M23 = core.M32, M24 = 0,
                M31 = core.M13, M32 = core.M23, M33 = core.M33, M34 = 0,
                OffsetX = core.M14, OffsetY = core.M24, OffsetZ = core.M34,
                M44 = 1
            };

            return result;
        }

        /// <summary>
        ///     Translates a <see cref="ISymmetryOperation" /> (M * v row-layout, 3x4) into the SharpDX <see cref="Matrix" />
        ///     equivalent (v * M row-layout, 4x4)
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static Matrix ToDxMatrix(this ISymmetryOperation operation)
        {
            ref var core = ref operation.Core;
            var result = new Matrix
            {
                M11 = (float) core.M11, M12 = (float) core.M21, M13 = (float) core.M31, M14 = 0,
                M21 = (float) core.M12, M22 = (float) core.M22, M23 = (float) core.M32, M24 = 0,
                M31 = (float) core.M13, M32 = (float) core.M23, M33 = (float) core.M33, M34 = 0,
                M41 = (float) core.M14, M42 = (float) core.M24, M43 = (float) core.M34, M44 = 1
            };

            return result;
        }

        /// <summary>
        ///     Translates a <see cref="ISymmetryOperation" /> (M * v row-layout, 3x4) into the <see cref="Matrix3D" />
        ///     equivalent (v * M row-layout, 4x4) with an additional offset
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetZ"></param>
        /// <returns></returns>
        public static Matrix3D ToTransformMatrix3D(this ISymmetryOperation operation, double offsetX, double offsetY, double offsetZ)
        {
            ref var core = ref operation.Core;
            var result = new Matrix3D
            {
                M11 = core.M11, M12 = core.M21, M13 = core.M31, M14 = 0,
                M21 = core.M12, M22 = core.M22, M23 = core.M32, M24 = 0,
                M31 = core.M13, M32 = core.M23, M33 = core.M33, M34 = 0,
                OffsetX = core.M14 + offsetX, OffsetY = core.M24 + offsetY, OffsetZ = core.M34 + offsetZ,
                M44 = 1
            };

            return result;
        }

        /// <summary>
        ///     Translates a <see cref="ISymmetryOperation" /> (M * v column-layout, 3x4) into the SharpDX <see cref="Matrix" />
        ///     equivalent (v * M row-layout, 4x4) with an additional offset
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetZ"></param>
        /// <returns></returns>
        public static Matrix ToDxMatrix(this ISymmetryOperation operation, double offsetX, double offsetY, double offsetZ)
        {
            ref var core = ref operation.Core;
            var result = new Matrix
            {
                M11 = (float) core.M11, M12 = (float) core.M21, M13 = (float) core.M31, M14 = 0,
                M21 = (float) core.M12, M22 = (float) core.M22, M23 = (float) core.M32, M24 = 0,
                M31 = (float) core.M13, M32 = (float) core.M23, M33 = (float) core.M33, M34 = 0,
                M41 = (float) (core.M14 + offsetX), M42 = (float) (core.M24 + offsetY), M43 = (float) (core.M34 + offsetZ), M44 = 1
            };

            return result;
        }

        /// <summary>
        ///     Translates a <see cref="TransformMatrix2D" /> (M * v column-layout, 3x3) into the <see cref="Matrix3D" />
        ///     equivalent (v * M row-layout, 4x4)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Matrix3D ToTransformMatrix3D(this TransformMatrix2D source)
        {
            var values = source.Values;
            var result = new Matrix3D
            {
                M11 = values[0, 0], M12 = values[1, 0], M13 = values[2, 0], M14 = 0,
                M21 = values[0, 1], M22 = values[1, 1], M23 = values[2, 1], M24 = 0,
                M31 = values[0, 2], M32 = values[1, 2], M33 = values[2, 2], M34 = 0,
                M44 = 1
            };

            return result;
        }

        /// <summary>
        ///     Translates a <see cref="TransformMatrix2D" /> (M * v row-layout, 3x3) into the <see cref="Matrix" /> SharpDX
        ///     equivalent (v * M row-layout, 4x4)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Matrix ToDxMatrix(this TransformMatrix2D source)
        {
            var values = source.Values;
            var result = new Matrix
            {
                M11 = (float) values[0, 0], M12 = (float) values[1, 0], M13 = (float) values[2, 0], M14 = 0,
                M21 = (float) values[0, 1], M22 = (float) values[1, 1], M23 = (float) values[2, 1], M24 = 0,
                M31 = (float) values[0, 2], M32 = (float) values[1, 2], M33 = (float) values[2, 2], M34 = 0,
                M44 = 1
            };

            return result;
        }

        /// <summary>
        ///     Compares two <see cref="Rect3D" /> using the provided <see cref="IComparer{T}" /> for doubles
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int Compare(this Rect3D lhs, Rect3D rhs, IComparer<double> comparer)
        {
            var compX = comparer.Compare(lhs.X, rhs.X);
            if (compX != 0) return compX;
            var compY = comparer.Compare(lhs.Y, rhs.Y);
            if (compY != 0) return compY;
            var compZ = comparer.Compare(lhs.Z, rhs.Z);
            if (compZ != 0) return compZ;
            var compSizeX = comparer.Compare(lhs.X + lhs.SizeX, rhs.X + rhs.SizeX);
            if (compSizeX != 0) return compSizeX;
            var compSizeY = comparer.Compare(lhs.Y + lhs.SizeY, rhs.Y + rhs.SizeY);
            return compSizeY == 0 ? comparer.Compare(lhs.Z + lhs.SizeZ, rhs.Z + rhs.SizeZ) : compSizeY;
        }

        /// <summary>
        ///     Compares the bounding <see cref="Rect3D" /> of two <see cref="MeshGeometryVisual3D" /> instances
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int CompareBounds(this MeshGeometryVisual3D lhs, MeshGeometryVisual3D rhs, IComparer<double> comparer)
        {
            var lhsBounds = lhs.Transform.TransformBounds(lhs.MeshGeometry.Bounds);
            var rhsBounds = rhs.Transform.TransformBounds(rhs.MeshGeometry.Bounds);
            var result = lhsBounds.Compare(rhsBounds, comparer);
            return result;
        }
    }
}