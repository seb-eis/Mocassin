using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="Mocassin.Model.Translator.ModelContext.IKineticLocalJumpModel" />
    public class KineticLocalJumpModel : ModelComponentBase, IKineticLocalJumpModel
    {
        /// <inheritdoc />
        public IKineticMappingModel MappingModel { get; set; }

        /// <inheritdoc />
        public IKineticRuleModel RuleModel { get; set; }

        /// <inheritdoc />
        public double NormalizedElectricFieldInfluence { get; set; }

        /// <inheritdoc />
        public ITransitionRuleModel RuleModelBase => RuleModel;

        /// <inheritdoc />
        public ITransitionMappingModel MappingModelBase => MappingModel;

        /// <inheritdoc />
        public double ElectricFieldRuleInfluence => RuleModel.RuleDirectionValue;

        /// <inheritdoc />
        public double ElectricFieldMappingFactor { get; set; }

        /// <inheritdoc />
        public Cartesian3D ChargeTransportVector { get; set; }

        /// <inheritdoc />
        public MobilityType GetMobilityType(int positionId, int particleId)
        {
            for (var pathId = 0; pathId < MappingModel.Mapping.PathLength; pathId++)
            {
                if (RuleModel.StartState[pathId].Index == particleId &&
                    RuleModel.FinalState[pathId].Index != particleId &&
                    MappingModel.PositionSequence4D[pathId].P == positionId)
                    return pathId == 0 ? MobilityType.Selectable : MobilityType.Mobile;
            }

            return MobilityType.Immobile;
        }

        /// <inheritdoc />
        public bool Equals(IKineticLocalJumpModel other)
        {
            return other != null
                   && MappingModel.Equals(other.MappingModel)
                   && RuleModel.Equals(other.RuleModel);
        }
    }
}