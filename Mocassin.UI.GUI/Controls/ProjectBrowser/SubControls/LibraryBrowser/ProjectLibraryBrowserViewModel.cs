using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Mocassin.Framework.Xml;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.JsonBrowser;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.ProjectLibrary;
using Newtonsoft.Json;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> for <see cref="ProjectLibraryBrowserView" />
    /// </summary>
    public class ProjectLibraryBrowserViewModel : PrimaryControlViewModel, IDataObjectAcceptor
    {
        private JsonBrowserViewModel jsonBrowserViewModel;
        private string objectJson;
        private string objectXml;

        /// <summary>
        ///     Get the <see cref="JsonBrowserViewModel" /> that provides the <see cref="IMocassinProjectLibrary" /> data as a JSON
        ///     tree
        /// </summary>
        public JsonBrowserViewModel JsonBrowserViewModel
        {
            get => jsonBrowserViewModel;
            private set => SetProperty(ref jsonBrowserViewModel, value);
        }

        /// <summary>
        ///     Get or set the json <see cref="string" /> of a passed object
        /// </summary>
        public string ObjectJson
        {
            get => objectJson;
            set => SetProperty(ref objectJson, value);
        }


        /// <summary>
        ///     Get or set the Xml <see cref="string" /> of a passed object
        /// </summary>
        public string ObjectXml
        {
            get => objectXml;
            set => SetProperty(ref objectXml, value);
        }

        /// <summary>
        ///     Get a <see cref="AsyncRelayCommand{T}" /> to set a <see cref="TreeViewItem" /> representation of the parameter
        /// </summary>
        public AsyncRelayCommand<object> SetObjectTreeViewCommand { get; }

        /// <summary>
        ///     Get a <see cref="AsyncRelayCommand{T}" /> to set a JSON <see cref="string" /> representation of the parameter
        ///     <see cref="object" />
        /// </summary>
        public AsyncRelayCommand<object> SetObjectJsonCommand { get; }

        /// <summary>
        ///     Get a <see cref="AsyncRelayCommand{T}" /> to set a Xml <see cref="string" /> representation of the parameter
        ///     <see cref="object" />
        /// </summary>
        public AsyncRelayCommand<object> SetObjectXmlCommand { get; }

        /// <inheritdoc />
        public Command<IDataObject> ProcessDataObjectCommand { get; }

        /// <inheritdoc />
        public ProjectLibraryBrowserViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            JsonBrowserViewModel = new JsonBrowserViewModel();

            bool CanExecute(object obj)
            {
                return obj != null;
            }

            SetObjectTreeViewCommand = new AsyncRelayCommand<object>(SetActiveObjectTreeViewAsync, CanExecute);
            SetObjectXmlCommand = new AsyncRelayCommand<object>(SetActiveObjectXmlAsync, CanExecute);
            SetObjectJsonCommand = new AsyncRelayCommand<object>(SetActiveObjectJsonAsync, CanExecute);
            ProcessDataObjectCommand = new RelayCommand<IDataObject>(SetObjectDropByFormat, CanExecute);
        }

        /// <summary>
        ///     Rebuilds the <see cref="TreeViewItem" /> collection of the passed <see cref="object" />
        /// </summary>
        private async Task SetActiveObjectTreeViewAsync(object obj)
        {
            await ExecuteOnAppThreadAsync(() => JsonBrowserViewModel.SetRootViewToNoContent());
            if (obj == null) return;

            await ExecuteOnAppThreadAsync(() => JsonBrowserViewModel.SetActiveTreeView(obj, "Object_Tree"));
        }

        /// <summary>
        ///     Serializes the passed object to its XML representation and sets the affiliated property to the new value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task SetActiveObjectXmlAsync(object obj)
        {
            ObjectXml = await Task.Run(() => XmlStreamService.Serialize(obj, Encoding.UTF8));
        }

        /// <summary>
        ///     Serializes the passed object to its JSON representation and sets the affiliated property to the new value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task SetActiveObjectJsonAsync(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.None,
                Culture = CultureInfo.InvariantCulture
            };

            try
            {
                var json = await Task.Run(() => JsonConvert.SerializeObject(obj, Formatting.Indented, settings));
                ExecuteOnAppThread(() => ObjectJson = json);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ObjectJson = null;
            }
        }

        /// <summary>
        ///     Sets the passed object <see cref="IDataObject" /> contents based upon the contained set view format key
        /// </summary>
        /// <param name="dataObject"></param>
        private async void SetObjectDropByFormat(IDataObject dataObject)
        {
            if (dataObject.GetData(Resources.DataObjectFormatKey_ViewFormat_Json) is { } jsonConvertible) await SetActiveObjectJsonAsync(jsonConvertible);
            if (dataObject.GetData(Resources.DataObjectFormatKey_ViewFormat_Xml) is { } xmlConvertible) await SetActiveObjectXmlAsync(xmlConvertible);
            if (dataObject.GetData(Resources.DataObjectFormatKey_ViewFormat_Tree) is { } treeConvertible) await SetActiveObjectTreeViewAsync(treeConvertible);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            QueueOnAppDispatcher(() => JsonBrowserViewModel.SetRootViewToNoContent());
        }
    }
}