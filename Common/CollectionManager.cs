using System.Threading.Tasks;
using System.Collections.ObjectModel;
using tfox.ViewModels;
using Avalonia.Controls;
using System;
using System.Linq;
using ReactiveUI;

namespace tfox.Common;

public class CollectionManager : IDisposable
{
    private ObservableCollection<LGroup> _LGroups { get; }
    private ObservableCollection<LItem> _LItems { get; }

    public ObservableCollection<LItem> FilteredItems { get; } = new();
    private Window? _Owner;

    public CollectionManager(Window? Owner, ObservableCollection<LGroup> LGroups, ObservableCollection<LItem> LItems)
    {
        _LGroups = LGroups;
        _LItems = LItems;
        _Owner = Owner;
    }


    public void SetGroupsGridCursorPositionByGroupID(DataGrid grid, string groupID)
    {
        var item = _LGroups.FirstOrDefault(g => g.GroupID.ToString() == groupID);
        if (item != null)
        {
            grid.SelectedItem = item;
            grid.ScrollIntoView(item, null);
        }
    }

    public void SetGroupsGridCursorPositionByGroupPos(DataGrid grid, int groupPos)
    {
        if (groupPos == 1) return;
        var item = _LGroups.FirstOrDefault(g => g.GroupPos == groupPos);
        if (item != null)
        {
            grid.SelectedItem = item;
            grid.ScrollIntoView(item, null);
        }
    }

    public void SetItemsGridCursorPositionByItemID(DataGrid grid, string itemID)
    {
        var item = _LItems.FirstOrDefault(g => g.ItemID.ToString() == itemID);
        if (item != null)
        {
            grid.SelectedItem = item;
            grid.ScrollIntoView(item, null);
        }
    }


    private void ChekZeroGroup()
    {
        if (_LGroups.Count() == 0)
        {
            _LGroups.Add(new LGroup(Guid.NewGuid(), 1, "NewGroup"));
            SaveCollections();
        }
    }

    public void RenumCollectionGroups()
    {
        int g = 1;
        foreach (var group in _LGroups)
        {
            group.GroupPos = g;
            RenumCollectionItems(group.GroupID);
            g++;
        }
        ChekZeroGroup();
        FullSortCollection();
    }

    private void RenumCollectionItems(Guid groupID)
    {
        int i = 1;
        foreach (var item in _LItems.Where(x => x.GroupID == groupID))
        {
            item.ItemPos = i++;
        }
    }

    private void FullSortCollection()
    {
        var sortedGroup = _LGroups.OrderBy(g => g.GroupPos).ToList();
        _LGroups.Clear();
        foreach (var g in sortedGroup)
            _LGroups.Add(g);

        var sortedItem = _LItems
                  .OrderBy(i => i.GroupID)
                  .ThenBy(i => i.ItemPos)
                  .ToList();
        _LItems.Clear();
        foreach (var i in sortedItem)
            _LItems.Add(i);
    }


    public async Task<string> GroupAddAsync()
    {
        if (_Owner == null) return string.Empty;
        var dialog = new InputDialog("Enter Group Name", "Group Name:", "NewGroup");
        var result = await dialog.ShowDialogAsync(_Owner);
        if (result.IsOk)
        {
            var newGroup = new LGroup(Guid.NewGuid(), 999, result.Text ?? "NewGroup");
            _LGroups.Add(newGroup);
            RenumCollectionGroups();
            SaveCollections();
            return newGroup.GroupID.ToString();
        }
        return string.Empty;
    }

    public async Task GroupEditAsync(LGroup group)
    {
        var dialog = new InputDialog("Enter Group Name", "Group Name:", group.GroupName);
        var result = await dialog.ShowDialogAsync(_Owner!);
        if (result.IsOk)
        {
            group.GroupName = result.Text ?? "NewGroup";
            RenumCollectionGroups();
            SaveCollections();
        }
    }

