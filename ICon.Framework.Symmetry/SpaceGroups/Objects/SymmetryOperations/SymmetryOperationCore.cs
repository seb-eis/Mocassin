using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     The core of a symmetry operation that contains the matrix entries. The layout is row wise and operations are
    ///     performed as matrix * vector
    /// </summary>
    public readonly struct SymmetryOperationCore
    {
        /// <summary>
        ///     Entry at row 1 column 1
        /// </summary>
        public double M11 { get; }

        /// <summary>
        ///     Entry at row 1 column 2
        /// </summary>
        public double M12 { get; }

        /// <summary>
        ///     Entry at row 1 column 3
        /// </summary>
        public double M13 { get; }

        /// <summary>
        ///     Entry at row 1 column 4
        /// </summary>
        public double M14 { get; }

        /// <summary>
        ///     Entry at row 2 column 1
        /// </summary>
        public double M21 { get; }

        /// <summary>
        ///     Entry at row 2 column 2
        /// </summary>
        public double M22 { get; }

        /// <summary>
        ///     Entry at row 2 column 3
        /// </summary>
        public double M23 { get; }

        /// <summary>
        ///     Entry at row 2 column 4
        /// </summary>
        public double M24 { get; }

        /// <summary>
        ///     Entry at row 3 column 1
        /// </summary>
        public double M31 { get; }

        /// <summary>
        ///     Entry at row 3 column 2
        /// </summary>
        public double M32 { get; }

        /// <summary>
        ///     Entry at row 3 column 3
        /// </summary>
        public double M33 { get; }

        /// <summary>
        ///     Entry at row 3 column 4
        /// </summary>
        public double M34 { get; }

        /// <summary>
        ///     Creates a new <see cref="SymmetryOperationCore" /> from 12 basis entries
        /// </summary>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m13"></param>
        /// <param name="m14"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <param name="m23"></param>
        /// <param name="m24"></param>
        /// <param name="m31"></param>
        /// <param name="m32"></param>
        /// <param name="m33"></param>
        /// <param name="m34"></param>
        public SymmetryOperationCore(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31, double m32,
            double m33, double m34)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
        }

        /// <summary>
        ///     Creates a new <see cref="SymmetryOperationCore" /> from an array with the 12 basis entries
        /// </summary>
        /// <param name="values"></param>
        public SymmetryOperationCore(double[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length != 12) throw new ArgumentException("Array has to contain 12 entries.", nameof(values));
            M11 = values[0];
            M12 = values[1];
            M13 = values[2];
            M14 = values[3];
            M21 = values[4];
            M22 = values[5];
            M23 = values[6];
            M24 = values[7];
            M31 = values[8];
            M32 = values[9];
            M33 = values[10];
            M34 = values[11];
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{M11},{M12},{M13},{M14};{M21},{M22},{M23},{M24};{M31},{M32},{M33},{M34}";
        }

        /// <summary>
        ///     Performs a left multiplication with <see cref="Fractional3D" /> vector as M * v. Returns the transformed
        ///     <see cref="Fractional3D" />
        /// </summary>
        /// <param name="core"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Fractional3D operator *(in SymmetryOperationCore core, in Fractional3D vector)
        {
            return core.Transform(vector);
        }

        /// <summary>
        ///     Performs a left transform with <see cref="Fractional3D" /> vector as M * v. Returns the transformed
        ///     <see cref="Fractional3D" />
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D Transform(in Fractional3D vector)
        {
            var a = vector.A * M11 + vector.B * M12 + vector.C * M13 + M14;
            var b = vector.A * M21 + vector.B * M22 + vector.C * M23 + M24;
            var c = vector.A * M31 + vector.B * M32 + vector.C * M33 + M34;
            return new Fractional3D(a, b, c);
        }

        /// <summary>
        ///     Performs a left transform with <see cref="Fractional3D" /> vector as M * v. Returns the transformed
        ///     <see cref="Fractional3D" /> and trims it into the 0,0,0 unit cell with the provided tolerance
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="trimTolerance"></param>
        /// <returns></returns>
        public Fractional3D TrimTransform(in Fractional3D vector, double trimTolerance)
        {
            return Transform(vector).TrimToUnitCell(trimTolerance);
        }

        /// <summary>
        ///     Performs a left transform with <see cref="Fractional3D" /> vector as M * v. Returns the transformed
        ///     <see cref="Fractional3D" />
        /// </summary>
        /// <param name="orgA"></param>
        /// <param name="orgB"></param>
        /// <param name="orgC"></param>
        /// <returns></returns>
        public Fractional3D Transform(double orgA, double orgB, double orgC)
        {
            var a = orgA * M11 + orgB * M12 + orgC * M13 + M14;
            var b = orgA * M21 + orgB * M22 + orgC * M23 + M24;
            var c = orgA * M31 + orgB * M32 + orgC * M33 + M34;
            return new Fractional3D(a, b, c);
        }

        /// <summary>
        ///     Performs a left transform with vector coordinates as M * v. Returns the transformed
        ///     <see cref="Fractional3D" /> and trims it into the 0,0,0 unit cell with the provided tolerance
        /// </summary>
        /// <param name="orgC"></param>
        /// <param name="trimTolerance"></param>
        /// <param name="orgA"></param>
        /// <param name="orgB"></param>
        /// <returns></returns>
        public Fractional3D TrimTransform(double orgA, double orgB, double orgC, double trimTolerance)
        {
            return Transform(orgA, orgB, orgC).TrimToUnitCell(trimTolerance);
        }

        /// <summary>
        ///     Get the <see cref="SymmetryOperationCore" /> operation entries as a 12 entry array
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new[] {M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34};
        }

        /// <summary>
        ///     Adds an a,b,c offset and returns the new <see cref="SymmetryOperationCore" />
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public SymmetryOperationCore Offset(double a, double b, double c)
        {
            return new SymmetryOperationCore(M11, M12, M13, M14 + a, M21, M22, M23, M24 + b, M31, M32, M33, M34 + c);
        }

        /// <summary>
        ///     Adds an <see cref="Fractional3D" /> offset vector and returns the new <see cref="SymmetryOperationCore" />
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public SymmetryOperationCore Offset(in Fractional3D offset)
        {
            return Offset(offset.A, offset.B, offset.C);
        }
    }
}