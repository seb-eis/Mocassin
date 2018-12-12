using System.Collections.Generic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a kinetic transition rule that describes a dynamic state change process and allows for setting of
    ///     boundary flags and attempt frequency
    /// </summary>
    public interface IKineticRule : ITransitionRule
    {
        /// <summary>
        ///     Get the parent kinetic transition
        /// </summary>
        IKineticTransition Transition { get; }

        /// <summary>
        ///     The attempt frequency of this rule
        /// </summary>
        double AttemptFrequency { get; }

        /// <summary>
        ///     Get an enumerable of dependent rules that are a result of this one
        /// </summary>
        /// <returns></returns>
        IEnumerable<IKineticRule> GetDependentRules();
    }
}