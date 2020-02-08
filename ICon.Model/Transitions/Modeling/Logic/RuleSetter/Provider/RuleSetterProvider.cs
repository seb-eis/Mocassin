using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc />
    public class RuleSetterProvider : IRuleSetterProvider
    {
        /// <summary>
        ///     Get The <see cref="IDataAccessorSource{TData}" /> to manipulate the required <see cref="TransitionModelData" />
        /// </summary>
        private IDataAccessorSource<TransitionModelData> DataAccessorSource { get; }

        /// <inheritdoc />
        public IValueConstraint<double, double> AttemptFrequencyConstraint { get; set; }

        /// <summary>
        ///     Create new <see cref="RuleSetterProvider" /> that uses the provided
        ///     <see cref="IDataAccessorSource{TData}" />
        /// </summary>
        /// <param name="dataAccessorSource"></param>
        public RuleSetterProvider(IDataAccessorSource<TransitionModelData> dataAccessorSource)
        {
            DataAccessorSource = dataAccessorSource ?? throw new ArgumentNullException(nameof(dataAccessorSource));
        }

        /// <inheritdoc />
        public IReadOnlyList<IKineticRuleSetter> GetRuleSetters()
        {
            using var accessor = DataAccessorSource.Create();
            return accessor.Query(x => x.KineticTransitions.Select(GetRuleSetter).ToList());
        }

        /// <inheritdoc />
        public IKineticRuleSetter GetRuleSetter(IKineticTransition kineticTransition)
        {
            using var accessor = DataAccessorSource.CreateUnsafe();
            var transition = accessor.Query(x => x.KineticTransitions.SingleOrDefault(y => TransitionsAreEqual(y, kineticTransition)));
            return transition is null
                ? null
                : new KineticRuleSetter(transition, DataAccessorSource) {AttemptFrequencyConstraint = AttemptFrequencyConstraint};
        }

        /// <summary>
        ///     Check if the two passed <see cref="IKineticTransition" /> objects have equal identifiers
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        protected bool TransitionsAreEqual(IKineticTransition lhs, IKineticTransition rhs)
        {
            return lhs.Index == rhs.Index || lhs.Key.Equals(rhs.Key);
        }
    }
}