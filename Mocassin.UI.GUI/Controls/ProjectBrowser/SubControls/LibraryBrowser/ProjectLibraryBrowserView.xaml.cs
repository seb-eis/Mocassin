using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mocassin.UI.GUI.Extensions;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser
{
    /// <summary>
    ///     Interaktionslogik für ProjectLibraryBrowserView.xaml
    /// </summary>
    public partial class ProjectLibraryBrowserView : UserControl
    {
        public ProjectLibraryBrowserView()
        {
            InitializeComponent();
        }

        private void UIElement_OnDragOver(object sender, DragEventArgs e)
        {
            AddSenderConversionFormatToDragEventArgs(sender, e);
            this.RelayDragOverToContext(e, DragDropEffects.Copy);
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            AddSenderConversionFormatToDragEventArgs(sender, e);
            this.RelayDropToContext(null, e);
        }

        private void AddSenderConversionFormatToDragEventArgs(object sender, DragEventArgs e)
        {
            if (e.Data.GetFormats().Length != 1) return;

            var obj = e.Data.GetData(e.Data.GetFormats().FirstOrDefault());
            if (ReferenceEquals(sender, JsonViewerTab))
            {
                e.Data.SetData(Properties.Resources.DataObjectFormatKey_ViewFormat_Json, obj);
                return;
            }

            if (ReferenceEquals(sender, XmlViewerTab))
            {
                e.Data.SetData(Properties.Resources.DataObjectFormatKey_ViewFormat_Xml, obj);
                return;
            }

            if (ReferenceEquals(sender, TreeViewerTab)) e.Data.SetData(Properties.Resources.DataObjectFormatKey_ViewFormat_Tree, obj);
        }
    }
}