using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;
using Mocassin.Symmetry.CrystalSystems;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Represents a set of cell parameters (a,b,c) and angles (alpha, beta, gamma)
    /// </summary>
    public interface ICellParameters : IModelParameter
    {
        /// <summary>
        /// The parameter in A direction
        /// </summary>
        double ParamA { get; }

        /// <summary>
        /// The parameter in B direction
        /// </summary>
        double ParamB { get; }

        /// <summary>
        /// The parameter in C direction
        /// </summary>
        double ParamC { get; }

        /// <summary>
        /// The angle alpha in radian
        /// </summary>
        double Alpha { get; }

        /// <summary>
        /// The angle alpha in degree
        /// </summary>
        double AlphaDegree { get; }

        /// <summary>
        /// The angle beta in radian
        /// </summary>
        double Beta { get; }

        /// <summary>
        /// The angle beta in degree
        /// </summary>
        double BetaDegree { get; }

        /// <summary>
        /// The angle gamma in radian
        /// </summary>
        double Gamma { get; }

        /// <summary>
        /// The angle gamma in degree
        /// </summary>
        double GammaDegree { get; }

        /// <summary>
        /// The the cell parameters as a parameter set
        /// </summary>
        /// <returns></returns>
        CrystalParameterSet AsParameterSet();
    }
}
