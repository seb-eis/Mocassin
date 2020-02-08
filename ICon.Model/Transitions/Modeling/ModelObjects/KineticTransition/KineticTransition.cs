using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="IKineticTransition" />
    public class KineticTransition : ModelObject, IKineticTransition
    {
        /// <inheritdoc />
        [UseTrackedData]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        ///     The geometry of the transition as 3D fractional coordinates
        /// </summary>
        public List<Fractional3D> PathGeometry { get; set; }

        /// <summary>
        ///     The list of affiliated kinetic transition rules (auto-managed by the model)
        /// </summary>
        [UseTrackedData]
        public List<KineticRule> TransitionRules { get; set; }

        /// <inheritdoc />
        public int GeometryStepCount => PathGeometry.Count;

        /// <inheritdoc />
        public override string ObjectName => "Kinetic Transition";

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
            return (PathGeometry ?? new List<Fractional3D>()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IKineticRule> GetTransitionRules()
        {
            return (TransitionRules ?? new List<KineticRule>()).AsEnumerable();
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IKineticTransition>(obj) is { } transition))
                return null;

            PathGeometry = transition.GetGeometrySequence().ToList();
            AbstractTransition = transition.AbstractTransition;
            return this;
        }
    }
}