using System.Collections.Generic;
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
    public class XmlSimulationData : XmlProjectManagerData
    {
        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            throw new System.NotImplementedException();
        }
    }
}