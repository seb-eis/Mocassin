using System;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.DataBrowser;
using Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.ReportBrowser;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser
{
    /// <summary>
    ///     The <see cref="UserControlTabControlViewModel" /> that controls project browser tabs
    /// </summary>
    public class ProjectBrowserViewModel : UserControlTabControlViewModel
    {
        /// <summary>
        /// Get the <see cref="ModelDataBrowserViewModel"/> that controls model data browsing
        /// </summary>
        private ModelDataBrowserViewModel ModelDataBrowserViewModel { get; }

        /// <summary>
        /// Get the <see cref="ReportBrowserViewModel"/> that controls report browsing
        /// </summary>
        private ReportBrowserViewModel ReportBrowserViewModel { get; }

        /// <summary>
        /// Creates new <see cref="ProjectBrowserViewModel"/> from
        /// </summary>
        /// <param name="modelDataBrowserViewModel"></param>
        /// <param name="reportBrowserViewModel"></param>
        public ProjectBrowserViewModel(ModelDataBrowserViewModel modelDataBrowserViewModel, ReportBrowserViewModel reportBrowserViewModel)
        {
            ModelDataBrowserViewModel = modelDataBrowserViewModel ?? throw new ArgumentNullException(nameof(modelDataBrowserViewModel));
            ReportBrowserViewModel = reportBrowserViewModel ?? throw new ArgumentNullException(nameof(reportBrowserViewModel));
            InitializeDefaultTabs();
        }

        /// <inheritdoc />
        protected sealed override void InitializeDefaultTabs()
        {
            base.InitializeDefaultTabs();
            AddNonClosableTab("Model Browser", ModelDataBrowserViewModel, new ModelDataBrowserView());
            AddNonClosableTab("Report Browser", ReportBrowserViewModel, new ReportBrowserView());
        }
    }
}