using System;
using Mocassin.Framework.Exceptions;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Enum that identifies specific variations of crystal systems e.g. hexagonal axes for trigonal type
    /// </summary>
    public enum CrystalVariation : byte
    {
        None = 0,
        UniqueAxisA = 1,
        UniqueAxisB = 2,
        UniqueAxisC = 3,
        HexagonalAxes = 4,
        RhombohedralAxes = 5
    }

    /// <summary>
    ///     Enum to identify the 7 crystal systems by increasing symmetry
    /// </summary>
    public enum CrystalSystemId : byte
    {
        Triclinic = 0,
        Monoclinic = 1,
        Orthorhombic = 2,
        Tetragonal = 3,
        Trigonal = 4,
        Hexagonal = 5,
        Cubic = 6
    }

    /// <summary>
    ///     Abstract crystal system class that defines and handles validations and coordinate system creation of the crystal
    ///     systems
    /// </summary>
    public abstract class CrystalSystem
    {
        /// <summary>
        ///     Identifies the specific variation of the system (Majority of systems does not have a specific variation)
        /// </summary>
        public CrystalVariation Variation { get; internal set; }

        /// <summary>
        ///     The crystal system ID (0 to 6)
        /// </summary>
        public CrystalSystemId SystemId { get; internal set; }

        /// <summary>
        ///     The crystal system literal name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        ///     Flag that indicates if the crystal system is ready for usage
        /// </summary>
        public bool IsReady { get; internal set; }

        /// <summary>
        ///     The constraint for the crystal parameters
        /// </summary>
        public NumericConstraint ParameterConstraint { get; internal set; }

        /// <summary>
        ///     The basic double constraint for vector lengths, vector entries, calculations and other not further specified
        ///     potentially constraint double values in the crystal system
        /// </summary>
        public NumericConstraint BasicConstraint { get; internal set; }

        /// <summary>
        ///     Constraint for the angle alpha
        /// </summary>
        public NumericConstraint AlphaConstraint { get; internal set; }

        /// <summary>
        ///     Constraint for the angle beta
        /// </summary>
        public NumericConstraint BetaConstraint { get; internal set; }

        /// <summary>
        ///     Constraint for the angle gamma
        /// </summary>
        public NumericConstraint GammaConstraint { get; internal set; }

        /// <summary>
        ///     Lattice parameter in A direction (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (double Value, bool Fixed) ParamA { get; internal set; }

        /// <summary>
        ///     Lattice parameter in B direction (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (double Value, bool Fixed) ParamB { get; internal set; }

        /// <summary>
        ///     Lattice parameter in C direction (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (double Value, bool Fixed) ParamC { get; internal set; }

        /// <summary>
        ///     Lattice angle alpha in radian (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (double Value, bool Fixed) Alpha { get; internal set; }

        /// <summary>
        ///     Lattice angle beta in radian (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (double Value, bool Fixed) Beta { get; internal set; }

        /// <summary>
        ///     Lattice angle gamma in radian (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (double Value, bool Fixed) Gamma { get; internal set; }

        /// <summary>
        ///     Sets the parameters if they pass the constraints of the system, else returns false
        /// </summary>
        /// <param name="paramSet"></param>
        /// <returns></returns>
        public bool TrySetParameters(CrystalParameterSet paramSet)
        {
            ApplyParameterDependencies(paramSet);
            if (!ValidateAngleConditions(paramSet.Alpha, paramSet.Beta, paramSet.Gamma))
                return false;

            if (!ValidateParameterConstraintCondition(paramSet.ParamA, paramSet.ParamB, paramSet.ParamC))
                return false;

            SetParameters(paramSet);
            return true;
        }

        /// <summary>
        ///     Sets the parameters and angles from a parameter set without checking the validity
        /// </summary>
        /// <param name="paramSet"></param>
        protected void SetParameters(CrystalParameterSet paramSet)
        {
            ParamA = (paramSet.ParamA, ParamA.Fixed);
            ParamB = (paramSet.ParamB, ParamB.Fixed);
            ParamC = (paramSet.ParamC, ParamC.Fixed);
            Alpha = (paramSet.Alpha, Alpha.Fixed);
            Beta = (paramSet.Beta, Beta.Fixed);
            Gamma = (paramSet.Gamma, Gamma.Fixed);
            IsReady = true;
        }

        /// <summary>
        ///     Creates a fractional coordinate system from the crytsal system parameters
        /// </summary>
        /// <returns></returns>
        public FractionalCoordinateSystem3D CreateCoordinateSystem()
        {
            if (!IsReady)
                throw new InvalidObjectStateException("Cannot calculate a coordinate system, required parameters are not set");

            var (a, b, c) = CalculateBaseVectors();
            if (BasicVectorValidation(a, b, c) == false)
                throw new InvalidObjectStateException("Cannot calculate a coordinate system, the parameters form invalid base vectors");

            return new FractionalCoordinateSystem3D(a, b, c, BasicConstraint.Comparer);
        }

        /// <summary>
        ///     Checks if three radians angles are basically valid crystal system angles without specified cyrstal system type
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public bool ValidateGeneralAngleCondition(double alpha, double beta, double gamma)
        {
            var cosAlpha = Math.Cos(alpha);
            var con1 = Math.Cos(beta + gamma) < cosAlpha && cosAlpha < Math.Cos(beta - gamma);
            var con2 = Math.Cos(beta - gamma) < cosAlpha && cosAlpha < Math.Cos(beta + gamma);
            return con1 || con2;
        }

        /// <summary>
        ///     Validates that the angles fulfill the angle constraints specified by the implementing system
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public bool ValidateAngleConstraintCondition(double alpha, double beta, double gamma)
        {
            return AlphaConstraint.IsValid(alpha) && BetaConstraint.IsValid(beta) && GammaConstraint.IsValid(gamma);
        }

        /// <summary>
        ///     Performs an angle validation that tests the soft angle condition of the crystal system (no hierarchy enforcement)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public abstract bool ValidateSoftAngleCondition(double alpha, double beta, double gamma);

        /// <summary>
        ///     Performs a full angle validation that checks of both system specific and general sufficient angle conditions are
        ///     met
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public bool ValidateAngleConditions(double alpha, double beta, double gamma)
        {
            if (!ValidateGeneralAngleCondition(alpha, beta, gamma))
                return false;

            return ValidateAngleConstraintCondition(alpha, beta, gamma)
                   && ValidateSoftAngleCondition(alpha, beta, gamma);
        }

        /// <summary>
        ///     Validate that the cell parameters fulfill both soft system condition and general parameter constraints
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public bool ValidateParameterConditions(double paramA, double paramB, double paramC)
        {
            return ValidateParameterConstraintCondition(paramA, paramB, paramC)
                   && ValidateSoftParameterCondition(paramA, paramB, paramC);
        }

        /// <summary>
        ///     Validates that the passed parameters fulfill the parameter constraints of the implementing system
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public bool ValidateParameterConstraintCondition(double paramA, double paramB, double paramC)
        {
            return ParameterConstraint.IsValid(paramA) && ParameterConstraint.IsValid(paramB) && ParameterConstraint.IsValid(paramC);
        }

        /// <summary>
        ///     Validates that the passed parameters fulfill the soft parameter condition (No hierarchy enforcement)
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public abstract bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC);

        /// <summary>
        ///     Calculates the base vectors from the crystal system parameters
        /// </summary>
        /// <returns></returns>
        public (Coordinates3D A, Coordinates3D B, Coordinates3D C) CalculateBaseVectors()
        {
            return (GetLatticeVectorA(), GetLatticeVectorB(), GetLatticeVectorC());
        }

        /// <summary>
        ///     Calculates the lattice vector in A direction from the system parameters
        /// </summary>
        /// <returns></returns>
        public Coordinates3D GetLatticeVectorA()
        {
            return new Coordinates3D(ParamA.Value, 0.0, 0.0);
        }

        /// <summary>
        ///     Calculates the lattice vector in B direction from the system parameters
        /// </summary>
        /// <returns></returns>
        public Coordinates3D GetLatticeVectorB()
        {
            var a = (ParamB.Value * Math.Cos(Gamma.Value)).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            var b = (ParamB.Value * Math.Sin(Gamma.Value)).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            return new Coordinates3D(a, b, 0.0);
        }

        /// <summary>
        ///     Calculates the lattice vector in C direction from the system parameters
        /// </summary>
        /// <returns></returns>
        public Coordinates3D GetLatticeVectorC()
        {
            var a = (ParamC.Value * Math.Cos(Beta.Value)).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            var b = (ParamC.Value * Math.Cos(Alpha.Value) - Math.Cos(Gamma.Value) * Math.Cos(Beta.Value) / Math.Sin(Gamma.Value))
                .ZeroSafeRound(0.0, BasicConstraint.Comparer);
            var c = Math.Sqrt(ParamC.Value * ParamC.Value - a * a - b * b).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            return new Coordinates3D(a, b, c);
        }

        /// <summary>
        ///     Validates that the lattice vectors are potentially valid lattice vectors without checking if they match the actual
        ///     crystal system type
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <param name="vectorC"></param>
        /// <returns></returns>
        public bool BasicVectorValidation(Coordinates3D vectorA, Coordinates3D vectorB, Coordinates3D vectorC)
        {
            return vectorA.IsLinearIndependentFrom(vectorB, vectorC, BasicConstraint.Comparer);
        }

        /// <summary>
        ///     Checks if a vector length is valid
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected bool ValidateVectorLength(in Coordinates3D vector)
        {
            return !BasicConstraint.Comparer.Equals(vector.GetLength(), 0.0);
        }

        /// <summary>
        ///     Corrects potential parameter dependencies in a crystal parameter set using the rules of the specific crystal system
        ///     implementation
        /// </summary>
        /// <param name="paramSet"></param>
        public abstract void ApplyParameterDependencies(CrystalParameterSet paramSet);

        /// <summary>
        ///     Returns a default parameter set that fulfills all restrictions of the actual crystal system
        /// </summary>
        /// <returns></returns>
        public abstract CrystalParameterSet GetDefaultParameterSet();
    }
}