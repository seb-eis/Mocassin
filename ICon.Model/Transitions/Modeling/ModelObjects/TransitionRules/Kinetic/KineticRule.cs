using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Model.Particles;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Kinetic rule implementation that extends the basic transition rule by the kinetic properties and functionalities
    /// </summary>
    [DataContract]
    public class KineticRule : TransitionRule, IKineticRule
    {
        /// <summary>
        /// The parent kinetic transition instance
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IKineticTransition Transition { get; set; }

        /// <summary>
        /// The attempt frequency value in [1/s]
        /// </summary>
        [DataMember]
        public double AttemptFrequency { get; set; }

        /// <summary>
        /// The cell boundary flags of the rule that enables deactivation of specfific boundaries
        /// </summary>
        [DataMember]
        public CellBoundaryFlags BoundaryFlags { get; set; }

        /// <summary>
        /// The list of dependent rules that are a direct result of this rule
        /// </summary>
        [DataMember]
        public List<KineticRule> DependentRules { get; set; }

        /// <summary>
        /// Creates new kinetic rule with empty dependent rule list
        /// </summary>
        public KineticRule()
        {
            DependentRules = new List<KineticRule>(); 
        }

        /// <summary>
        /// Get the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Kinetic Rule'";
        }

        /// <summary>
        /// Set the attempt frequency. This value is not passed to linked rules
        /// </summary>
        /// <param name="value"></param>
        public void SetAttemptFrequency(double value)
        {
            AttemptFrequency = value;
        }

        /// <summary>
        /// Set the cell boundary flags. This value is not passed to linked transitions
        /// </summary>
        /// <param name="flags"></param>
        public void SetCellBoundaryFlags(CellBoundaryFlags flags)
        {
            BoundaryFlags = flags;
        }

        /// <summary>
        /// Populates the object by  a model object interface and returns this object (Retruns null if population failed)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IKineticRule>(obj) is var rule)
            {
                base.PopulateFrom(obj);
                Transition = rule.Transition;
                AttemptFrequency = rule.AttemptFrequency;
                BoundaryFlags = rule.BoundaryFlags;
            }
            return null;
        }

        /// <summary>
        /// Get all dependent rules of this rule
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IKineticRule> GetDependentRules()
        {
            return DependentRules.AsEnumerable();
        }

        /// <summary>
        /// Adds a dependent kinetic transition rule. Throw if the passed rule is not a kinetic rule
        /// </summary>
        /// <param name="rule"></param>
        public override void AddDependentRule(TransitionRule rule)
        {
            if (rule is KineticRule kineticRule)
            {
                DependentRules.Add(kineticRule);
                return;
            }
            throw new ArgumentException("Passed rule does not inherit from a kinetic rule", nameof(rule));
        }
    }
}
