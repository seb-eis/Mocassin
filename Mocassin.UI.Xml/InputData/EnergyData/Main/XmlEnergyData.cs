﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.EnergyData
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Energies.IEnergyManager" />
    ///     system
    /// </summary>
    [XmlRoot("EnergyModel")]
    public class XmlEnergyData : XmlProjectManagerData
    {
        /// <summary>
        ///     Get or set the stable environment info of the energy model
        /// </summary>
        [XmlElement("StableEnvironmentInfo")]
        public XmlStableEnvironmentInfo StableEnvironmentInfo { get; set; }

        /// <summary>
        ///     Get or set the list of unstable environments of the energy model
        /// </summary>
        [XmlArray("UnstableEnvironments")]
        [XmlArrayItem("UnstableEnvironment")]
        public List<XmlUnstableEnvironment> UnstableEnvironments { get; set; }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield return StableEnvironmentInfo.GetInputObject();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return UnstableEnvironments.Select(x => x.GetInputObject());
        }
    }
}