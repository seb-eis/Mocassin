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
        public double NormalizedElectricFieldInfluence { get; set; }

        /// <inheritdoc />
        public double ElectricFieldRuleFactor => NormalizedElectricFieldInfluence / ElectricFieldMappingFactor;

        /// <inheritdoc />
        public double ElectricFieldMappingFactor => System.Math.Abs(NormalizedElectricFieldInfluence);

        /// <inheritdoc />
        public Cartesian3D ChargeTransportVector { get; set; }
    }
}