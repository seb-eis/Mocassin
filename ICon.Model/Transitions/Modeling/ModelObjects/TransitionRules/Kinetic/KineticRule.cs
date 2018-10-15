using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ICon.Model.Basic;
using ICon.Model.Particles;

namespace ICon.Model.Transitions
{
    /// <inheritdoc cref="ICon.Model.Transitions.IKineticRule"/>
    [DataContract]
    public class KineticRule : TransitionRule, IKineticRule
    {
        /// <inheritdoc />
        [DataMember]
        [IndexResolved]
        public IKineticTransition Transition { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double AttemptFrequency { get; set; }

        /// <inheritdoc />
        [DataMember]
        public CellBoundaryFlags BoundaryFlags { get; set; }

        /// <summary>
        /// The list of dependent rules that are a direct result of this rule
        /// </summary>
        [DataMember]
        public List<KineticRule> DependentRules { get; set; }

        /// <inheritdoc />
        public KineticRule()
        {
            DependentRules = new List<KineticRule>();
        }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "'Kinetic Rule'";
        }

        /// <inheritdoc />
        public void SetAttemptFrequency(double value)
        {
            AttemptFrequency = value;
        }

        /// <inheritdoc />
        public void SetCellBoundaryFlags(CellBoundaryFlags flags)
        {
            BoundaryFlags = flags;
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IKineticRule>(obj) is IKineticRule rule))
                return null;

            base.PopulateFrom(obj);
            Transition = rule.Transition;
            AttemptFrequency = rule.AttemptFrequency;
            BoundaryFlags = rule.BoundaryFlags;

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<IKineticRule> GetDependentRules()
        {
            return DependentRules.AsEnumerable();
        }

        /// <inheritdoc />
        public override void AddDependentRule(TransitionRule rule)
        {
            if (!(rule is KineticRule kineticRule))
                throw new ArgumentException("Passed rule does not inherit from a kinetic rule", nameof(rule));

            kineticRule.Transition = Transition;
            DependentRules.Add(kineticRule);
        }
    }
}