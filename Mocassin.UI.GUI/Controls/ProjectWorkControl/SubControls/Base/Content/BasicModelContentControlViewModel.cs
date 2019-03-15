using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.Views;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.Content
{
    /// <summary>
    ///     Basic <see cref="ViewModel" /> for <see cref="BasicModelContentControlView" /> that provides a default control
    ///     content for model data manipulation
    /// </summary>
    public class BasicModelContentControlViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     The <see cref="SelectedProjectGraph" /> backing field
        /// </summary>
        private MocassinProjectGraph selectedProjectGraph;

        /// <summary>
        ///     The <see cref="DataContentControl" /> backing field
        /// </summary>
        private ContentControl dataContentControl;

        /// <summary>
        ///     The <see cref="InfoContentControl" /> backing field
        /// </summary>
        private ContentControl visualizerContentControl;

        /// <summary>
        ///     The <see cref="VisualizerContentControl" /> backing field
        /// </summary>
        private ContentControl infoContentControl;

        /// <summary>
        ///     Get or set the selected <see cref="MocassinProjectGraph" />
        /// </summary>
        public MocassinProjectGraph SelectedProjectGraph
        {
            get => selectedProjectGraph;
            set
            {
                SetProperty(ref selectedProjectGraph, value);
                OnProjectGraphSelectionChanged();
            }
        }

        /// <summary>
        ///     Get or set the <see cref="ContentControl" /> for data manipulation
        /// </summary>
        public ContentControl DataContentControl
        {
            get => dataContentControl ?? new NoContentView();
            set => SetProperty(ref dataContentControl, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ContentControl" /> for data visualization
        /// </summary>
        public ContentControl VisualizerContentControl
        {
            get => visualizerContentControl ?? new NoContentView();
            set => SetProperty(ref visualizerContentControl, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ContentControl" /> for data info
        /// </summary>
        public ContentControl InfoContentControl
        {
            get => infoContentControl ?? new NoContentView();
            set => SetProperty(ref infoContentControl, value);
        }

        /// <inheritdoc />
        public BasicModelContentControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <summary>
        ///     Action that is invoked if the selected <see cref="MocassinProjectGraph" /> changes
        /// </summary>
        private void OnProjectGraphSelectionChanged()
        {
            NotifyGraphSelectionChanged(DataContentControl);
            NotifyGraphSelectionChanged(VisualizerContentControl);
            NotifyGraphSelectionChanged(InfoContentControl);
        }

        /// <summary>
        ///     Tries to notify a <see cref="ContentControl" /> about a change in the selected <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="contentControl"></param>
        private void NotifyGraphSelectionChanged(ContentControl contentControl)
        {
            if (contentControl.DataContext is IReactToSelectionChanges<MocassinProjectGraph> reactToSelectionChanges)
                reactToSelectionChanges.NotifyThatSelectionChanged(SelectedProjectGraph);
        }
    }
}