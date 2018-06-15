using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Basic position dummy that is used only for visual purposes in the model and is not actually part of the extended position list
    /// </summary>
    [DataContract(Name ="DummyPosition")]
    public class PositionDummy : ModelObject, IPositionDummy
    {
        /// <summary>
        /// The fractional position 3D vector data
        /// </summary>
        [DataMember]
        public DataVector3D Vector { get; set; }

        /// <summary>
        /// Interface access to the fractional position vector as a struct
        /// </summary>
        [IgnoreDataMember]
        Fractional3D IPositionDummy.Vector => Vector.AsFractional();

        /// <summary>
        /// Get the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Dummy Position'";
        }

        /// <summary>
        /// Copies values in the consumed interface and returns the object as a model object (Returns null if the consume failed due to wrong interface type)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IPositionDummy>(obj) is var dummy)
            {
                Vector = new DataVector3D(dummy.Vector);
                return this;
            }
            return null;
        }
    }
}
