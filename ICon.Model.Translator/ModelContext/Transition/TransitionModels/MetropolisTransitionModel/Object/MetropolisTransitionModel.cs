﻿using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IMetropolisTransitionModel" />
    public class MetropolisTransitionModel : TransitionModel, IMetropolisTransitionModel
    {
        /// <inheritdoc />
        public IMetropolisTransition Transition { get; set; }

        /// <inheritdoc />
        public IMetropolisTransitionModel InverseTransitionModel { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisMappingModel> MappingModels { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisRuleModel> RuleModels { get; set; }

        /// <inheritdoc />
        public override IEnumerable<ITransitionMappingModel> GetMappingModels()
        {
            return MappingModels.AsEnumerable();
        }

        /// <inheritdoc />
        public override IEnumerable<ITransitionRuleModel> GetRuleModels()
        {
            return RuleModels.AsEnumerable();
        }

        /// <inheritdoc />
        public IMetropolisTransitionModel CreateGeometricInversion()
        {
            return new MetropolisTransitionModel
            {
                InverseTransitionModel = this,
                Transition = Transition
            };
        }
    }
}