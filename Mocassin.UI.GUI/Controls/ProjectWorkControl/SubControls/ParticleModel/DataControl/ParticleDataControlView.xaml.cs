using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel.DataControl
{
    /// <summary>
    ///     Interaktionslogik für ParticleDataControlView.xaml
    /// </summary>
    public partial class ParticleDataControlView : UserControl
    {
        public ParticleDataControlView()
        {
            InitializeComponent();
        }

        private void DataGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is DataGrid dataGrid)
            {
                DragDrop.DoDragDrop(this, new DataObject(dataGrid.SelectedItem), DragDropEffects.Copy);
            }
        }
    }
}