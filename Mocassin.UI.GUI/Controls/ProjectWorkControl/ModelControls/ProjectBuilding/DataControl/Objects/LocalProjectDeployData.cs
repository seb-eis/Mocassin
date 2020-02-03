using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding.DataControl.Objects
{
    /// <summary>
    ///     Contains required information for a local deploy of a <see cref="SimulationDbBuildTemplate" /> into a simulation
    ///     database
    /// </summary>
    public class LocalProjectDeployData
    {
        /// <summary>
        ///     Get or set the deploy file path <see cref="string" />
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        ///     Gte or set the <see cref="SimulationDbBuildTemplate" /> that describes the build process
        /// </summary>
        public SimulationDbBuildTemplate BuildInstruction { get; set; }
    }
}