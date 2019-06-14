﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Base;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.DataControl
{
    /// <summary>
    ///     Interaktionslogik für ParticleControlView.xaml
    /// </summary>
    public partial class ParticleControlView : UserControl
    {
        /// <summary>
        ///     Get or set the <see cref="DragHandler{TElement}"/> for the particle <see cref="DataGrid"/>
        /// </summary>
        private DragHandler<DataGrid> ParticleDataGridDragHandler { get; set; }

        public ParticleControlView()
        {
            InitializeDragHandlers();
            InitializeComponent();
        }

        private void InitializeDragHandlers()
        {
            ParticleDataGridDragHandler = new DragHandler<DataGrid>(x => new DataObject(x.SelectedItem ?? new object()));
        }

        private void ParticleDataGrid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ParticleDataGridDragHandler.RegisterDragStartPoint(ParticleDataGrid, e);
        }

        private void ParticleDataGrid_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ParticleDataGridDragHandler.DeleteDragStartPoint(ParticleDataGrid, e);
        }

        private void ParticleDataGrid_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            ParticleDataGridDragHandler.TryDoDragDrop(ParticleDataGrid, e);
        }
    }
}