using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content
{
    /// <summary>
    ///     Basic customization content control that extends <see cref="BasicModelContentControlViewModel" /> with a
    ///     <see cref="ProjectJobTranslationGraph" /> selection
    /// </summary>
    public class BasicJobTranslationContentControlViewModel : BasicModelContentControlViewModel
    {
        private IEnumerable<ProjectJobTranslationGraph> jobTranslationGraphs;
        private ProjectJobTranslationGraph selectedJobTranslationGraph;

        /// <summary>
        ///     Get or set the currently selected <see cref="ProjectJobTranslationGraph" />
        /// </summary>
        public ProjectJobTranslationGraph SelectedJobTranslationGraph
        {
            get => selectedJobTranslationGraph;
            set
            {
                SetProperty(ref selectedJobTranslationGraph, value);
                OnSelectionChanged(value);
            }
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="ProjectJobTranslationGraph" /> instances that can currently be
        ///     selected
        /// </summary>
        public IEnumerable<ProjectJobTranslationGraph> JobTranslationGraphs
        {
            get => jobTranslationGraphs;
            set => SetProperty(ref jobTranslationGraphs, value);
        }

        /// <summary>
        ///     Get the <see cref="AddNewJobTranslationCommand" /> to create a new <see cref="ProjectJobTranslationGraph" /> on the
        ///     current project
        /// </summary>
        public AddNewJobTranslationCommand AddJobTranslationCommand { get; }

        /// <summary>
        ///     Get the <see cref="DeleteJobTranslationCommand"/> to remove a <see cref="ProjectJobTranslationGraph"/> from the current project
        /// </summary>
        public DeleteJobTranslationCommand DeleteTranslationCommand { get; }

        /// <inheritdoc />
        public BasicJobTranslationContentControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            AddJobTranslationCommand = new AddNewJobTranslationCommand(projectControl, () => SelectedProjectGraph);
            DeleteTranslationCommand = new DeleteJobTranslationCommand(projectControl, () => SelectedProjectGraph, ReloadSelectionSource);
            PropertyChanged += OnTranslationSourceChanged;
        }

        /// <summary>
        ///     Notifies property change of <see cref="JobTranslationGraphs" /> if the selected project property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTranslationSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedProjectGraph)) return;
            ReloadSelectionSource();
        }

        /// <inheritdoc />
        protected override void OnProjectContentChangedInternal()
        {
            JobTranslationGraphs = SelectedProjectGraph?.ProjectJobTranslationGraphs.ToList();
        }

        /// <summary>
        ///     Nulls the currently selected <see cref="ProjectJobTranslationGraph"/> and reloads the option list
        /// </summary>
        protected void ReloadSelectionSource()
        {
            SelectedJobTranslationGraph = null;
            JobTranslationGraphs = SelectedProjectGraph?.ProjectJobTranslationGraphs?.ToList();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            PropertyChanged -= OnTranslationSourceChanged;
            base.Dispose();
        }
    }
}