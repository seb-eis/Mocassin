using Mocassin.Framework.Exceptions;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Class for a soft monoclinic crystal system that supports the crystal system hierarchy (i.e. the hard monoclinic
    ///     condition is not enforced)
    /// </summary>
    public class SoftMonoclinicCrystalSystem : CrystalSystem
    {
        /// <inheritdoc />
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            switch (Variation)
            {
                case CrystalVariation.UniqueAxisA:
                    paramSet.Beta = Beta.Value;
                    paramSet.Gamma = Gamma.Value;
                    return;

                case CrystalVariation.UniqueAxisB:
                    paramSet.Alpha = Alpha.Value;
                    paramSet.Gamma = Gamma.Value;
                    return;

                case CrystalVariation.UniqueAxisC:
                    paramSet.Alpha = Alpha.Value;
                    paramSet.Beta = Beta.Value;
                    return;

                default:
                    throw new InvalidObjectStateException("The monoclinic crystal system does not have a valid unique axis flag");
            }
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma)
        {
            switch (Variation)
            {
                case CrystalVariation.UniqueAxisA:
                    return MocassinMath.Radian90.CountMatches(BasicConstraint.Comparer, beta, gamma) == 2;

                case CrystalVariation.UniqueAxisB:
                    return MocassinMath.Radian90.CountMatches(BasicConstraint.Comparer, alpha, gamma) == 2;

                case CrystalVariation.UniqueAxisC:
                    return MocassinMath.Radian90.CountMatches(BasicConstraint.Comparer, alpha, beta) == 2;

                default:
                    throw new InvalidObjectStateException("The monoclinic crystal system has an invalid crystal system variation flag");
            }
        }

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC)
        {
            return true;
        }

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
        }
    }
}