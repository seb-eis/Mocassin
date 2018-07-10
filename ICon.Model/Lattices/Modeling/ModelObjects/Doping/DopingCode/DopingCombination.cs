using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.Structures;

namespace ICon.Model.Lattices
{

    /// <summary>
    /// Contains the information about the particles and unit cell positions which are involved in the doping process
    /// </summary>
    [DataContract(Name = "Doping")]
    public class DopingCombination : ModelObject, IDopingCombination
    {
        /// <summary>
        /// Dopand particle
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IParticle Dopant { set; get; }

        /// <summary>
        /// Particle that is doped
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IParticle DopedParticle { set; get; }

        /// <summary>
        /// unit cell position (contains information about the sublattice)
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IUnitCellPosition UnitCellPosition { set; get; }

        /// <summary>
        /// Building Block in which the doping should take place
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public IBuildingBlock BuildingBlock { get; set; }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'DopingCombination'";
        }

         /// <summary>
        /// Copies the information from the provided parameter interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject modelObject)
        {
            if (CastWithDepricatedCheck<IDopingCombination>(modelObject) is var casted)
            {
                Dopant = casted.Dopant;
                DopedParticle = casted.DopedParticle;
                UnitCellPosition = casted.UnitCellPosition;
                BuildingBlock = casted.BuildingBlock;
                return this;
            }
            return null;
        }
    }
}
