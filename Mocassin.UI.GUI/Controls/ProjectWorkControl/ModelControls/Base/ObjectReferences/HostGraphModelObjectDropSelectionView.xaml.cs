using System.Windows;
using System.Windows.Controls;
using Mocassin.UI.GUI.Extensions;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl
{
    /// <summary>
    ///     Interaktionslogik für HostGraphModelObjectDropSelectionView.xaml
    /// </summary>
    public partial class HostGraphModelObjectDropSelectionView : UserControl
    {
        public HostGraphModelObjectDropSelectionView()
        {
            InitializeComponent();
        }

        private void DataGrid_OnDrop(object sender, DragEventArgs e)
        {
            this.RelayDropToContext(sender as ItemsControl, e);
        }
    }
}