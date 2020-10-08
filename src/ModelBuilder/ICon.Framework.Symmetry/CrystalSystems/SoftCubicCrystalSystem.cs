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
            paramSet.Alpha = Alpha.Value;
            paramSet.Beta = Beta.Value;
            paramSet.Gamma = Gamma.Value;
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma) =>
            MocassinMath.Radian90.CountMatchesWithParameters(BasicConstraint.Comparer, alpha, beta, gamma) == 3;

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC) =>
            paramA.CountMatchesWithParameters(BasicConstraint.Comparer, paramB, paramC) == 2;

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet() => new CrystalParameterSet(1.0, 1.0, 1.0, Alpha.Value, Beta.Value, Gamma.Value);
    }
}