using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Windows.Controls;
using Mocassin.Framework.Extensions;
using Newtonsoft.Json;

namespace Mocassin.UI.GUI.Base.ViewModels.JsonBrowser
{
    /// <summary>
    ///     <see cref="ViewModel"/> for <see cref="JsonBrowserView"/> that enables browsing of JSON data
    /// </summary>
    public class JsonBrowserViewModel : ViewModel
    {
        /// <summary>
        ///     The <see cref="RootViewItems"/> backing field
        /// </summary>
        private IEnumerable<TreeViewItem> rootViewItems;

        /// <summary>
        ///     Get or set the <see cref="TreeViewItem"/> that is the root item
        /// </summary>
        public IEnumerable<TreeViewItem> RootViewItems
        {
            get => rootViewItems;
            private set => SetProperty(ref rootViewItems, value);
        }

        /// <summary>
        ///     Get or set the <see cref="JsonSerializerSettings"/> that should be used
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; }

        /// <summary>
        ///     Get or set the default root name that is used if none is provided
        /// </summary>
        public string DefaultRootName { get; set; } = "JObject";

        /// <summary>
        ///     Sets the active <see cref="TreeViewItem"/> by converting the provided <see cref="object"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rootName"></param>
        public void SetActiveTreeView(object obj, string rootName = null)
        {
            var builder = new JsonTreeViewBuilder();
            RootViewItems = builder.ConvertToTreeView(obj, rootName ?? DefaultRootName, SerializerSettings).AsSingleton();
        }

        /// <summary>
        ///     Sets the active <see cref="TreeViewItem"/> by converting the provided json formatted <see cref="string"/>
        /// </summary>
        /// <param name="json"></param>
        /// <param name="rootName"></param>
        public void SetActiveTreeView(string json, string rootName = null)
        {
            var builder = new JsonTreeViewBuilder();
            RootViewItems = builder.ConvertToTreeView(json, rootName ?? DefaultRootName, SerializerSettings).AsSingleton();
        }
    }
}