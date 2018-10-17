using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Class for a soft hexagonal crystal system that supports the crystal system hierarchy (i.e. the hard hexagonal
    ///     condition is not enforced)
    /// </summary>
    public class SoftHexagonalCrystalSystem : CrystalSystem
    {
        /// <inheritdoc />
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            paramSet.ParamB = paramSet.ParamA;
            paramSet.Alpha = MocassinMath.Radian90;
            paramSet.Beta = MocassinMath.Radian90;
            paramSet.Gamma = MocassinMath.Radian120;
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma)
        {
            var isValid = true;
            isValid &= BasicConstraint.Comparer.Equals(alpha, MocassinMath.Radian90);
            isValid &= BasicConstraint.Comparer.Equals(beta, MocassinMath.Radian90);
            isValid &= BasicConstraint.Comparer.Equals(gamma, MocassinMath.Radian120);
            return isValid;
        }

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC)
        {
            return BasicConstraint.Comparer.Equals(paramA, paramB);
        }

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian120);
        }
    }
}