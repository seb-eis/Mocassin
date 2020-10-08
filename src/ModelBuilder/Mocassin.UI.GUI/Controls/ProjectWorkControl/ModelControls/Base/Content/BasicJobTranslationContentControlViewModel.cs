using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands;
using Mocassin.UI.Data.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content
{
    /// <summary>
    ///     Basic customization content control that extends <see cref="BasicModelContentControlViewModel" /> with a
    ///     <see cref="ProjectJobSetTemplate" /> selection
    /// </summary>
    public sealed class BasicJobTranslationContentControlViewModel : BasicModelContentControlViewModel
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
            private set => SetProperty(ref jobTranslationGraphs, value);
        }

        /// <summary>
        ///     Get the <see cref="AddNewJobSetTemplateCommand" /> to create a new <see cref="ProjectJobSetTemplate" /> on the
        ///     current project
        /// </summary>
        public AddNewJobSetTemplateCommand AddJobSetTemplateCommand { get; }

        /// <summary>
        ///     Get the <see cref="DeleteJobSetTemplateCommand" /> to remove a <see cref="ProjectJobSetTemplate" /> from the
        ///     current project
        /// </summary>
        public DeleteJobSetTemplateCommand DeleteSetTemplateCommand { get; }

        /// <summary>
        ///     Get the <see cref="DuplicateJobSetTemplateCommand" /> to add a <see cref="ProjectJobSetTemplate" /> duplicate to
        ///     the current project
        /// </summary>
        public DuplicateJobSetTemplateCommand DuplicateSetTemplateCommand { get; }

        /// <inheritdoc />
        public BasicJobTranslationContentControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            AddJobSetTemplateCommand = new AddNewJobSetTemplateCommand(projectControl, () => SelectedProject, x => ReloadSelectionSource(false, true));
            DeleteSetTemplateCommand = new DeleteJobSetTemplateCommand(projectControl, () => ReloadSelectionSource(true, true));
            DuplicateSetTemplateCommand = new DuplicateJobSetTemplateCommand(projectControl, x => ReloadSelectionSource(false, true));
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
        ///     Nulls the currently selected <see cref="ProjectJobSetTemplate" /> and reloads the option list
        /// </summary>
        private void ReloadSelectionSource(bool nullSelected = true, bool selectLast = false)
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