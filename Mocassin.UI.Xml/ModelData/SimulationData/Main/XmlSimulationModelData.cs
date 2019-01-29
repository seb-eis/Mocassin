using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.BaseData;
using Mocassin.UI.Xml.ProjectData;

namespace Mocassin.UI.Xml.SimulationData
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Simulations.ISimulationManager" />
    ///     system
    /// </summary>
    [XmlRoot("SimulationModel")]
    public class XmlSimulationModelData : XmlProjectManagerModelData
    {
        /// <summary>
        /// Get or set the list of metropolis simulations
        /// </summary>
        [XmlArray("MetropolisSimulations")]
        [XmlArrayItem("MetropolisSimulation")]
        public List<XmlMetropolisSimulation> MetropolisSimulations { get; set; }

        /// <summary>
        /// Get or set the list of metropolis simulations
        /// </summary>
        [XmlArray("KineticSimulations")]
        [XmlArrayItem("KineticSimulation")]
        public List<XmlKineticSimulation> KineticSimulations { get; set; }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield break;
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return MetropolisSimulations.Select(x => x.GetInputObject())
                .Concat(KineticSimulations.Select(x => x.GetInputObject()));
        }
    }
}