using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <inheritdoc cref="ICon.Model.Transitions.IKineticTransition"/>
    [Serializable]
    [DataContract(Name ="KineticTransition")]
    public class KineticTransition : ModelObject, IKineticTransition
    {
        /// <inheritdoc />
        [DataMember]
        [LinkableByIndex]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        /// The geometry of the transition as 3D fractional coordinates
        /// </summary>
        [DataMember]
        public List<DataVector3D> PathGeometry { get; set; }

        /// <summary>
        /// The list of affiliated kinetic transition rules (auto-managed by the model)
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public List<KineticRule> TransitionRules { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        public int GeometryStepCount => PathGeometry.Count;

        /// <inheritdoc />
        public IEnumerable<IKineticRule> GetExtendedTransitionRules()
        {
            foreach (var rule in GetTransitionRules())
            {
                yield return rule;
                foreach (var dependentRule in rule.GetDependentRules())
                {
                    yield return dependentRule;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> GetGeometrySequence()
        {
            return (PathGeometry ?? new List<DataVector3D>()).Select(value => value.AsFractional());
        }

        /// <inheritdoc />
        public IEnumerable<IKineticRule> GetTransitionRules()
        {
            return (TransitionRules ?? new List<KineticRule>()).AsEnumerable();
        }

        /// <inheritdoc />
        public override string GetModelObjectName()
        {
            return "'Kinetic Transition Rule'";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IKineticTransition>(obj) is var transition)
            {
                PathGeometry = transition.GetGeometrySequence().Select(value => new DataVector3D(value)).ToList();
                AbstractTransition = transition.AbstractTransition;
                return this;
            }
            return null;
        }
    }
}
