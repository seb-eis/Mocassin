using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Fully describes the behaviour of a transition mapping model in the context of a metropolis simulation model
    /// </summary>
    public interface IMetropolisLocalJumpModel : IModelComponent, IEquatable<IMetropolisLocalJumpModel>
    {
        /// <summary>
        /// The metropolis mapping model that describes the geometry of the local jump model
        /// </summary>
        IMetropolisMappingModel MappingModel { get; set; }

        /// <summary>
        /// The metropolis rule model that is valid for the jump model
        /// </summary>
        IMetropolisRuleModel RuleModel { get; set; }
    }
}