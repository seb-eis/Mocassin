using System;


using ICon.Mathematics.Coordinates;
using ICon.Mathematics.Constraints;
using ICon.Mathematics.Extensions;
using ICon.Framework.Exceptions;

using ACoorTuple = ICon.Mathematics.ValueTypes.Coordinates<System.Double, System.Double, System.Double>;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Enum that identifies specific variations of crystal systems e.g. hexagonal axes for trigonal type
    /// </summary>
    public enum CrystalVariation : Byte
    {
        None = 0,
        UniqueAxisA = 1,
        UniqueAxisB = 2,
        UniqueAxisC =3,
        HexagonalAxes = 4,
        RhombohedralAxes = 5
    } 

    /// <summary>
    /// Enum to identify the 7 cyrstal systems by increasing symmetry
    /// </summary>
    public enum CrystalSystemID : Byte
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
    /// Abstract crystal system class that defines and handles validations and coordinate system creation of the crystal systems
    /// </summary>
    public abstract class CrystalSystem
    {
        /// <summary>
        /// Identifies the specific variation of the system (MAjority of systems does not have a specific variation)
        /// </summary>
        public CrystalVariation Variation { get; internal set; }

        /// <summary>
        /// The crytsal system ID (0 to 6)
        /// </summary>
        public CrystalSystemID SystemID { get; internal set; }

        /// <summary>
        /// The crystal system literal name
        /// </summary>
        public String SystemName { get; set; }

        /// <summary>
        /// Flag that indicates if the crystal system is ready for usage
        /// </summary>
        public Boolean IsReady { get; internal set; }

        /// <summary>
        /// The constraint for the crystal parameters
        /// </summary>
        public NumericConstraint ParameterConstraint { get; internal set; }

        /// <summary>
        /// The basic double constraint for vector lengths, vector entries, calculations and other not further specified potentially constraint double values in the crystal system
        /// </summary>
        public NumericConstraint BasicConstraint { get; internal set; }

        /// <summary>
        /// Constraint for the angle alpha
        /// </summary>
        public NumericConstraint AlphaConstraint { get; internal set; }

        /// <summary>
        /// Constraint for the angle beta
        /// </summary>
        public NumericConstraint BetaConstraint { get; internal set; }

        /// <summary>
        /// Constraint for the angle gamma
        /// </summary>
        public NumericConstraint GammaConstraint { get; internal set; }

        /// <summary>
        /// Lattice parameter in A direction (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (Double Value, Boolean Fixed) ParamA { get; internal set; }

        /// <summary>
        /// Lattice parameter in B direction (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (Double Value, Boolean Fixed) ParamB { get; internal set; }

        /// <summary>
        /// Lattice parameter in C direction (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (Double Value, Boolean Fixed) ParamC { get; internal set; }

        /// <summary>
        /// Lattice angle alpha in radian (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (Double Value, Boolean Fixed) Alpha { get; internal set; }

        /// <summary>
        /// Lattice angle beta in radian (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (Double Value, Boolean Fixed) Beta { get; internal set; }

        /// <summary>
        /// Lattice angle gamma in radian (Value and flag if value is fixed by crystal system)
        /// </summary>
        public (Double Value, Boolean Fixed) Gamma { get; internal set; }

        /// <summary>
        /// Sets the parameters if they pass the constraints of the system, else returns false
        /// </summary>
        /// <param name="paramSet"></param>
        /// <returns></returns>
        public Boolean TrySetParameters(CrystalParameterSet paramSet)
        {
            ApplyParameterDependencies(paramSet);
            if (ValidateAngleConditions(paramSet.Alpha, paramSet.Beta, paramSet.Gamma) == false)
            {
                return false;
            }
            if (ValidateParameterConstraintCondition(paramSet.ParamA, paramSet.ParamB, paramSet.ParamC) == false)
            {
                return false;
            }
            SetParameters(paramSet);
            return true;
        }

        /// <summary>
        /// Sets the parameters and angles from a paremeter set without checking the validity
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
        /// Creates a fractional coordinate system from the crytsal system parameters
        /// </summary>
        /// <returns></returns>
        public FractionalCoordinateSystem3D CreateCoordinateSystem()
        {
            if (!IsReady)
            {
                throw new InvalidObjectStateException("Cannot calculate a coordinate system, required parameters are not set");
            }
            var (A, B, C) = CalculateBaseVectors();
            if (BasicVectorValidation(A, B, C) == false)
            {
                throw new InvalidObjectStateException("Cannot calculate a coordinate system, the parameters form invalid base vectors");
            }
            return new FractionalCoordinateSystem3D(A, B, C, BasicConstraint.Comparer);
        }

        /// <summary>
        /// Checks if three radians angles are basically valid crystal system angles without specified cyrstal system type
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public Boolean ValidateGeneralAngleCondition(Double alpha, Double beta, Double gamma)
        {
            Double cosAlpha = Math.Cos(alpha);
            Boolean con1 = Math.Cos(beta + gamma) < cosAlpha && cosAlpha < Math.Cos(beta - gamma);
            Boolean con2 = Math.Cos(beta - gamma) < cosAlpha && cosAlpha < Math.Cos(beta + gamma);
            return con1 || con2;
        }

        /// <summary>
        /// Validates that the angles fulfill the angle constraints specified by the implementing system
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public Boolean ValidateAngleConstraintCondition(Double alpha, Double beta, Double gamma)
        {
            return AlphaConstraint.IsValid(alpha) && BetaConstraint.IsValid(beta) && GammaConstraint.IsValid(gamma);
        }

        /// <summary>
        /// Performs an angle validation that tests the soft angle condition of the crystall system (no hierachy enforcement)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public abstract Boolean ValidateSoftAngleCondition(Double alpha, Double beta, Double gamma);

        /// <summary>
        /// Performs a full angle validation that checks of both system specific and general sufficient angle conditions are met
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public Boolean ValidateAngleConditions(Double alpha, Double beta, Double gamma)
        {
            if (ValidateGeneralAngleCondition(alpha, beta, gamma) == false)
            {
                return false;
            }
            if (ValidateAngleConstraintCondition(alpha, beta, gamma) == false)
            {
                return false;
            }
            return ValidateSoftAngleCondition(alpha, beta, gamma);
        }

        /// <summary>
        /// Validate that the cell parameters fulfill both soft system condition and general parameter constraints
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public Boolean ValidateParameterConditions(Double paramA, Double paramB, Double paramC)
        {
            if (ValidateParameterConstraintCondition(paramA, paramB, paramC) == false)
            {
                return false;
            }
            return ValidateSoftParameterCondition(paramA, paramB, paramC);
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the parameter constraints of the implementing system
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public Boolean ValidateParameterConstraintCondition(Double paramA, Double paramB, Double paramC)
        {
            return ParameterConstraint.IsValid(paramA) && ParameterConstraint.IsValid(paramB) && ParameterConstraint.IsValid(paramC);
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the soft parameter condition (No hierachy enforcement)
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public abstract Boolean ValidateSoftParameterCondition(Double paramA, Double paramB, Double paramC);

        /// <summary>
        /// Calculates the base vectors from the crystal system parameters
        /// </summary>
        /// <returns></returns>
        public (ACoorTuple A, ACoorTuple B, ACoorTuple C) CalculateBaseVectors()
        {
            return (GetLatticeVectorA(), GetLatticeVectorB(), GetLatticeVectorC());
        }

        /// <summary>
        /// Calculates the lattice vector in A direction from the system parameters
        /// </summary>
        /// <returns></returns>
        public ACoorTuple GetLatticeVectorA()
        {
            return new ACoorTuple(ParamA.Value, 0.0, 0.0);
        }

        /// <summary>
        /// Calculates the lattice vector in B direction from the system parameters
        /// </summary>
        /// <returns></returns>
        public ACoorTuple GetLatticeVectorB()
        {
            Double a = (ParamB.Value * Math.Cos(Gamma.Value)).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            Double b = (ParamB.Value * Math.Sin(Gamma.Value)).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            return new ACoorTuple(a, b, 0.0);
        }

        /// <summary>
        /// Calculates the lattice vector in C direction from the system parameters
        /// </summary>
        /// <returns></returns>
        public ACoorTuple GetLatticeVectorC()
        {
            Double a = (ParamC.Value * Math.Cos(Beta.Value)).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            Double b = (ParamC.Value * Math.Cos(Alpha.Value) - Math.Cos(Gamma.Value) * Math.Cos(Beta.Value) / Math.Sin(Gamma.Value)).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            Double c = (Math.Sqrt(ParamC.Value * ParamC.Value - a * a - b * b)).ZeroSafeRound(0.0, BasicConstraint.Comparer);
            return new ACoorTuple(a, b, c);
        }

        /// <summary>
        /// Validates that the lattice vectors are potentially valid lattice vectors without checking if they match the actual crystal system type
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <param name="vectorC"></param>
        /// <returns></returns>
        public Boolean BasicVectorValidation(ACoorTuple vectorA, ACoorTuple vectorB, ACoorTuple vectorC)
        {
            return vectorA.IsLinearIndependentFrom(vectorB, vectorC, BasicConstraint.Comparer);
        }

        /// <summary>
        /// Checks if a vector length is valid
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        protected Boolean ValidateVectorLength(ref ACoorTuple vector)
        {
            return !BasicConstraint.Comparer.Equals(vector.GetLength(), 0.0);
        }

        /// <summary>
        /// Corrects potential parameter dependencies in a cyrstal parameter set using the rules of the specific cyrstal system implementation
        /// </summary>
        /// <param name="paramSet"></param>
        public abstract void ApplyParameterDependencies(CrystalParameterSet paramSet);

        /// <summary>
        /// Retruns a default parameter set that fullfills all restrictions of the actual crystal system
        /// </summary>
        /// <returns></returns>
        public abstract CrystalParameterSet GetDefaultParameterSet();
    }
}
