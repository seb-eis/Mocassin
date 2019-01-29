using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Energies;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="Mocassin.Model.Energies.IEnergyManager" />
    ///     interaction settings through the <see cref="IEnergySetterProvider"/> system
    /// </summary>
    [XmlRoot("EnergyModelParametrization")]
    public class XmlEnergyParameterSet
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

        /// <summary>
        ///     Creates a new <see cref="XmlEnergyParameterSet" /> by transforming all defined data in the passed
        ///     <see cref="IEnergySetterProvider" /> into serializable data formats
        /// </summary>
        /// <param name="energySetterProvider"></param>
        /// <returns></returns>
        public static XmlEnergyParameterSet Create(IEnergySetterProvider energySetterProvider)
        {
            if (energySetterProvider == null)
                throw new ArgumentNullException(nameof(energySetterProvider));

            var obj = new XmlEnergyParameterSet
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

            obj.StablePairEnergyParameterSets.Sort();
            obj.UnstablePairEnergyParameterSets.Sort();
            obj.GroupEnergyParameterSets.Sort();

            return obj;
        }
    }
}