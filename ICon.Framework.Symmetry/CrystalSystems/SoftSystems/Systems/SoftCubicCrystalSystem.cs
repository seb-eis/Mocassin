using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Class for a soft cubic crystal system that supports the crystal system hierarchy (i.e. the hard cubic condition is
    ///     not enforced)
    /// </summary>
    public class SoftCubicCrystalSystem : CrystalSystem
    {
        /// <inheritdoc />
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            paramSet.ParamB = paramSet.ParamA;
            paramSet.ParamC = paramSet.ParamA;
            paramSet.Alpha = MocassinMath.Radian90;
            paramSet.Beta = MocassinMath.Radian90;
            paramSet.Gamma = MocassinMath.Radian90;
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma)
        {
            return MocassinMath.Radian90.CountMatches(BasicConstraint.Comparer, alpha, beta, gamma) == 3;
        }

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC)
        {
            return paramA.CountMatches(BasicConstraint.Comparer, paramB, paramC) == 3;
        }

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
        }
    }
}