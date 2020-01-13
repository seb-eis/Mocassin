using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content
{
    /// <summary>
    ///     Basic customization content control that extends <see cref="BasicModelContentControlViewModel" /> with a
    ///     <see cref="ProjectCustomizationGraph" /> selection
    /// </summary>
    public class BasicCustomizationContentControlViewModel : BasicModelContentControlViewModel
    {
        private ObservableCollection<ProjectCustomizationGraph> customizationGraphs;
        private ProjectCustomizationGraph selectedCustomizationGraph;

        /// <summary>
        ///     Get or set the currently selected <see cref="ProjectCustomizationGraph" />
        /// </summary>
        public ProjectCustomizationGraph SelectedCustomizationGraph
        {
            get => selectedCustomizationGraph;
            set => SetProperty(ref selectedCustomizationGraph, value, () => OnSelectionChanged(value));
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}"/> of <see cref="ProjectCustomizationGraph"/> instances that can currently be selected
        /// </summary>
        public ObservableCollection<ProjectCustomizationGraph> CustomizationGraphs
        {
            get => customizationGraphs;
            protected set => SetProperty(ref customizationGraphs, value);
        }

        /// <summary>
        ///     Get the command to create a new <see cref="ProjectCustomizationGraph"/> on the current project
        /// </summary>
        public AddNewCustomizationCommand AddCustomizationCommand { get; }

        /// <summary>
        ///     Get the command to delete a <see cref="ProjectCustomizationGraph"/> from the current project
        /// </summary>
        public DeleteCustomizationCommand DeleteCustomizationCommand { get; }

        /// <summary>
        ///     Get the command to duplicate a <see cref="ProjectCustomizationGraph"/> and add it to the current project
        /// </summary>
        public DuplicateCustomizationCommand DuplicateCustomizationCommand { get; }

        /// <summary>
        ///     Get the command to migrate a <see cref="ProjectCustomizationGraph"/> and add it to the current project
        /// </summary>
        public MigrateCustomizationCommand MigrateCustomizationCommand { get; }

        /// <inheritdoc />
        public BasicCustomizationContentControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            AddCustomizationCommand = new AddNewCustomizationCommand(projectControl, () => SelectedProjectGraph, x => ReloadSelectionSource(false, true));
            DeleteCustomizationCommand = new DeleteCustomizationCommand(projectControl, () => ReloadSelectionSource(true, true));
            DuplicateCustomizationCommand = new DuplicateCustomizationCommand(projectControl, x => ReloadSelectionSource(false, true));
            MigrateCustomizationCommand = new MigrateCustomizationCommand(projectControl, x => ReloadSelectionSource(false, true));
            PropertyChanged += OnCustomizationSourceChanged;
        }

        /// <summary>
        ///     Notifies property change of <see cref="CustomizationGraphs"/> if the selected project property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCustomizationSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedProjectGraph)) return;
            ReloadSelectionSource();
        }

        /// <inheritdoc />
        protected override void OnProjectContentChangedInternal()
        {
            CustomizationGraphs = SelectedProjectGraph?.ProjectCustomizationGraphs;
        }

        /// <summary>
        ///     Nulls the currently selected <see cref="ProjectCustomizationGraph"/> and reloads the option list
        /// </summary>
        protected void ReloadSelectionSource(bool nullSelected = true, bool selectLast = false)
        {
            ExecuteOnAppThread(() =>
            {
                if (nullSelected) SelectedCustomizationGraph = null;
                CustomizationGraphs = SelectedProjectGraph?.ProjectCustomizationGraphs;
                if (selectLast) SelectedCustomizationGraph = CustomizationGraphs?.LastOrDefault();
            });
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            PropertyChanged -= OnCustomizationSourceChanged;
            base.Dispose();
        }
    }
}