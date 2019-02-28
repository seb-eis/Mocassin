using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Main
{
    /// <summary>
    ///     The main project data root for a Mocassin project data and database creation instructions
    /// </summary>
    [XmlRoot("MocassinProject")]
    public class XmlMocassinProject
    {
        /// <summary>
        ///     Get or set the <see cref="XmlProjectModelData" /> that defines the reference information
        /// </summary>
        [XmlElement("ProjectModelData")]
        public XmlProjectModelData ProjectModelData { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="XmlProjectCustomization" /> that defines parameters for auto generated content
        /// </summary>
        [XmlArray("ModelCustomizations")]
        [XmlArrayItem("ModelCustomization")]
        public List<XmlProjectCustomization> ProjectCustomizations { get; set; }

        /// <summary>
        ///     Get or set the list of <see cref="XmlDbCreationInstruction" /> that defines
        ///     <see cref="Mocassin.Model.Translator.ISimulationDbContext" /> build instructions
        /// </summary>
        [XmlArray("DbCreationInstructions")]
        [XmlArrayItem("DbCreationInstruction")]
        public List<XmlDbCreationInstruction> DbCreationInstructions { get; set; }
    }
}