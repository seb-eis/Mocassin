using Mocassin.Framework.Exceptions;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Class for a soft trigonal crystal system that supports the crystal system hierarchy (i.e. the hard trigonal
    ///     condition is not enforced)
    /// </summary>
    public class SoftTrigonalCrystalSystem : CrystalSystem
    {
        /// <inheritdoc />
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            switch (SystemVariation)
            {
                case CrystalSystemVariation.HexagonalAxes:
                    paramSet.ParamB = paramSet.ParamA;
                    paramSet.Alpha = Alpha.Value;
                    paramSet.Beta = Beta.Value;
                    paramSet.Gamma = Gamma.Value;
                    return;

                case CrystalSystemVariation.RhombohedralAxes:
                    paramSet.ParamB = paramSet.ParamA;
                    paramSet.ParamC = paramSet.ParamA;
                    paramSet.Beta = paramSet.Alpha;
                    paramSet.Gamma = paramSet.Alpha;
                    return;

                default:
                    throw new InvalidObjectStateException("The trigonal crystal system has an invalid crystal system variation flag");
            }
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma)
        {
            var isValid = true;
            switch (SystemVariation)
            {
                case CrystalSystemVariation.HexagonalAxes:
                    isValid &= BasicConstraint.Comparer.Equals(alpha, MocassinMath.Radian90);
                    isValid &= BasicConstraint.Comparer.Equals(beta, MocassinMath.Radian90);
                    isValid &= BasicConstraint.Comparer.Equals(gamma, MocassinMath.Radian120);
                    return isValid;

                case CrystalSystemVariation.RhombohedralAxes:
                    isValid = alpha.CountMatches(BasicConstraint.Comparer, beta, gamma) == 2;
                    return isValid;

                default:
                    throw new InvalidObjectStateException("The trigonal crystal system has an invalid crystal system variation flag");
            }
        }

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC)
        {
            switch (SystemVariation)
            {
                case CrystalSystemVariation.HexagonalAxes:
                    return BasicConstraint.Comparer.Equals(paramA, paramB);

                case CrystalSystemVariation.RhombohedralAxes:
                    return paramA.CountMatches(BasicConstraint.Comparer, paramB, paramC) == 2;

                default:
                    throw new InvalidObjectStateException("The trigonal crystal system has an invalid crystal system variation flag");
            }
        }

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            switch (SystemVariation)
            {
                case CrystalSystemVariation.HexagonalAxes:
                    return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian120);

                case CrystalSystemVariation.RhombohedralAxes:
                    return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);

                default:
                    throw new InvalidObjectStateException("The trigonal crystal system has an invalid crystal system variation flag");
            }
        }
    }
}