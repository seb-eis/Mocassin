using System;
using System.Collections.Generic;
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
        public double ElectricFieldRuleFactor => Math.Abs(NormalizedElectricFieldInfluence) > 1.0e-10 
            ? NormalizedElectricFieldInfluence / ElectricFieldMappingFactor
            : 0;

        /// <inheritdoc />
        public double ElectricFieldMappingFactor => System.Math.Abs(NormalizedElectricFieldInfluence);

        /// <inheritdoc />
        public Cartesian3D ChargeTransportVector { get; set; }

        /// <inheritdoc />
        public bool Equals(IKineticLocalJumpModel other)
        {
            return other != null 
                   && MappingModel.Equals(other.MappingModel) 
                   && RuleModel.Equals(other.RuleModel);
        }
    }
}