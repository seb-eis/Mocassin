using System.Collections.Generic;
using Mocassin.Framework.Constraints;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Provider for <see cref="Mocassin.Model.Basic.IValueSetter" /> for <see cref="ITransitionRule" /> objects
    /// </summary>
    public interface IRuleSetterProvider
    {
        /// <summary>
        ///     Get the <see cref="IValueConstraint{TSource,TTarget}" /> that limits the attempt frequency of
        ///     <see cref="IKineticRule" /> objects
        /// </summary>
        IValueConstraint<double, double> AttemptFrequencyConstraint { get; }

        /// <summary>
        ///     Get all <see cref="IKineticRuleSetter" /> for all <see cref="IKineticTransition" /> objects
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IKineticRuleSetter> GetRuleSetters();

        /// <summary>
        ///     Get a <see cref="IKineticRuleSetter" /> for the passed <see cref="IKineticTransition" /> object
        /// </summary>
        /// <param name="kineticTransition"></param>
        /// <returns></returns>
        IKineticRuleSetter GetRuleSetter(IKineticTransition kineticTransition);
    }
}