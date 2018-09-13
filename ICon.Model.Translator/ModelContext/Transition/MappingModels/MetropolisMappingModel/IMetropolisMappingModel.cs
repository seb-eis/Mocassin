using ICon.Mathematics.ValueTypes;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a metropolis mapping model that fully describes a transitions geometric properties on a specific position
    /// </summary>
    public interface IMetropolisMappingModel
    {
        /// <summary>
        /// The metropolis mapping object the model is based upon
        /// </summary>
        MetropolisMapping Mapping { get; set; }

        /// <summary>
        /// The inverse mapping model that describes the neutralizing transition
        /// </summary>
        IMetropolisMappingModel InverseMapping { get; set; }

        /// <summary>
        /// The fractional 3D coordinates of the start point
        /// </summary>
        Fractional3D StartVector3D { get; set; }

        /// <summary>
        /// The encoded 4D coordinates of the start point (0,0,0,P)
        /// </summary>
        CrystalVector4D StartVector4D { get; set; }
    }
}
