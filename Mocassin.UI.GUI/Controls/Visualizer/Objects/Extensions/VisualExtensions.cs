using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Provides extension methods for visual information
    /// </summary>
    public static class VisualExtensions
    {
        /// <summary>
        ///     Get the <see cref="Regex"/> to match the contents of the ARGB color format #FFFFFFFF
        /// </summary>
        public static Regex ColorRegexArgbHex { get; } = 
            new Regex(@"#(?<a>[a-fA-F0-9]{2})(?<r>[a-fA-F0-9]{2})(?<g>[a-fA-F0-9]{2})(?<b>[a-fA-F0-9]{2})");

        /// <summary>
        ///     Converts a <see cref="Color"/> to a ARGB #FFFFFFFF format
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToArgbHex(this Color color)
        {
            return color.ToString();
        }

        /// <summary>
        ///     Tries to convert a <see cref="string"/> to a <see cref="Color"/> if in the  ARGB #FFFFFFFF format
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool TryParseArgbHex(string str, out Color color)
        {
            if (str is null)
            {
                color = default;
                return false;
            }

            var match = ColorRegexArgbHex.Match(str);
            if (!match.Success)
            {
                color = default;
                return false;
            }

            const NumberStyles numberStyle = System.Globalization.NumberStyles.HexNumber;

            var a = byte.Parse(match.Groups["a"].Value, numberStyle);
            var r = byte.Parse(match.Groups["r"].Value, numberStyle);
            var g = byte.Parse(match.Groups["g"].Value, numberStyle);
            var b = byte.Parse(match.Groups["b"].Value, numberStyle);
            color = Color.FromArgb(a, r, g, b);
            return true;
        }

        /// <summary>
        ///     Converts a <see cref="string" /> to a <see cref="Color" /> if in the ARGB #FFFFFFFF format (With option to suppress conversion errors)
        ///     use a default over an exception on parsing error
        /// </summary>
        /// <param name="str"></param>
        /// <param name="noException"></param>
        /// <returns></returns>
        public static Color ParseArgbHex(string str, bool noException = true)
        {
            if (TryParseArgbHex(str, out var color)) return color;
            if (noException) return Color.FromArgb(byte.MaxValue, 0, 0, 0);
            throw new FormatException("Invalid color format.");
        }

        public static IEnumerable<Transform3D> ToTransform3D(this IEnumerable<ISymmetryOperation> operations,
            FractionalCoordinateSystem3D coordinateSystem3D)
        {
            yield break;
        }

        /// <summary>
        ///     Creates a <see cref="Transform3D"/> instance from a <see cref="ISymmetryOperation"/> and <see cref="FractionalCoordinateSystem3D"/>
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
        ///     Creates a <see cref="Transform3D"/> instance from a <see cref="ISymmetryOperation"/> and <see cref="FractionalCoordinateSystem3D"/>
        ///     with a <see cref="Fractional3D"/> offset that can directly transform cartesian coordinates using the space group information
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="coordinateSystem3D"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Transform3D ToTransform3D(this ISymmetryOperation operation, FractionalCoordinateSystem3D coordinateSystem3D, in Fractional3D offset)
        {
            var toFractionalMatrix = coordinateSystem3D.ToFractionalMatrix.ToTransformMatrix3D();
            var toCartesianMatrix = coordinateSystem3D.ToCartesianMatrix.ToTransformMatrix3D();
            var operationMatrix = operation.ToTransformMatrix3D(offset.A, offset.B, offset.C);
            return new MatrixTransform3D(toFractionalMatrix * operationMatrix * toCartesianMatrix);
        }

        /// <summary>
        ///     Translates a <see cref="ISymmetryOperation"/> (M * v column-layout, 3x4) into the <see cref="Matrix3D"/> equivalent (v * M row-layout, 4x4)
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static Matrix3D ToTransformMatrix3D(this ISymmetryOperation operation)
        {
            var values = operation.GetOperationsArray();
            var result = new Matrix3D
            {
                M11 = values[0], M12 = values[4], M13 = values[8], M14 = 0,
                M21 = values[1], M22 = values[5], M23 = values[9], M24 = 0,
                M31 = values[2], M32 = values[6], M33 = values[10], M34 = 0,
                OffsetX = values[3], OffsetY = values[7], OffsetZ = values[11],
                M44 = 1,
            };

            return result;
        }

        /// <summary>
        ///     Translates a <see cref="ISymmetryOperation" /> (M * v column-layout, 3x4) into the <see cref="Matrix3D" />
        ///     equivalent (v * M row-layout, 4x4) with an additional offset
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetZ"></param>
        /// <returns></returns>
        public static Matrix3D ToTransformMatrix3D(this ISymmetryOperation operation, double offsetX, double offsetY, double offsetZ)
        {
            var values = operation.GetOperationsArray();
            var result = new Matrix3D
            {
                M11 = values[0], M12 = values[4], M13 = values[8], M14 = 0,
                M21 = values[1], M22 = values[5], M23 = values[9], M24 = 0,
                M31 = values[2], M32 = values[6], M33 = values[10], M34 = 0,
                OffsetX = values[3] + offsetX, OffsetY = values[7] + offsetY, OffsetZ = values[11] + offsetZ,
                M44 = 1,
            };

            return result;
        }

        /// <summary>
        ///     Translates a <see cref="TransformMatrix2D"/> (M * v column-layout, 3x3) into the <see cref="Matrix3D"/> equivalent (v * M row-layout, 4x4)
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
    }
}