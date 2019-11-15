﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.EnergyModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Energies.IEnergyManager" />
    ///     system
    /// </summary>
    [XmlRoot("EnergyModel")]
    public class EnergyModelGraph : ModelManagerGraph
    {
        private StableEnvironmentGraph stableEnvironment;
        private ObservableCollection<UnstableEnvironmentGraph> unstableEnvironments;
        private ObservableCollection<GroupInteractionGraph> groupInteractions;

        /// <summary>
        ///     Get or set the stable environment info of the energy model
        /// </summary>
        [XmlElement("StableEnvironmentSetting")]
        public StableEnvironmentGraph StableEnvironment
        {
            get => stableEnvironment;
            set => SetProperty(ref stableEnvironment, value);
        }

        /// <summary>
        ///     Get or set the list of unstable environments of the energy model
        /// </summary>
        [XmlArray("UnstableEnvironments")]
        [XmlArrayItem("UnstableEnvironment")]
        public ObservableCollection<UnstableEnvironmentGraph> UnstableEnvironments
        {
            get => unstableEnvironments;
            set => SetProperty(ref unstableEnvironments, value);
        }

        /// <summary>
        ///     Get or set the list of unstable environments of the energy model
        /// </summary>
        [XmlArray("GroupInteractions")]
        [XmlArrayItem("GroupInteraction")]
        public ObservableCollection<GroupInteractionGraph> GroupInteractions
        {
            get => groupInteractions;
            set => SetProperty(ref groupInteractions, value);
        }

        /// <summary>
        ///     Creates new <see cref="EnergyModelGraph" /> with empty component lists
        /// </summary>
        public EnergyModelGraph()
        {
            GroupInteractions = new ObservableCollection<GroupInteractionGraph>();
            UnstableEnvironments = new ObservableCollection<UnstableEnvironmentGraph>();
            StableEnvironment = new StableEnvironmentGraph();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield return StableEnvironment.GetInputObject();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return UnstableEnvironments.Select(x => x.GetInputObject())
                .Concat(GroupInteractions.Select(x => x.GetInputObject()));
        }
    }
}