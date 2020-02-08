using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Simulations;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="KmcJobPackageControlView" /> that controls
    ///     manipulation of <see cref="KmcJobConfigData" /> instances
    /// </summary>
    public class KmcJobPackageControlViewModel : CollectionControlViewModel<KmcJobPackageData>,
        IContentSupplier<ProjectJobSetTemplate>
    {
        private ProjectJobSetTemplate contentSource;
        private int duplicateCount = 1;
        private IEnumerable<ModelObjectReference<KineticSimulation>> selectableSimulations;

        /// <inheritdoc />
        public ProjectJobSetTemplate ContentSource
        {
            get => contentSource;
            set => SetProperty(ref contentSource, value);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReference{T}" /> instances that describe the
        ///     target <see cref="KineticSimulation" />
        /// </summary>
        public IEnumerable<ModelObjectReference<KineticSimulation>> SelectableSimulations
        {
            get => selectableSimulations;
            set => SetProperty(ref selectableSimulations, value);
        }

        /// <summary>
        ///     Get the <see cref="DuplicateCollectionItemCommand{T}" /> for the collection
        /// </summary>
        public DuplicateCollectionItemCommand<KmcJobPackageData> DuplicateItemCommand { get; }

        /// <summary>
        ///     Get or set the duplicate count if the duplicate command is executed
        /// </summary>
        public int DuplicateCount
        {
            get => duplicateCount;
            set => SetProperty(ref duplicateCount, value > 0 ? value : 1);
        }

        /// <summary>
        ///     Creates a new <see cref="KmcJobPackageControlViewModel" />
        /// </summary>
        public KmcJobPackageControlViewModel()
        {
            DuplicateItemCommand = new DuplicateCollectionItemCommand<KmcJobPackageData>(this)
            {
                CountProvider = () => DuplicateCount
            };
        }

        /// <inheritdoc />
        public void ChangeContentSource(ProjectJobSetTemplate jobSetTemplate)
        {
            ContentSource = jobSetTemplate;
            SetCollection(jobSetTemplate?.KmcJobPackageDescriptions);
            SelectableSimulations = GetSelectableSimulations(jobSetTemplate);
        }

        /// <summary>
        ///     Get the sequence selectable <see cref="ModelObjectReference{T}" /> of <see cref="KineticSimulation" /> instances
        ///     that are available for the passed <see cref="ProjectJobSetTemplate" />
        /// </summary>
        /// <param name="jobTranslation"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<KineticSimulation>> GetSelectableSimulations(ProjectJobSetTemplate jobTranslation)
        {
            return jobTranslation?.Parent?.ProjectModelData?.SimulationModelData?.KineticSimulations
                ?.Select(x => new ModelObjectReference<KineticSimulation>(x));
        }
    }
}