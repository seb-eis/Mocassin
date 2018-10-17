using Mocassin.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a metropolis transition model that fully describes geometry and mobility information of a reference transition
    /// </summary>
    public interface IMetropolisTransitionModel : ITransitionModel
    {
        /// <summary>
        /// The transition the model is based upon
        /// </summary>
        IMetropolisTransition Transition { get; set; }

        /// <summary>
        /// The inverse transition model that describes the neutralizing transition
        /// </summary>
        /// <remarks> Is equal to itself if start and end unit cell positions are identical </remarks>
        IMetropolisTransitionModel InverseTransitionModel { get; set; }

        /// <summary>
        /// The list of existing mapping models that describe all possible geometries this transition model can be applied to
        /// </summary>
        IList<IMetropolisMappingModel> MappingModels { get; set; }

        /// <summary>
        /// The extended rule models that contain all possible rules for the transition
        /// </summary>
        IList<IMetropolisRuleModel> RuleModels { get; set; }

        /// <summary>
        /// Creates an inverse transition model
        /// </summary>
        /// <returns></returns>
        IMetropolisTransitionModel CreateInverse();
    }
}
