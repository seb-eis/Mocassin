﻿using System;
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
        /// Specifies the doping concentration
        /// </summary>
        double Concentration { get; }

        /// <summary>
        /// Information about the doping (particles and sublattice)
        /// </summary>
        IDopingCombination DopingInfo { get; }

        /// <summary>
        /// Information about the counter doping (particles and sublattice)
        /// </summary>
        IDopingCombination CounterDopingInfo { get; }
    }
}
