using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mocassin.UI.GUI.Base.ViewModels.JsonBrowser
{
    /// <summary>
    ///     Builds a simple <see cref="TreeViewItem" /> graph from JSON formatted data
    /// </summary>
    public class JsonTreeViewBuilder
    {
        private static Regex CapitalRegex { get; } = new Regex(@"(?:([a-z]{1})([A-Z0-9]))");

        /// <summary>
        ///     Converts the given <see cref="object"/>  to a <see cref="TreeViewItem"/> using the <see cref="JsonConvert"/> system
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rootName"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public TreeViewItem ConvertToTreeView(object obj, string rootName, JsonSerializerSettings serializerSettings = null)
        {
            try
            {
                var settings = JsonSerializer.CreateDefault(serializerSettings);
                var treeView = new TreeViewItem {Header = rootName};
                switch (obj)
                {
                    case JObject jObject:
                        AddToTreeView(jObject, treeView);
                        break;

                    case JArray jArray:
                        AddToTreeView(jArray, treeView);
                        break;

                    case IEnumerable _:
                        var enumerable = JArray.FromObject(obj, settings);
                        AddToTreeView(enumerable, treeView);
                        break;

                    default:
                        var @object = JObject.FromObject(obj, settings);
                        AddToTreeView(@object, treeView);
                        break;
                }

                return treeView;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return MakeExceptionItem(exception);
            }
        }

        /// <summary>
        ///     Converts a json formatted <see cref="string" /> into a nested <see cref="TreeViewItem" />
        /// </summary>
        /// <param name="json"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public TreeViewItem JsonToTreeView(string json, string rootName)
        {
            try
            {
                var jObject = JObject.Parse(json);
                var rootViewItem = new TreeViewItem {Header = rootName};
                AddToTreeView(jObject, rootViewItem);
                return rootViewItem;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return MakeExceptionItem(exception);
            }
        }

        /// <summary>
        ///     Adds a <see cref="JObject" /> to the provided parent <see cref="TreeViewItem" />
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="parentViewItem"></param>
        public void AddToTreeView(JObject jObject, TreeViewItem parentViewItem)
        {
            foreach (var jProperty in jObject.Properties())
            {
                var treeView = new TreeViewItem {Header = CreateBaseHeader(jProperty.Name)};
                parentViewItem.Items.Add(treeView);
                AddToTreeView(jProperty.Value, treeView);
            }
        }

        /// <summary>
        ///     Adds a <see cref="JArray" /> to the provided parent <see cref="TreeViewItem" />
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="parentViewItem"></param>
        public void AddToTreeView(JArray jArray, TreeViewItem parentViewItem)
        {
            var index = 0;
            foreach (var jToken in jArray)
            {
                var treeView = new TreeViewItem {Header = $"[{index++}]"};
                parentViewItem.Items.Add(treeView);
                AddToTreeView(jToken, treeView);
            }
        }

        /// <summary>
        ///     Adds any <see cref="JToken" /> to the provided parent <see cref="TreeViewItem" />
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="parentViewItem"></param>
        public void AddToTreeView(JToken jToken, TreeViewItem parentViewItem)
        {
            var viewItem = new TreeViewItem();
            switch (jToken)
            {
                case JValue jValue:
                    parentViewItem.Header = CreateHeaderWithValue(jValue, parentViewItem);
                    return;

                case JArray jArray:
                    AddToTreeView(jArray, parentViewItem);
                    return;

                case JObject jObject:
                    AddToTreeView(jObject, parentViewItem);
                    return;

                default:
                    return;
            }
        }

        /// <summary>
        ///     Creates a <see cref="TreeViewItem"/> for any <see cref="Exception"/>
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static TreeViewItem MakeExceptionItem(Exception e)
        {
            var item = new TreeViewItem {Header = $"Exception : {e?.Message ?? "Unknown"}"};
            return item;
        }

        /// <summary>
        ///     Creates the header <see cref="string"/> for the passed <see cref="JValue"/> and <see cref="TreeViewItem"/> parent
        /// </summary>
        /// <param name="jValue"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static string CreateHeaderWithValue(JValue jValue, TreeViewItem parent)
        {
            return $"{parent.Header} : \"{jValue.ToString(CultureInfo.InvariantCulture)}\"";
        }

        /// <summary>
        ///     Creates the base header of an item
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CreateBaseHeader(string name)
        {
            return CapitalRegex.Replace(name, x => $"{x.Groups[1]}_{x.Groups[2]}");
        }
    }
}