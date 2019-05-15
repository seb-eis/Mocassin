using System;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.Views;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content
{
    /// <summary>
    ///     Basic <see cref="ViewModelBase" /> for <see cref="BasicModelContentControlView" /> that provides a default control
    ///     content for model data manipulation
    /// </summary>
    public class BasicModelContentControlViewModel : PrimaryControlViewModel
    {
        private MocassinProjectGraph selectedProjectGraph;
        private ContentControl dataContentControl;
        private ContentControl visualizerContentControl;
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
        protected void OnProjectGraphSelectionChanged()
        {
            NotifyGraphSelectionChanged(DataContentControl);
            NotifyGraphSelectionChanged(VisualizerContentControl);
            NotifyGraphSelectionChanged(InfoContentControl);
        }

        /// <summary>
        ///     Action that is invoked if a selected object of type <see cref="T" /> has changed
        /// </summary>
        /// <param name="value"></param>
        protected void OnSelectionChanged<T>(T value)
        {
            NotifySelectionChanged(DataContentControl, value);
            NotifySelectionChanged(VisualizerContentControl, value);
            NotifySelectionChanged(InfoContentControl, value);
        }

        /// <summary>
        ///     Tries to notify a <see cref="ContentControl" /> about a change in the selected <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="contentControl"></param>
        protected void NotifyGraphSelectionChanged(ContentControl contentControl)
        {
            (contentControl.DataContext as IContentSupplier<MocassinProjectGraph>)?.ChangeContentSource(SelectedProjectGraph);
        }

        /// <summary>
        ///     Notifies the passed <see cref="ContentControl" /> about a selection change if it implements
        ///     the generic <see cref="IContentSupplier{T}" /> interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentControl"></param>
        /// <param name="value"></param>
        protected void NotifySelectionChanged<T>(ContentControl contentControl, T value)
        {
            (contentControl.DataContext as IContentSupplier<T>)?.ChangeContentSource(value);
        }
    }
}