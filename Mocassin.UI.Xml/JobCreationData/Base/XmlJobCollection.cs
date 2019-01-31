using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;

namespace Mocassin.UI.Xml.JobCreationData
{
    /// <summary>
    ///     Serializable data object base for the creation of <see cref="Mocassin.Model.Translator.Jobs.IJobCollection" />
    ///     database creation instructions
    /// </summary>
    [XmlRoot("JobCollection")]
    public abstract class XmlJobCollection
    {
        /// <summary>
        ///     Get or set the key of the <see cref="Mocassin.Model.Simulations.ISimulation" /> that the collection is for
        /// </summary>
        [XmlAttribute("BaseSimulation")]
        public string SimulationKey { get; set; }

        /// <summary>
        ///     Get or set an rng seed string to overwrite the one defined in the affiliated
        ///     <see cref="Mocassin.Model.Simulations.ISimulation" />
        /// </summary>
        [XmlAttribute("RngSeed")]
        public string RngSeed { get; set; }

        /// <summary>
        ///     Creates a <see cref="IJobCollection" /> for simulation database creation in the context of the passed
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public abstract IJobCollection ToInternal(IModelProject modelProject);
    }
}