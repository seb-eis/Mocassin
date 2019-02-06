﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="Mocassin.Model.Energies.IEnergyManager" />
    ///     interaction settings through the <see cref="IEnergySetterProvider" /> system
    /// </summary>
    [XmlRoot("EnergyModelCustomization")]
    public class XmlEnergyModelCustomization : XmlModelCustomizationData
    {
        /// <summary>
        ///     Get or set the list of <see cref="Mocassin.Model.Energies.ISymmetricPairInteraction" /> customization data sets
        /// </summary>
        [XmlArray("StablePairEnergySets")]
        [XmlArrayItem("PairEnergySet")]
        public List<XmlPairEnergyParameterSet> StablePairEnergyParameterSets { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="Mocassin.Model.Energies.IAsymmetricPairInteraction" /> customization data sets
        /// </summary>
        [XmlArray("UnstablePairEnergySets")]
        [XmlArrayItem("PairEnergySet")]
        public List<XmlPairEnergyParameterSet> UnstablePairEnergyParameterSets { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="Mocassin.Model.Energies.IGroupInteraction" /> customization data sets
        /// </summary>
        [XmlArray("GroupEnergySets")]
        [XmlArrayItem("GroupEnergySet")]
        public List<XmlGroupEnergyParameterSet> GroupEnergyParameterSets { get; set; }

        /// <inheritdoc />
        public override void PushToModel(IModelProject modelProject)
        {
            var setterProvider = modelProject.GetManager<IEnergyManager>().QueryPort.Query(x => x.GetEnergySetterProvider());

            foreach (var parameterSet in StablePairEnergyParameterSets)
            {
                var setter = setterProvider.GetStablePairEnergySetter(parameterSet.PairInteractionIndex);
                parameterSet.PushToModel(modelProject, setter);
            }

            foreach (var parameterSet in UnstablePairEnergyParameterSets)
            {
                var setter = setterProvider.GetUnstablePairEnergySetter(parameterSet.PairInteractionIndex);
                parameterSet.PushToModel(modelProject, setter);
            }

            foreach (var parameterSet in GroupEnergyParameterSets)
            {
                var setter = setterProvider.GetGroupEnergySetter(parameterSet.GroupInteractionIndex);
                parameterSet.PushToModel(modelProject, setter);
            }
        }

        /// <summary>
        ///     Sort all contained lists of data in a user friendly order
        /// </summary>
        public void OrderDataForUser()
        {
            StablePairEnergyParameterSets.Sort();
            UnstablePairEnergyParameterSets.Sort();
            GroupEnergyParameterSets.Sort();
        }

        /// <summary>
        ///     Sort all contained lists of data in a system friendly order
        /// </summary>
        public void OrderDataForSystem()
        {
            StablePairEnergyParameterSets.Sort((lhs, rhs) => lhs.PairInteractionIndex.CompareTo(rhs.PairInteractionIndex));
            UnstablePairEnergyParameterSets.Sort((lhs, rhs) => lhs.PairInteractionIndex.CompareTo(rhs.PairInteractionIndex));
            GroupEnergyParameterSets.Sort((lhs, rhs) => lhs.GroupInteractionIndex.CompareTo(rhs.GroupInteractionIndex));
        }

        /// <summary>
        ///     Creates a new <see cref="XmlEnergyModelCustomization" /> by transforming all defined data in the passed
        ///     <see cref="IEnergySetterProvider" /> into serializable data formats
        /// </summary>
        /// <param name="energySetterProvider"></param>
        /// <returns></returns>
        public static XmlEnergyModelCustomization Create(IEnergySetterProvider energySetterProvider)
        {
            if (energySetterProvider == null) throw new ArgumentNullException(nameof(energySetterProvider));

            var obj = new XmlEnergyModelCustomization
            {
                StablePairEnergyParameterSets = energySetterProvider
                    .GetStablePairEnergySetters()
                    .Select(XmlPairEnergyParameterSet.Create)
                    .ToList(),

                UnstablePairEnergyParameterSets = energySetterProvider
                    .GetUnstablePairEnergySetters()
                    .Select(XmlPairEnergyParameterSet.Create)
                    .ToList(),

                GroupEnergyParameterSets = energySetterProvider
                    .GetGroupEnergySetters()
                    .Select(XmlGroupEnergyParameterSet.Create)
                    .ToList()
            };

            obj.OrderDataForUser();

            return obj;
        }
    }
}