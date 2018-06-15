using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Property group to define sets of exchangeable properties for transitions 
    /// </summary>
    [DataContract(Name ="PropertyGroup")]
    public class PropertyGroup : ModelObject, IPropertyGroup
    {
        /// <summary>
        /// The property state pair indices affiliated with this property group
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public List<IPropertyStatePair> PropertyStatePairs { get; set; }

        /// <summary>
        /// Flag if the property group is a vacancy group
        /// </summary>
        [DataMember]
        public bool VacancyGroup { get; set; }

        /// <summary>
        /// The relative charge value from the donor to acceptor state
        /// </summary>
        [DataMember]
        public double ChargeTransfer { get; set; }

        /// <summary>
        /// Get the number of property state pairs in the group
        /// </summary>
        [IgnoreDataMember]
        public int StatePairCount => PropertyStatePairs.Count;

        /// <summary>
        /// Get the property state pair indices as an enumerable
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPropertyStatePair> GetPropertyStatePairs()
        {
            return PropertyStatePairs.AsEnumerable();
        }

        /// <summary>
        /// Get the type name of the model object
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Property Group'";
        }

        /// <summary>
        /// Tries to consume a model object interface and return a new model object of this type (Returns null if wrong type or deprecated)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IPropertyGroup>(obj) is var group)
            {
                Index = group.Index;
                VacancyGroup = group.VacancyGroup;
                ChargeTransfer = group.ChargeTransfer;
                PropertyStatePairs = group.GetPropertyStatePairs().ToList();
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks for equality of the model information by comparing if either of the pair index lists contains the complete other list
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IPropertyGroup other)
        {
            bool secondContainsFirst = true;
            bool firstContainsSecond = true;
            foreach (var index in other.GetPropertyStatePairs().Select(a => a.Index))
            {
                if (!PropertyStatePairs.Select(a => a.Index).Contains(index))
                {
                    firstContainsSecond = false;
                    break;
                }
            }
            foreach (var index in PropertyStatePairs.Select(a => a.Index))
            {
                if (!other.GetPropertyStatePairs().Select(a => a.Index).Contains(index))
                {
                    secondContainsFirst = false;
                    break;
                }
            }
            return firstContainsSecond || secondContainsFirst;
        }
    }
}
