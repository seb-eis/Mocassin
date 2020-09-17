using System.Collections.Generic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a metropolis transition rule that describes a static state exchange
    /// </summary>
    public interface IMetropolisRule : ITransitionRule
    {
        /// <summary>
        ///     Get the metropolis parent transition
        /// </summary>
        IMetropolisTransition Transition { get; }

        /// <summary>
        ///     Get all dependent rules that are a direct result of this rule
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMetropolisRule> GetDependentRules();
    }
}