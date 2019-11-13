using System.Collections.Generic;
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
        private IReadOnlyList<ProjectCustomizationGraph> customizationGraphs;
        private ProjectCustomizationGraph selectedCustomizationGraph;

        /// <summary>
        ///     Get or set the currently selected <see cref="ProjectCustomizationGraph" />
        /// </summary>
        public ProjectCustomizationGraph SelectedCustomizationGraph
        {
            get => selectedCustomizationGraph;
            set
            {
                SetProperty(ref selectedCustomizationGraph, value);
                OnSelectionChanged(value);
            }
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}"/> of <see cref="ProjectCustomizationGraph"/> instances that can currently be selected
        /// </summary>
        public IReadOnlyList<ProjectCustomizationGraph> CustomizationGraphs
        {
            get => customizationGraphs;
            set => SetProperty(ref customizationGraphs, value);
        }

        /// <summary>
        ///     Get the <see cref="AddNewCustomizationCommand"/> to create a new <see cref="ProjectCustomizationGraph"/> on the current project
        /// </summary>
        public AddNewCustomizationCommand AddCustomizationCommand { get; }

        /// <summary>
        ///     Get the <see cref="DeleteCustomizationCommand"/> to delete a <see cref="ProjectCustomizationGraph"/> from the current project
        /// </summary>
        public DeleteCustomizationCommand DeleteCustomizationCommand { get; }

        /// <summary>
        ///     Get the <see cref="DuplicateCustomizationCommand"/> to duplicate a <see cref="ProjectCustomizationGraph"/> and add it to the current project
        /// </summary>
        public DuplicateCustomizationCommand DuplicateCustomizationCommand { get; }

        /// <inheritdoc />
        public BasicCustomizationContentControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            AddCustomizationCommand = new AddNewCustomizationCommand(projectControl, () => SelectedProjectGraph, () => ReloadSelectionSource(false, true));
            DeleteCustomizationCommand = new DeleteCustomizationCommand(projectControl,() => SelectedProjectGraph, () => ReloadSelectionSource(true, true));
            DuplicateCustomizationCommand = new DuplicateCustomizationCommand(projectControl, () => SelectedProjectGraph, () => ReloadSelectionSource(false, true));
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
            CustomizationGraphs = SelectedProjectGraph?.ProjectCustomizationGraphs.ToList();
        }

        /// <summary>
        ///     Nulls the currently selected <see cref="ProjectCustomizationGraph"/> and reloads the option list
        /// </summary>
        protected void ReloadSelectionSource(bool nullSelected = true, bool selectLast = false)
        {
            ExecuteOnDispatcher(() =>
            {
                if (nullSelected) SelectedCustomizationGraph = null;
                CustomizationGraphs = SelectedProjectGraph?.ProjectCustomizationGraphs?.ToList();
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