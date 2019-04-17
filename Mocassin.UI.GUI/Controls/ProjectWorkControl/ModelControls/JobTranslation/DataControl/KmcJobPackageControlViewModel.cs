using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Simulations;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="KmcJobPackageControlView" /> that controls
    ///     manipulation of <see cref="KmcJobDescriptionGraph" /> instances
    /// </summary>
    public class KmcJobPackageControlViewModel : CollectionControlViewModel<KmcJobPackageDescriptionGraph>,
        IContentSupplier<ProjectJobTranslationGraph>
    {
        private IEnumerable<ModelObjectReferenceGraph<KineticSimulation>> selectableSimulations;

        /// <inheritdoc />
        public ProjectJobTranslationGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReferenceGraph{T}" /> instances that describe the
        ///     target <see cref="KineticSimulation" />
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<KineticSimulation>> SelectableSimulations
        {
            get => selectableSimulations;
            set => SetProperty(ref selectableSimulations, value);
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is ProjectJobTranslationGraph jobTranslation) ChangeContentSource(jobTranslation);
        }

        /// <inheritdoc />
        public void ChangeContentSource(ProjectJobTranslationGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(contentSource?.KmcJobPackageDescriptions);
            SelectableSimulations = GetSelectableSimulations(contentSource);
        }

        /// <summary>
        ///     Get the sequence selectable <see cref="ModelObjectReferenceGraph{T}"/> of <see cref="KineticSimulation"/> instances that are available for the passed <see cref="ProjectJobTranslationGraph"/>
        /// </summary>
        /// <param name="jobTranslation"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReferenceGraph<KineticSimulation>> GetSelectableSimulations(ProjectJobTranslationGraph jobTranslation)
        {
            return jobTranslation?.Parent?.ProjectModelGraph?.SimulationModelGraph?.KineticSimulations
                ?.Select(x => new ModelObjectReferenceGraph<KineticSimulation>(x));
        }
    }
}