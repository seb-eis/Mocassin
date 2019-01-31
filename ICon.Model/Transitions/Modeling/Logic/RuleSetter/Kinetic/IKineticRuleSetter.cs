using System.Collections.Generic;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Value setter that enables manipulation of the <see cref="IKineticRule" /> parameters
    /// </summary>
    public interface IKineticRuleSetter : IValueSetter
    {
        /// <summary>
        ///     Get the <see cref="IValueConstraint{TSource,TTarget}" /> that limits the settable attempt frequency value range
        /// </summary>
        IValueConstraint<double, double> AttemptFrequencyConstraint { get; }

        /// <summary>
        ///     Get the <see cref="IKineticTransition" /> that the setter is for
        /// </summary>
        IKineticTransition KineticTransition { get; }

        /// <summary>
        ///     Get a read only collection of all <see cref="IKineticRule" /> objects that can be manipulated by the setter
        /// </summary>
        IReadOnlyCollection<IKineticRule> KineticRules { get; }

        /// <summary>
        ///     Set the attempt frequency of the passed <see cref="IKineticRule" /> to the passed value
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="value"></param>
        void SetAttemptFrequency(IKineticRule rule, double value);
    }
}