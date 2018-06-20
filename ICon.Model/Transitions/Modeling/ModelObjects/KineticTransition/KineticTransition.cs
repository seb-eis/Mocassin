using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Kinetic model transition that defines a full reference transition by geoemtry index and abstract transition index
    /// </summary>
    [Serializable]
    [DataContract(Name ="KineticTransition")]
    public class KineticTransition : ModelObject, IKineticTransition
    {
        /// <summary>
        /// The index of the abstract transition description
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public IAbstractTransition AbstractTransition { get; set; }

        /// <summary>
        /// The geometry of the transtion as 3D fractional coordinates
        /// </summary>
        [DataMember]
        public List<DataVector3D> PathGeometry { get; set; }

        /// <summary>
        /// The list of affiliated kinetic transition rules (automanaged by the model)
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public List<KineticRule> TransitionRules { get; set; }

        /// <summary>
        /// The number of geometry steps of the transition
        /// </summary>
        [IgnoreDataMember]
        public int GeometryStepCount => PathGeometry.Count;

        /// <summary>
        /// Get the geometry of the transition as sequence of 3D fractional vectors
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Fractional3D> GetGeometrySequence()
        {
            return (PathGeometry ?? new List<DataVector3D>()).Select(value => value.AsFractional());
        }

        /// <summary>
        /// Get the affilaited transition rules of the transiton
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IKineticRule> GetTransitionRules()
        {
            return (TransitionRules ?? new List<KineticRule>()).AsEnumerable();
        }

        /// <summary>
        /// Get the type name of the model object
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Kinetic Transition'";
        }

        /// <summary>
        /// Tries to create a new model transition from a model object interface (Returns null if wrong type or deprecated)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
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
