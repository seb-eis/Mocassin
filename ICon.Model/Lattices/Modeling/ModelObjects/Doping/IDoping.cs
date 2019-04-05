using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Doping information that describes the element, concentration, sublattice which is substituted. 
    /// May also contain information about counter doping which is described in the same manner.
    /// </summary>
    public interface IDoping : IModelObject
    {
        /// <summary>
        /// Information about the doping (particles and sublattice)
        /// </summary>
        IDopingCombination PrimaryDoping { get; }

        /// <summary>
        /// Information about the counter doping (particles and sublattice)
        /// </summary>
        IDopingCombination CounterDoping { get; }

	    /// <summary>
	    /// Building Block in which the doping should take place
	    /// </summary>
	    IBuildingBlock BuildingBlock { get; set; }

        /// <summary>
        /// Doping group ID for simultaneous doping
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Flag to indicate whether a counter doping should be applied
        /// </summary>
        bool UseCounterDoping { get; }
    }
}
