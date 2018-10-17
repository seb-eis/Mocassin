﻿using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a metropolis mapping model that fully describes a transitions geometric properties on a specific position
    /// </summary>
    public interface IMetropolisMappingModel
    {
        /// <summary>
        /// Boolean flag if the inverse mapping model is set
        /// </summary>
        bool InverseIsSet { get; }

        /// <summary>
        /// Boolean flag that indicates if this mapping model describes an inversion of its source mapping
        /// </summary>
        bool IsSourceInversion { get; set; }

        /// <summary>
        /// The metropolis mapping object the model is based upon
        /// </summary>
        MetropolisMapping Mapping { get; set; }

        /// <summary>
        /// The inverse mapping model that describes the neutralizing transition
        /// </summary>
        IMetropolisMappingModel InverseMapping { get; set; }

        /// <summary>
        /// The fractional 3D coordinates of the start point in the origin unit cell
        /// </summary>
        Fractional3D StartVector3D { get; set; }

        /// <summary>
        /// The fractional 3D coordinates of the end point in the origin unit cell
        /// </summary>
        Fractional3D EndVector3D { get; set; }

        /// <summary>
        /// The encoded 4D coordinates of the start point (0,0,0,P) in the origin unit cell
        /// </summary>
        CrystalVector4D StartVector4D { get; set; }

        /// <summary>
        /// The encoded 4D coordinates of the end position (0,0,0,P) in the origin unit cell
        /// </summary>
        CrystalVector4D EndVector4D { get; set; }

        /// <summary>
        /// Create a new inverted version of this metropolis mapping model
        /// </summary>
        /// <returns></returns>
        IMetropolisMappingModel CreateInverse();

        /// <summary>
        /// Links this and the passed mapping model together if they describe inverse mappings. Returns false if they do not match
        /// </summary>
        /// <param name="mappingModel"></param>
        /// <returns></returns>
        bool LinkIfInverseMatch(IMetropolisMappingModel mappingModel);
    }
}
