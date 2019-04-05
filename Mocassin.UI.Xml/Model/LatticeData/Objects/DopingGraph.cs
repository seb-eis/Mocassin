using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.LatticeModel
{
	/// <summary>
	///     Serializable data object for <see cref="Mocassin.Model.Lattices.IDoping" /> model object creation
	/// </summary>
	[XmlRoot("Doping")]
	public class DopingGraph : ModelObjectGraph
	{
		/// <summary>
		/// The doping which is applied
		/// </summary>
		[XmlAttribute("PrimaryDoping")]
		[JsonProperty("PrimaryDoping")]
		public ModelObjectReferenceGraph<DopingCombination> PrimaryDoping { get; set; }

		/// <summary>
		/// The doping to compensate the primary doping
		/// </summary>
		[XmlAttribute("CounterDoping")]
		[JsonProperty("CounterDoping")]
		public ModelObjectReferenceGraph<DopingCombination> CounterDoping { get; set; }

		/// <summary>
		/// The building block in which the doping is used
		/// </summary>
		[XmlAttribute("BuildingBlock")]
		[JsonProperty("BuildingBlock")]
		public ModelObjectReferenceGraph<BuildingBlock> BuildingBlock { get; set; }

		/// <summary>
		/// Flag to indicate if a counter doping should be used
		/// </summary>
		[XmlAttribute("BuildingBlock")]
		[JsonProperty("BuildingBlock")]
		public bool UseCounterDoping { get; set; }

		/// <summary>
		/// The priority oder in which the doping is applied
		/// </summary>
		[XmlAttribute("Priority")]
		[JsonProperty("Priority")]
		public int Priority { get; set; }

		/// <inheritdoc />
		protected override ModelObject GetModelObjectInternal()
		{
			var obj = new Doping()
			{
				CounterDoping = new DopingCombination(){Key = CounterDoping.Key},
				PrimaryDoping = new DopingCombination(){Key = PrimaryDoping.Key},
				BuildingBlock = new BuildingBlock(){Key = BuildingBlock.Key},
				UseCounterDoping = UseCounterDoping,
				Priority = Priority
			};
			return obj;
		}

	}
}