using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ICon.Model.Basic;

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
    }
}
