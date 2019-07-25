﻿using System.Runtime.InteropServices;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Tools.UAccess.Readers.McsReader.Components
{
    /// <summary>
    ///     Simulation state movement tracker struct that stores a fractional movement information within the 'C' simulation
    ///     state
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24, Pack = 8)]
    public readonly struct McsMovementTracker : IFractional3D
    {
        /// <summary>
        ///     Get the 'A' component of the tracker (Fractional coordinate context)
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double A;

        /// <summary>
        ///     Get the 'B' component of the tracker (Fractional coordinate context)
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double B;

        /// <summary>
        ///     Get the 'C' component of the tracker (Fractional coordinate context)
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double C;

        /// <inheritdoc />
        double IFractional3D.A => A;

        /// <inheritdoc />
        double IFractional3D.B => B;

        /// <inheritdoc />
        double IFractional3D.C => C;

        /// <inheritdoc />
        Coordinates<double, double, double> IVector3D.Coordinates => new Coordinates<double, double, double>(A, B, C);

        /// <summary>
        ///     Get the contents of the <see cref="McsMovementTracker" /> as a <see cref="Fractional3D" /> vector type
        /// </summary>
        /// <returns></returns>
        public Fractional3D AsVector()
        {
            return new Fractional3D(A, B, C);
        }

        public static Fractional3D operator +(in Fractional3D lhs, in McsMovementTracker rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        public static Fractional3D operator +(in McsMovementTracker lhs, in Fractional3D rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        public static Fractional3D operator +(in McsMovementTracker lhs, in McsMovementTracker rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        public static Fractional3D operator +(in McsMovementTracker lhs, IFractional3D rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }


        public static Fractional3D operator -(in Fractional3D lhs, in McsMovementTracker rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        public static Fractional3D operator -(in McsMovementTracker lhs, in Fractional3D rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        public static Fractional3D operator -(in McsMovementTracker lhs, in McsMovementTracker rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        public static Fractional3D operator -(in McsMovementTracker lhs, IFractional3D rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }


        public static Fractional3D operator *(in McsMovementTracker lhs, double rhs)
        {
            return new Fractional3D(lhs.A * rhs, lhs.B * rhs, lhs.C * rhs);
        }
    }
}