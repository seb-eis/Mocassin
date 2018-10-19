using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.IFractionalPosition"/>
    public readonly struct FractionalPosition : IFractionalPosition, IFractionalMassPoint3D<FractionalPosition>
    {
        /// <inheritdoc />
        public Fractional3D Vector { get; }

        /// <inheritdoc />
        public int OccupationIndex { get; }

        /// <inheritdoc />
        public PositionStatus Status { get; }

        /// <inheritdoc />
        public Coordinates<double, double, double> Coordinates => Vector.Coordinates;

        /// <inheritdoc />
        public double A => Vector.A;

        /// <inheritdoc />
        public double B => Vector.B;

        /// <inheritdoc />
        public double C => Vector.C;

        /// <summary>
        ///     Creates new cell position from fractional 3D vector and particle set index
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="occupationIndex"></param>
        /// <param name="status"></param>
        public FractionalPosition(Fractional3D vector, int occupationIndex, PositionStatus status)
            : this()
        {
            Status = status;
            Vector = vector;
            OccupationIndex = occupationIndex;
        }

        /// <inheritdoc />
        public FractionalPosition CreateNew(double a, double b, double c, double mass)
        {
            return new FractionalPosition(new Fractional3D(a, b, c), Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public FractionalPosition CreateNew(in Coordinates<double, double, double> coordinates, double mass)
        {
            return new FractionalPosition(new Fractional3D(coordinates), Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public FractionalPosition CreateNew(in Fractional3D vector, double mass)
        {
            return new FractionalPosition(vector, Convert.ToInt32(mass), Status);
        }

        /// <inheritdoc />
        public FractionalPosition CreateNew(double a, double b, double c)
        {
            return new FractionalPosition(new Fractional3D(a, b, c), OccupationIndex, Status);
        }

        /// <inheritdoc />
        public FractionalPosition CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new FractionalPosition(new Fractional3D(coordinates), OccupationIndex, Status);
        }

        /// <inheritdoc />
        public double GetMass()
        {
            return OccupationIndex;
        }
    }
}