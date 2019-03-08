using System;
using System.Collections.Generic;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace ProjectWorkControl.SubControls.JobBuild
{
    /// <summary>
    ///     The <see cref="ViewModel" /> for <see cref="JobBuildControlView" />
    /// </summary>
    public class JobBuildControlViewModel : ViewModel
    {
        /// <summary>
        ///     The <see cref="SimulationLibrary" /> backing field
        /// </summary>
        private IMocassinSimulationLibrary simulationLibrary;

        /// <summary>
        ///     The <see cref="SelectedBuildGraph" /> backing field
        /// </summary>
        private MocassinProjectBuildGraph selectedBuildGraph;

        /// <summary>
        ///     The <see cref="CurrentJobCollections" /> backing field
        /// </summary>
        private IList<IJobCollection> currentJobCollections;

        /// <summary>
        ///     Get or set the <see cref="IMocassinProjectLibrary" /> that is currently used
        /// </summary>
        private IMocassinProjectLibrary ProjectLibrary { get; }

        /// <summary>
        ///     Get or set the <see cref="MocassinProjectBuildGraph" /> that is currently active
        /// </summary>
        public MocassinProjectBuildGraph SelectedBuildGraph
        {
            get => selectedBuildGraph;
            set => SetProperty(ref selectedBuildGraph, value);
        }

        /// <summary>
        ///     Get or set the <see cref="IMocassinSimulationLibrary" /> that is currently used
        /// </summary>
        public IMocassinSimulationLibrary SimulationLibrary
        {
            get => simulationLibrary;
            set => SetProperty(ref simulationLibrary, value);
        }

        /// <summary>
        ///     Get the list of <see cref="IJobCollection" /> that are currently translated
        /// </summary>
        public IList<IJobCollection> CurrentJobCollections
        {
            get => currentJobCollections;
            set => SetProperty(ref currentJobCollections, value);
        }

        /// <summary>
        ///     Creates new <see cref="JobBuildControlViewModel" /> for the provided <see cref="IMocassinProjectLibrary" />
        /// </summary>
        /// <param name="projectLibrary"></param>
        public JobBuildControlViewModel(IMocassinProjectLibrary projectLibrary)
        {
            ProjectLibrary = projectLibrary ?? throw new ArgumentNullException(nameof(projectLibrary));
            CurrentJobCollections = new List<IJobCollection>();
        }
    }
}