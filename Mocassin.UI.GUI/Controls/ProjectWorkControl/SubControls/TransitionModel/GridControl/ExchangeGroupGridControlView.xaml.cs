﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mocassin.UI.GUI.Base;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.GridControl
{
    /// <summary>
    /// Interaktionslogik für ExchangeGroupGridControlView.xaml
    /// </summary>
    public partial class ExchangeGroupGridControlView : UserControl
    {
        /// <summary>
        ///     Get or set the <see cref="DragHandler{TElement}"/> for drag events on the exchange group grid
        /// </summary>
        private DragHandler<DataGrid> ExchangeGroupDataGridDragHandler { get; set; }

        public ExchangeGroupGridControlView()
        {
            InitializeDragHandlers();
            InitializeComponent();
        }

        private void InitializeDragHandlers()
        {
            ExchangeGroupDataGridDragHandler = new DragHandler<DataGrid>(x => new DataObject(x.SelectedItem ?? new object()));
        }

        private void ExchangeGroupDataGrid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExchangeGroupDataGridDragHandler.RegisterDragStartPoint(sender as DataGrid, e);
        }

        private void ExchangeGroupDataGrid_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ExchangeGroupDataGridDragHandler.DeleteDragStartPoint(sender as DataGrid, e);
        }

        private void ExchangeGroupDataGrid_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            ExchangeGroupDataGridDragHandler.TryDoDragDrop(sender as DataGrid, e);
        }
    }
}
