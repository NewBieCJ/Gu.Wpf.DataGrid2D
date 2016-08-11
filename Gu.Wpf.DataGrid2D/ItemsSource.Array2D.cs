﻿namespace Gu.Wpf.DataGrid2D
{
    using Internals;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public static partial class ItemsSource
    {
        public static readonly DependencyProperty Array2DProperty = DependencyProperty.RegisterAttached(
            "Array2D",
            typeof(Array),
            typeof(ItemsSource),
            new PropertyMetadata(default(Array), OnArray2DChanged),
            OnValidateArray2D);

        public static void SetArray2D(this DataGrid element, Array value)
        {
            element.SetValue(Array2DProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static Array GetArray2D(this DataGrid element)
        {
            return (Array)element.GetValue(Array2DProperty);
        }

        private static void OnArray2DChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)d;
            dataGrid.AutoGeneratingColumn -= DataGrid_AutoGeneratingColumn;
            var array = (Array)e.NewValue;
            if (array == null)
            {
                BindingOperations.ClearBinding(dataGrid, ItemsControl.ItemsSourceProperty);
                return;
            }

            var array2DView = Array2DView.Create(array);
            dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
            dataGrid.Bind(ItemsControl.ItemsSourceProperty)
                    .OneWayTo(array2DView);
            dataGrid.RaiseEvent(new RoutedEventArgs(Events.ColumnsChanged));
        }

        private static void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            CustomDataGridTemplateColumn col = new CustomDataGridTemplateColumn();
            col.CellTemplate = (DataTemplate)((DataGrid)sender).GetCellTemplate();
            col.CellEditingTemplate = (DataTemplate)((DataGrid)sender).GetCellEditingTemplate();
            if (col.CellTemplate != null && col.CellEditingTemplate != null)
            {
                DataGridTextColumn tc = e.Column as DataGridTextColumn;
                if (tc != null && tc.Binding != null)
                {
                    col.Binding = tc.Binding;
                    e.Column = col;
                }
            }
        }

        private static bool OnValidateArray2D(object value)
        {
            var array = value as Array;
            if (array != null)
            {
                return array.Rank == 2;
            }

            return true;
        }
    }
}
