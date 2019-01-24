using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="IKineticTransition" />
    [Serializable]
    [DataContract(Name = "KineticTransition")]
    public class KineticTransition : ModelObject, IKineticTransition
    {
        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        ///     The geometry of the transition as 3D fractional coordinates
        /// </summary>
        [DataMember]
        public List<DataVector3D> PathGeometry { get; set; }

        /// <summary>
        ///     The list of affiliated kinetic transition rules (auto-managed by the model)
        /// </summary>
        [DataMember]
        [UseTrackedReferences]
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
                    yield return dependentRule;
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
        public override string GetObjectName()
        {
            return "Kinetic Transition Rule";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IKineticTransition>(obj) is IKineticTransition transition))
                return null;

            PathGeometry = transition.GetGeometrySequence().Select(value => new DataVector3D(value)).ToList();
            AbstractTransition = transition.AbstractTransition;
            return this;

        }
    }
}