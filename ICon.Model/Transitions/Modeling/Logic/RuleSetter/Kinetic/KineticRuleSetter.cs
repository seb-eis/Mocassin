using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.IKineticRuleSetter" />
    public class KineticRuleSetter : ValueSetter, IKineticRuleSetter
    {
        /// <summary>
        ///     The <see cref="KineticTransition" /> object that is managed by the setter
        /// </summary>
        private KineticTransition Transition { get; }

        /// <summary>
        ///     The dictionary to store frequency value overwrites before push operations
        /// </summary>
        private Dictionary<IKineticRule, double> RuleDictionary { get; }

        /// <inheritdoc />
        public IValueConstraint<double, double> AttemptFrequencyConstraint { get; set; }

        /// <inheritdoc />
        IKineticTransition IKineticRuleSetter.KineticTransition => Transition;

        /// <inheritdoc />
        IReadOnlyCollection<IKineticRule> IKineticRuleSetter.KineticRules => Transition.TransitionRules;

        /// <summary>
        ///     Get or set the <see cref="TransitionModelData" />  access provider to lock the required data object during push
        ///     operations
        /// </summary>
        /// <remarks> Used to lock the energy data object as long as the setter is writing energy values </remarks>
        public IDataAccessorSource<TransitionModelData> DataAccessorSource { get; set; }

        /// <summary>
        ///     Creates new <see cref="KineticRuleSetter" /> for the passed <see cref="KineticTransition" /> object
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="dataAccessorSource"></param>
        public KineticRuleSetter(KineticTransition transition, IDataAccessorSource<TransitionModelData> dataAccessorSource)
        {
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
            RuleDictionary = transition.TransitionRules.ToDictionary(rule => (IKineticRule) rule, rule => rule.AttemptFrequency);
            DataAccessorSource = dataAccessorSource ?? throw new ArgumentNullException(nameof(dataAccessorSource));
        }

        /// <inheritdoc />
        public override void PushData()
        {
            using (DataAccessorSource.Create())
            {
                foreach (var entry in RuleDictionary)
                {
                    var rule = (KineticRule) entry.Key;
                    rule.AttemptFrequency = entry.Value;
                    foreach (var dependentRule in rule.DependentRules)
                    {
                        dependentRule.AttemptFrequency = entry.Value;
                        OnValuesPushed.OnNext();
                    }
                }
            }
        }

        /// <inheritdoc />
        public void SetAttemptFrequency(IKineticRule rule, double value)
        {
            if (!AttemptFrequencyConstraint.IsValid(value))
            {
                OnValueChanged.OnError(new ArgumentException("Frequency value violates the constraints", nameof(value)));
                return;
            }

            if (!RuleDictionary.Keys.Contains(rule))
            {
                OnValueChanged.OnError(new ArgumentException("The passed rule entry does not exists in this context", nameof(rule)));
                return;
            }

            RuleDictionary[rule] = value;
            OnValueChanged.OnNext();
        }
    }
}