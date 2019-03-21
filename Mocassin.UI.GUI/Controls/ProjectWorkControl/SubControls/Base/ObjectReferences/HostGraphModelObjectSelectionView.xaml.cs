using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mocassin.Model.Particles;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Extensions;
using Mocassin.UI.GUI.Helper;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.GridControl
{
    /// <summary>
    ///     Interaktionslogik für HostGraphModelObjectSelectionView.xaml
    /// </summary>
    public partial class HostGraphModelObjectSelectionView : UserControl
    {
        public HostGraphModelObjectSelectionView()
        {
            InitializeComponent();
        }

        private void DataGrid_OnDrop(object sender, DragEventArgs e)
        {
            this.RelayDropToContext(sender as ItemsControl, e);
        }
    }
}   