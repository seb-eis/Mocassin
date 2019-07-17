﻿using System.Collections.Generic;
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
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="MmcJobPackageControlView" /> that controls
    ///     manipulation of <see cref="MmcJobDescriptionGraph" /> instances
    /// </summary>
    public class MmcJobPackageControlViewModel : CollectionControlViewModel<MmcJobPackageDescriptionGraph>,
        IContentSupplier<ProjectJobTranslationGraph>
    {
        private int duplicateCount;
        private ProjectJobTranslationGraph contentSource;
        private IEnumerable<ModelObjectReferenceGraph<MetropolisSimulation>> selectableSimulations;

        /// <inheritdoc />
        public ProjectJobTranslationGraph ContentSource
        {
            get => contentSource;
            set => SetProperty(ref contentSource, value);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReferenceGraph{T}" /> instances that describe the
        ///     target <see cref="MetropolisSimulation" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<MetropolisSimulation>> SelectableSimulations
        {
            get => selectableSimulations;
            set => SetProperty(ref selectableSimulations, value);
        }

        /// <summary>
        ///     Get the <see cref="DuplicateCollectionItemCommand{T}"/> for the collection
        /// </summary>
        public DuplicateCollectionItemCommand<MmcJobPackageDescriptionGraph> DuplicateItemCommand { get; }

        /// <summary>
        ///     Get or set the duplicate count if the duplicate command is executed
        /// </summary>
        public int DuplicateCount
        {
            get => duplicateCount;
            set => SetProperty(ref duplicateCount, value > 0 ? value : 1);
        }

        /// <summary>
        ///     Creates a new <see cref="MmcJobPackageControlViewModel"/>
        /// </summary>
        public MmcJobPackageControlViewModel()
        {
            duplicateCount = 1;
            DuplicateItemCommand = new DuplicateCollectionItemCommand<MmcJobPackageDescriptionGraph>(this)
            {
                CountProvider = () => DuplicateCount
            };
        }

        /// <inheritdoc />
        public void ChangeContentSource(ProjectJobTranslationGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(contentSource?.MmcJobPackageDescriptions);
            SelectableSimulations = GetSelectableSimulations(contentSource);
        }

        /// <summary>
        ///     Get the sequence selectable <see cref="ModelObjectReferenceGraph{T}" /> of <see cref="KineticSimulation" />
        ///     instances that are available for the passed <see cref="ProjectJobTranslationGraph" />
        /// </summary>
        /// <param name="jobTranslation"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReferenceGraph<MetropolisSimulation>> GetSelectableSimulations(
            ProjectJobTranslationGraph jobTranslation)
        {
            return jobTranslation?.Parent?.ProjectModelGraph?.SimulationModelGraph?.MetropolisSimulations
                ?.Select(x => new ModelObjectReferenceGraph<MetropolisSimulation>(x));
        }
    }
}