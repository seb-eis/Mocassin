using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// State exchange group that defines a set of possible state changes for a position during a transition
    /// </summary>
    [DataContract]
    public class StateExchangeGroup : ModelObject, IStateExchangeGroup
    {
        /// <summary>
        /// The state exchange pairs affiliated with this state exchange group group
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IStateExchangePair> StateExchangePairs { get; set; }

        /// <summary>
        /// Flag if the property group is a vacancy group
        /// </summary>
        [DataMember]
        public bool VacancyGroup { get; set; }

        /// <summary>
        /// Get the number of property state pairs in the group
        /// </summary>
        [IgnoreDataMember]
        public int StatePairCount => StateExchangePairs.Count;

        /// <summary>
        /// Get the state exchange pairs of this state exchange group
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IStateExchangePair> GetStateExchangePairs()
        {
            return (StateExchangePairs ?? new List<IStateExchangePair>()).AsEnumerable();
        }

        /// <summary>
        /// Get the type name of the model object
        /// </summary>
        /// <returns></returns>
        public override string GetObjectName()
        {
            return "'Property Group'";
        }

        /// <summary>
        /// Tries to consume a model object interface and return a new model object of this type (Returns null if wrong type or deprecated)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastIfNotDeprecated<IStateExchangeGroup>(obj) is var group)
            {
                Index = group.Index;
                VacancyGroup = group.VacancyGroup;
                StateExchangePairs = group.GetStateExchangePairs().ToList();
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks for equality of the model information by comparing if either of the pair index lists contains the complete other list
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IStateExchangeGroup other)
        {
            bool secondContainsFirst = true;
            bool firstContainsSecond = true;
            foreach (var index in other.GetStateExchangePairs().Select(a => a.Index))
            {
                if (!StateExchangePairs.Select(a => a.Index).Contains(index))
                {
                    firstContainsSecond = false;
                    break;
                }
            }
            foreach (var index in StateExchangePairs.Select(a => a.Index))
            {
                if (!other.GetStateExchangePairs().Select(a => a.Index).Contains(index))
                {
                    secondContainsFirst = false;
                    break;
                }
            }
            return firstContainsSecond || secondContainsFirst;
        }
    }
}
