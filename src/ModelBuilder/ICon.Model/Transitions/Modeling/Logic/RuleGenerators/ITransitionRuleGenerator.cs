using System.Collections.Generic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a rule generator that translates <see cref="IAbstractTransition"/> definitions into the affiliated <see cref="TransitionRule"/> sequences
    /// </summary>
    /// <typeparam name="TRule"></typeparam>
    public interface ITransitionRuleGenerator<out TRule> where TRule : TransitionRule, new()
    {
        /// <summary>
        ///     Makes the unique rule sequence for each passed abstract transition. With option flag to control if the system
        ///     should automatically filter out rules that are not supported
        /// </summary>
        /// <param name="abstractTransitions"></param>
        /// <param name="onlySupported"></param>
        /// <returns></returns>
        /// <remarks> Unsupported rules are typically physically meaningless e.g. they violate matter conservation rules </remarks>
        IEnumerable<IEnumerable<TRule>> MakeUniqueRules(IEnumerable<IAbstractTransition> abstractTransitions, bool onlySupported);
    }
}