using ICon.Mathematics.ValueTypes;
using ICon.Model.Transitions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// A metropolis mapping model that fully describes the geometric properties of a metropolis transition on a specific point
    /// </summary>
    public class MetropolisMappingModel : IMetropolisMappingModel
    {
        /// <summary>
        /// Boolean flag that indicates if this mapping model describes an inversion of its source mapping
        /// </summary>
        public bool IsSourceInversion { get; set; }

        /// <summary>
        /// The metropolis mapping object the model is based upon
        /// </summary>
        public MetropolisMapping Mapping { get; set; }

        /// <summary>
        /// The inverse mapping model that describes the neutralizing transition
        /// </summary>
        public IMetropolisMappingModel InverseMapping { get; set; }

        /// <summary>
        /// The fractional 3D coordinates of the start point in the origin unit cell
        /// </summary>
        public Fractional3D StartVector3D { get; set; }

        /// <summary>
        /// The fractional 3D coordinates of the end point in the origin unit cell
        /// </summary>
        public Fractional3D EndVector3D { get; set; }

        /// <summary>
        /// The encoded 4D coordinates of the start point (0,0,0,P) in the origin unit cell
        /// </summary>
        public CrystalVector4D StartVector4D { get; set; }

        /// <summary>
        /// The encoded 4D coordinates of the end position (0,0,0,P) in te origin unit cell
        /// </summary>
        public CrystalVector4D EndVector4D { get; set; }

        /// <summary>
        /// Boolean flag if the inverse is already set
        /// </summary>
        public bool InverseIsSet => InverseMapping != null;

        /// <summary>
        /// Create a new inverted version of this metropolis mapping model
        /// </summary>
        /// <returns></returns>
        public IMetropolisMappingModel CreateInverse()
        {
            return new MetropolisMappingModel()
            {
                IsSourceInversion = true,
                Mapping = Mapping,
                InverseMapping = this,
                StartVector3D = EndVector3D,
                EndVector3D = StartVector3D,
                StartVector4D = EndVector4D,
                EndVector4D = StartVector4D
            };
        }

        /// <summary>
        /// Links this and the passed mapping model as inverse cases if they fulfill the inversion criteria. Retuurns false if they do not match
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <returns></returns>
        public bool LinkIfInverseMatch(IMetropolisMappingModel mappingModel)
        {
            if (Mapping.Transition == mappingModel.Mapping.Transition && StartVector4D.Equals(mappingModel.EndVector4D))
            {
                InverseMapping = mappingModel;
                mappingModel.InverseMapping = this;
                return true;
            }
            return false;
        }
    }
}
