using System.Collections.Generic;
using System.ComponentModel;
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
        ///     Get the <see cref="IList{T}"/> of <see cref="ProjectCustomizationGraph"/> instances that can currently be selected
        /// </summary>
        public IList<ProjectCustomizationGraph> CustomizationGraphs => SelectedProjectGraph?.ProjectCustomizationGraphs;

        /// <summary>
        ///     Get the <see cref="AddCustomizationCommand"/> to create a new <see cref="ProjectCustomizationGraph"/> on the current project
        /// </summary>
        public AddNewCustomizationCommand AddCustomizationCommand { get; }

        /// <inheritdoc />
        public BasicCustomizationContentControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            AddCustomizationCommand = new AddNewCustomizationCommand(projectControl, () => SelectedProjectGraph);
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
            OnPropertyChanged(nameof(CustomizationGraphs));
        }
    }
}