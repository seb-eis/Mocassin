using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Framework.Xml;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.JsonBrowser;
using Mocassin.UI.GUI.Controls.Base;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser
{
    /// <summary>
    ///     The <see cref="Mocassin.UI.GUI.Base.ViewModels.ViewModel" /> for <see cref="ProjectLibraryBrowserView" />
    /// </summary>
    public class ProjectLibraryBrowserViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     The <see cref="ProjectLibrary" /> backing field
        /// </summary>
        private IMocassinProjectLibrary projectLibrary;

        /// <summary>
        ///     The <see cref="JsonBrowserViewModel"/> backing field
        /// </summary>
        private JsonBrowserViewModel jsonBrowserViewModel;

        private ObservableCollection<MocassinProjectGraph> projectGraphs;

        /// <summary>
        ///     Get the currently loaded <see cref="IMocassinProjectLibrary" />
        /// </summary>
        public IMocassinProjectLibrary ProjectLibrary
        {
            get => projectLibrary;
            private set => SetProperty(ref projectLibrary, value);
        }

        /// <summary>
        ///     Get the <see cref="MocassinProjectGraph"/> objects from the <see cref="IMocassinProjectLibrary"/> as an <see cref="ObservableCollection{T}"/>
        /// </summary>
        public ObservableCollection<MocassinProjectGraph> ProjectGraphs
        {
            get => projectGraphs;
            private set => SetProperty(ref projectGraphs, value);
        }

        /// <summary>
        ///     Get the <see cref="JsonBrowserViewModel"/> that provides the <see cref="IMocassinProjectLibrary"/> data as a JSON tree
        /// </summary>
        public JsonBrowserViewModel JsonBrowserViewModel
        {
            get => jsonBrowserViewModel;
            private set => SetProperty(ref jsonBrowserViewModel, value);
        }

        /// <inheritdoc />
        public ProjectLibraryBrowserViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {
            if (mainProjectControl.OpenProjectLibrary != null)
                ProjectLibrary = mainProjectControl.OpenProjectLibrary;

            LoadTestTree();
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ProjectLibrary = newProjectLibrary;
            LoadLibraryContents();
        }

        /// <summary>
        ///     Loads all content from the <see cref="IMocassinProjectLibrary"/> into the affiliated <see cref="ObservableCollection{T}"/>
        /// </summary>
        private void LoadLibraryContents()
        {
            if (projectLibrary == null) return;
            projectLibrary.MocassinProjectGraphs.Load();
            projectLibrary.ProjectModelGraphs.Load();
            projectLibrary.ProjectCustomizationGraphs.Load();
            projectLibrary.ProjectJobTranslationGraphs.Load();
            projectLibrary.MocassinProjectBuildGraphs.Load();
            ProjectGraphs = ProjectLibrary.MocassinProjectGraphs.Local.ToObservableCollection();
        }

        private void LoadTestTree()
        {
            const string _basePath = "C:\\Users\\hims-user\\Documents\\Gitlab\\MocassinTestFiles\\YDopedCeria\\";
            const string _baseFile = "Ceria";
            var filePath = _basePath + _baseFile + ".Input.xml";

            Console.WriteLine($"Reading project XML description from: {filePath}");
            if (XmlStreamService.TryDeserialize(filePath, null, out ProjectModelGraph data, out var exception))
            {
                var browser = new JsonBrowserViewModel();
                browser.SetActiveTreeView(data, "Library");
                JsonBrowserViewModel = browser;
            }

            var library = SqLiteContext.OpenDatabase<MocassinProjectContext>(_basePath + "ProjectLibrary.db");
            OnProjectLibraryChangedInternal(library);
        }
    }
}