﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Processing;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.Xml.LatticeModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Lattices.ILatticeManager" />
    ///     system
    /// </summary>
    [XmlRoot("LatticeModel")]
    public class LatticeModelGraph : ModelManagerGraph
    {
	    /// <summary>
	    ///     The list of defines building blocks
	    /// </summary>
	    [XmlArray("BuildingBlock")]
	    [XmlArrayItem("BuildingBlock")]
        public List<BuildingBlockGraph> BuildingBlocks { get; set; }

	    /// <summary>
	    ///     The list of defines dopings
	    /// </summary>
	    [XmlArray("Doping")]
	    [XmlArrayItem("Doping")]
		public List<DopingGraph> Dopings { get; set; }
		
	    /// <summary>
	    ///     The list of defines doping combinations
	    /// </summary>
	    [XmlArray("DopingCombination")]
	    [XmlArrayItem("DopingCombination")]
	    public List<DopingCombinationGraph> DopingCombination { get; set; }

        /// <summary>
        ///     Creates new <see cref="EnergyModelGraph"/> with empty component lists
        /// </summary>
        public LatticeModelGraph()
        {
	        BuildingBlocks = new List<BuildingBlockGraph>();
	        Dopings = new List<DopingGraph>();
			DopingCombination = new List<DopingCombinationGraph>();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
	        yield break;
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
	        yield break;
        }
    }
}