    public void GroupDelete(LGroup group)
    {
        var GroupToRemove = _LGroups.FirstOrDefault(g => g.GroupID == group.GroupID);
        if (GroupToRemove != null) _LGroups.Remove(GroupToRemove); // delete Group
        foreach (var item in _LItems.Where(x => x.GroupID == group.GroupID).ToList()) //delete items
            _LItems.Remove(item);
        RenumCollectionGroups();
        SaveCollections();
    }

    public async Task AddNewItemAsync(Window owner, LGroup group)
    {
        var dialog = new ItemDialog();
        var result = await dialog.ShowDialog<string?>(owner);
        if (result != null)
        {
            Guid newLItemGuid = Guid.NewGuid();
            _LItems.Add(new LItem(
                groupID: group.GroupID,
                itemID: newLItemGuid,
                itemPos: 999,
                itemName: dialog.itemName.Text ?? "Empty description",
                itemCommand1: dialog.itemCommand1.Text ?? "echo empty command",
                anyKeyClose: dialog.anyKeyClose.IsChecked ?? true
            ));
            RenumCollectionGroups();
            SaveCollections();
        }
    }

    public async Task ItemEditAsync(Window owner, LItem item)
    {
        var dialog = new ItemDialog();
        dialog.itemName.Text = item.ItemName;
        dialog.itemCommand1.Text = item.ItemCommand1;
        dialog.anyKeyClose.IsChecked = item.AnyKeyClose;
        var result = await dialog.ShowDialog<string?>(owner);
        if (result != null)
        {
            item.ItemName = dialog.itemName.Text;
            item.ItemCommand1 = dialog.itemCommand1.Text;
            item.AnyKeyClose = dialog.anyKeyClose.IsChecked ?? true;
            SaveCollections();
        }
    }

    public void ItemDelete(LItem item)
    {
        var itemToRemove = _LItems.FirstOrDefault(g => g.ItemID == item.ItemID);
        if (itemToRemove != null) _LItems.Remove(itemToRemove);
        RenumCollectionGroups();
        SaveCollections();
    }

    /// <summary>
    /// Save collection by user
    /// </summary>
    public void SaveCollections()
    {
        _ = new StorageService(_LGroups, _LItems).SaveCollectionsToFileAsync();
    }


    public void MoveGroupUp(LGroup selectedGroup)
    {
        if (selectedGroup == null) return;
        int index = _LGroups.IndexOf(selectedGroup);
        if (index <= 0) return;
        var previous = _LGroups[index - 1];
        (selectedGroup.GroupPos, previous.GroupPos) = (previous.GroupPos, selectedGroup.GroupPos);
        _LGroups.Move(index, index - 1);
        RenumCollectionGroups();
        SaveCollections();
    }
    public void MoveGroupDown(LGroup selectedGroup)
    {
        if (selectedGroup == null) return;
        int index = _LGroups.IndexOf(selectedGroup);
        if (index < 0 || index >= _LGroups.Count - 1) return;
        var next = _LGroups[index + 1];
        (selectedGroup.GroupPos, next.GroupPos) = (next.GroupPos, selectedGroup.GroupPos);
        _LGroups.Move(index, index + 1);
        RenumCollectionGroups();
        SaveCollections();
    }


    public void MoveItemUp(LItem selectedItem)
    {
        if (selectedItem == null) return;
        int index = _LItems.IndexOf(selectedItem);
        if (index <= 0) return;
        var previous = _LItems[index - 1];
        (selectedItem.ItemPos, previous.ItemPos) = (previous.ItemPos, selectedItem.ItemPos);
        _LItems.Move(index, index - 1);
        RenumCollectionGroups();
        SaveCollections();
    }


    public void MoveItemDown(LItem selectedItem)
    {
        if (selectedItem == null) return;
        int index = _LItems.IndexOf(selectedItem);
        if (index < 0 || index >= _LItems.Count - 1) return;
        var next = _LItems[index + 1];
        (selectedItem.ItemPos, next.ItemPos) = (next.ItemPos, selectedItem.ItemPos);
        _LItems.Move(index, index + 1);
        RenumCollectionGroups();
        SaveCollections();
    }

    public void Dispose()
    {

    }
}
