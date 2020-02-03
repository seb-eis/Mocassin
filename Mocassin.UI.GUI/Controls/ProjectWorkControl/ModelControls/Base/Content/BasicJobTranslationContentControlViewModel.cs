using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content
{
    /// <summary>
    ///     Basic customization content control that extends <see cref="BasicModelContentControlViewModel" /> with a
    ///     <see cref="ProjectJobSetTemplate" /> selection
    /// </summary>
    public class BasicJobTranslationContentControlViewModel : BasicModelContentControlViewModel
    {
        private ObservableCollection<ProjectJobSetTemplate> jobTranslationGraphs;
        private ProjectJobSetTemplate selectedJobSetTemplate;

        /// <summary>
        ///     Get or set the currently selected <see cref="ProjectJobSetTemplate" />
        /// </summary>
        public ProjectJobSetTemplate SelectedJobSetTemplate
        {
            get => selectedJobSetTemplate;
            set => SetProperty(ref selectedJobSetTemplate, value, () => OnSelectionChanged(value));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="ProjectJobSetTemplate" /> instances that can currently be
        ///     selected
        /// </summary>
        public ObservableCollection<ProjectJobSetTemplate> JobTranslationGraphs
        {
            get => jobTranslationGraphs;
            protected set => SetProperty(ref jobTranslationGraphs, value);
        }

        /// <summary>
        ///     Get the <see cref="AddNewJobTranslationCommand" /> to create a new <see cref="ProjectJobSetTemplate" /> on the
        ///     current project
        /// </summary>
        public AddNewJobTranslationCommand AddJobTranslationCommand { get; }

        /// <summary>
        ///     Get the <see cref="DeleteJobTranslationCommand"/> to remove a <see cref="ProjectJobSetTemplate"/> from the current project
        /// </summary>
        public DeleteJobTranslationCommand DeleteTranslationCommand { get; }

        /// <summary>
        ///     Get the <see cref="DuplicateJobTranslationCommand"/> to add a <see cref="ProjectJobSetTemplate"/> duplicate to the current project
        /// </summary>
        public DuplicateJobTranslationCommand DuplicateTranslationCommand { get; }

        /// <inheritdoc />
        public BasicJobTranslationContentControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            AddJobTranslationCommand = new AddNewJobTranslationCommand(projectControl, () => SelectedProject, x => ReloadSelectionSource(false, true));
            DeleteTranslationCommand = new DeleteJobTranslationCommand(projectControl, () => ReloadSelectionSource(true, true));
            DuplicateTranslationCommand = new DuplicateJobTranslationCommand(projectControl, x => ReloadSelectionSource(false, true));
            PropertyChanged += OnTranslationSourceChanged;
        }

        /// <summary>
        ///     Notifies property change of <see cref="JobTranslationGraphs" /> if the selected project property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTranslationSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedProject)) return;
            ReloadSelectionSource();
        }

        /// <inheritdoc />
        protected override void OnProjectContentChangedInternal()
        {
            JobTranslationGraphs = SelectedProject?.JobSetTemplates;
        }

        /// <summary>
        ///     Nulls the currently selected <see cref="ProjectJobSetTemplate"/> and reloads the option list
        /// </summary>
        protected void ReloadSelectionSource(bool nullSelected = true, bool selectLast = false)
        {
            ExecuteOnAppThread(() =>
            {
                if (nullSelected) SelectedJobSetTemplate = null;
                JobTranslationGraphs = SelectedProject?.JobSetTemplates;
                if (selectLast) SelectedJobSetTemplate = JobTranslationGraphs?.LastOrDefault();
            });
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            PropertyChanged -= OnTranslationSourceChanged;
            base.Dispose();
        }
    }
}