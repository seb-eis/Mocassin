using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="Mocassin.Model.Translator.ModelContext.IKineticLocalJumpModel"/>
    public class KineticLocalJumpModel : ModelComponentBase, IKineticLocalJumpModel
    {
        /// <inheritdoc />
        public IKineticMappingModel MappingModel { get; set; }

        /// <inheritdoc />
        public IKineticRuleModel RuleModel { get; set; }

        /// <inheritdoc />
        public Fractional3D ElectricFieldProjectionVector { get; set; }

        /// <inheritdoc />
        public double ElectricFieldInfluenceFraction { get; set; }

        /// <inheritdoc />
        public double TransportCharge { get; set; }
    }
}