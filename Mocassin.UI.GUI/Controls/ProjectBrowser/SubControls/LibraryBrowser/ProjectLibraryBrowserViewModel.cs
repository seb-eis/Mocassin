using System;
using System.Linq;
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
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.ProjectLibrary;
using Newtonsoft.Json;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> for <see cref="ProjectLibraryBrowserView" />
    /// </summary>
    public class ProjectLibraryBrowserViewModel : PrimaryControlViewModel, IObjectDropAcceptor
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
        ///     Get or set the json <see cref="string"/> of a passed object
        /// </summary>
        public string ObjectJson
        {
            get => objectJson;
            set => SetProperty(ref objectJson, value);
        }

        
        /// <summary>
        ///     Get or set the Xml <see cref="string"/> of a passed object
        /// </summary>
        public string ObjectXml
        {
            get => objectXml;
            set => SetProperty(ref objectXml, value);
        }

        /// <summary>
        ///     Get a <see cref="AsyncRelayCommand{T}" /> to set a <see cref="TreeViewItem"/> representation of the parameter
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
        public Command<IDataObject> HandleDropAddCommand { get; }

        /// <inheritdoc />
        public ProjectLibraryBrowserViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            JsonBrowserViewModel = new JsonBrowserViewModel();
            Func<object, bool> canExecute = obj => obj != null;
            SetObjectTreeViewCommand = new AsyncRelayCommand<object>(SetActiveObjectTreeViewAsync, canExecute);
            SetObjectXmlCommand = new AsyncRelayCommand<object>(SetActiveObjectXmlAsync, canExecute);
            SetObjectJsonCommand = new AsyncRelayCommand<object>(SetActiveObjectJsonAsync, canExecute);
            HandleDropAddCommand = new RelayCommand<IDataObject>(SetObjectDropByFormat, canExecute);
        }

        /// <summary>
        ///     Rebuilds the <see cref="TreeViewItem" /> collection of the passed <see cref="object"/>
        /// </summary>
        private async Task SetActiveObjectTreeViewAsync(object obj)
        {
            await ExecuteOnDispatcherAsync(() => JsonBrowserViewModel.SetRootViewToNoContent());
            if (obj == null) return;

            await ExecuteOnDispatcherAsync(() => JsonBrowserViewModel.SetActiveTreeView(obj, "Object_Tree"));
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
            var  settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.None,
                Culture = System.Globalization.CultureInfo.InvariantCulture,
            };

            try
            {
                var json = await Task.Run(() => JsonConvert.SerializeObject(obj, Formatting.Indented, settings));
                ExecuteOnDispatcher(() => ObjectJson = json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ObjectJson = null;
            }
        }

        /// <summary>
        ///     Sets the passed object <see cref="IDataObject"/> contents based upon the contained set view format key
        /// </summary>
        /// <param name="dataObject"></param>
        private async void SetObjectDropByFormat(IDataObject dataObject)
        {
            if (dataObject.GetData(Resources.DataObjectFormatKey_ViewFormat_Json) is object jsonConvertible)
            {
                await SetActiveObjectJsonAsync(jsonConvertible);
            }
            if (dataObject.GetData(Resources.DataObjectFormatKey_ViewFormat_Xml) is object xmlConvertible)
            {
                await SetActiveObjectXmlAsync(xmlConvertible);
            }
            if (dataObject.GetData(Resources.DataObjectFormatKey_ViewFormat_Tree) is object treeConvertible)
            {
                await SetActiveObjectTreeViewAsync(treeConvertible);
            }
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            SendToDispatcher(() => JsonBrowserViewModel.SetRootViewToNoContent());
        }
    }
}