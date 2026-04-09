using Avalonia.Controls;
using tfox.ViewModels;
using Avalonia.Interactivity;
using System;
using tfox.Common;
using System.Linq;
using System.Collections.Generic;



namespace tfox.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        GroupsDataGrid.SelectionChanged += GroupsDataGridSelectionChangedEvent;
        ItemsDataGrid.DoubleTapped += ItemDataGridDoubleTappedEvent;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        GroupsDataGrid.SelectedItem = GroupsDataGrid.ItemsSource.Cast<ViewModels.LGroup>().FirstOrDefault();//focus first row
        if (DataContext is MainWindowViewModel vm)
        {
            new StorageService(vm.LGroups, vm.LItems).LoadCollections();
            UpdateItemsFilteredByGroup();
            GroupsDataGrid.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Group
    /// </summary>
    /// 

    private async void GroupAddClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            string newID = await new CollectionManager(this, vm.LGroups, vm.LItems).GroupAddAsync();
            if (!string.IsNullOrEmpty(newID))
                new CollectionManager(this, vm.LGroups, vm.LItems).SetGroupsGridCursorPositionByGroupID(GroupsDataGrid, newID);
        }
    }

    private async void GroupEditClick(object? sender, RoutedEventArgs e)
    {
        if (GroupsDataGrid.SelectedItem is LGroup selectedItem && DataContext is MainWindowViewModel vm)
            await new CollectionManager(this, vm.LGroups, vm.LItems).GroupEditAsync(selectedItem);
    }

    private void GroupDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (GroupsDataGrid.SelectedItem is LGroup selectedItem && DataContext is MainWindowViewModel vm)
        {
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            cm.GroupDelete(selectedItem);
            cm.SetGroupsGridCursorPositionByGroupPos(GroupsDataGrid, selectedItem.GroupPos - 1);
            UpdateItemsFilteredByGroup();
        }
    }

    /// <summary>
    /// item
    /// </summary>

    private async void ItemAddClick(object? sender, RoutedEventArgs e)
    {
        if (GroupsDataGrid.SelectedItem is LGroup selectedItem && DataContext is MainWindowViewModel vm)
        {
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            await cm.AddNewItemAsync(this, selectedItem);
            cm.SetGroupsGridCursorPositionByGroupID(GroupsDataGrid, selectedItem.GroupID.ToString());
            UpdateItemsFilteredByGroup();
        }
    }

    private async void ItemEditClick(object? sender, RoutedEventArgs e)
    {
        if (ItemsDataGrid.SelectedItem is LItem selectedItem && DataContext is MainWindowViewModel vm)
        {
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            await cm.ItemEditAsync(this, selectedItem);
            cm.SetGroupsGridCursorPositionByGroupID(GroupsDataGrid, selectedItem.GroupID.ToString());
            UpdateItemsFilteredByGroup();
        }
    }
    private void ItemDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (ItemsDataGrid.SelectedItem is LItem selectedItem && DataContext is MainWindowViewModel vm)
        {
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            cm.ItemDelete(selectedItem);
            cm.SetGroupsGridCursorPositionByGroupID(GroupsDataGrid, selectedItem.GroupID.ToString());
            UpdateItemsFilteredByGroup();
        }
    }

    /// <summary>
    /// Events
    /// </summary>

    private void ItemDataGridDoubleTappedEvent(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is DataGrid grid && grid.SelectedItem != null)
            _ = new LinuxCommandService().RunCommandAsync((LItem)grid.SelectedItem);
    }

    private void GroupsDataGridSelectionChangedEvent(object? sender, SelectionChangedEventArgs e)
    {
        UpdateItemsFilteredByGroup();
    }


    private void UpdateItemsFilteredByGroup()
    {
        if (GroupsDataGrid.SelectedItem is LGroup selectedItem && DataContext is MainWindowViewModel vm)
            vm.UpdateFilteredItems(selectedItem);
        if (((GroupsDataGrid.ItemsSource as IEnumerable<object>)?.Count() ?? 0) == 1)
            GroupsDataGrid.SelectedIndex = 0;
        GroupsDataGrid.Focus();
    }


    /// <summary>
    ///  Move Elements
    /// </summary>

    public void GroupUp(object? sender, RoutedEventArgs? e)
    {
        if (GroupsDataGrid.SelectedItem is LGroup selectedItem && DataContext is MainWindowViewModel vm)
        {
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            cm.MoveGroupUp(selectedItem);
            cm.SetGroupsGridCursorPositionByGroupID(GroupsDataGrid, selectedItem.GroupID.ToString());
            UpdateItemsFilteredByGroup();
        }
    }

    public void GroupDown(object? sender, RoutedEventArgs? e)
    {
        if (GroupsDataGrid.SelectedItem is LGroup selectedItem && DataContext is MainWindowViewModel vm)
        {
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            cm.MoveGroupDown(selectedItem);
            cm.SetGroupsGridCursorPositionByGroupID(GroupsDataGrid, selectedItem.GroupID.ToString());
            UpdateItemsFilteredByGroup();
        }
    }


    public void ItemUp(object? sender, RoutedEventArgs? e)
    {
        if (ItemsDataGrid.SelectedItem is LItem selectedItem && DataContext is MainWindowViewModel vm)
        {
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            cm.MoveItemUp(selectedItem);
            cm.SetGroupsGridCursorPositionByGroupID(GroupsDataGrid, selectedItem.GroupID.ToString());
            UpdateItemsFilteredByGroup();
            cm.SetItemsGridCursorPositionByItemID(ItemsDataGrid, selectedItem.ItemID.ToString());
        }
    }

    public void ItemDown(object? sender, RoutedEventArgs? e)
    {
        if (ItemsDataGrid.SelectedItem is LItem selectedItem && DataContext is MainWindowViewModel vm)
        {
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            cm.MoveItemDown(selectedItem);
            cm.SetGroupsGridCursorPositionByGroupID(GroupsDataGrid, selectedItem.GroupID.ToString());
            UpdateItemsFilteredByGroup();
            cm.SetItemsGridCursorPositionByItemID(ItemsDataGrid, selectedItem.ItemID.ToString());
        }
    }

    /// <summary>
    /// Import export node file
    /// </summary>

    public void ExportToFile(object? sender, RoutedEventArgs? e)
    {
        if (GroupsDataGrid.SelectedItem is LGroup selectedItem && DataContext is MainWindowViewModel vm)
        {
            _ = new StorageService(vm.LGroups, vm.LItems).ExportToFileAsync(this, selectedItem.GroupID);
        }
    }

    public async void ImportFromFile(object? sender, RoutedEventArgs? e)
    {
        if (GroupsDataGrid.SelectedItem is LGroup selectedItem && DataContext is MainWindowViewModel vm)
        {
            if (await new StorageService(vm.LGroups, vm.LItems).ImportFromFileAsync(this) == null) return;
            using var cm = new CollectionManager(this, vm.LGroups, vm.LItems);
            cm.SaveCollections();
        }
    }

}
