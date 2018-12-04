using System;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="Mocassin.Model.Translator.ModelContext.IMetropolisLocalJumpModel"/>
    public class MetropolisLocalJumpModel : ModelComponentBase, IMetropolisLocalJumpModel
    {
        /// <inheritdoc />
        public IMetropolisMappingModel MappingModel { get; set; }

        /// <inheritdoc />
        public IMetropolisRuleModel RuleModel { get; set; }

        /// <inheritdoc />
        public bool Equals(IMetropolisLocalJumpModel other)
        {
            return other != null 
                   && MappingModel.Equals(other.MappingModel) 
                   && RuleModel.Equals(other.RuleModel);
        }

        /// <inheritdoc />
        public ITransitionRuleModel RuleModelBase => RuleModel;

        /// <inheritdoc />
        public ITransitionMappingModel MappingModelBase => MappingModel;

        /// <inheritdoc />
        public double ElectricFieldRuleInfluence => SimulationConstants.UndefinableRuleDirectionFactor;

        /// <inheritdoc />
        public double ElectricFieldMappingFactor
        {
            get => 0.0;
            set => throw new NotSupportedException("Cannot set field factors on a metropolis transition");
        }

        /// <inheritdoc />
        public bool MakesElementOnPositionMobile(int positionId, int particleId)
        {

            if (RuleModel.StartState[0].Index == particleId &&
                RuleModel.FinalState[0].Index != particleId &&
                MappingModel.StartVector4D.P == positionId)
                return true;

            return RuleModel.StartState[1].Index == particleId &&
                   RuleModel.FinalState[1].Index != particleId &&
                   MappingModel.EndVector4D.P == positionId;
        }
    }
}