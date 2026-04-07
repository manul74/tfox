using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Newtonsoft.Json;
using tfox.ViewModels;
using Avalonia.Platform.Storage;
using System.Linq;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace tfox.Common;

public class StorageService : IDisposable
{
    private ObservableCollection<LGroup> _LGroups { get; }
    private ObservableCollection<LItem> _LItems { get; }

    public static string StorageFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.tfox/";
    private string GroupsFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.tfox/Groups.json";
    private string ItemsFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.tfox/Items.json";

    public StorageService(ObservableCollection<LGroup> LGroups, ObservableCollection<LItem> LItems)
    {
        _LGroups = LGroups;
        _LItems = LItems;
    }

    public void LoadCollections()
    {
        if (!Directory.Exists(StorageFolder))
        {
            Directory.CreateDirectory(StorageFolder);
            SetCollectionDataDefault();
            _ = SaveCollectionsToFileAsync();
            return;
        }
        if (!File.Exists(GroupsFilePath)) //No file?
        {
            SetCollectionDataDefault();
            _ = SaveCollectionsToFileAsync();
            return;
        }
        LoadFromFile();
    }

    private async void LoadFromFile()
    {
        try
        {
            string groupsJson = File.ReadAllText(GroupsFilePath);
            var groups = JsonConvert.DeserializeObject<ObservableCollection<LGroup>>(groupsJson);
            if (groups != null)
            {
                _LGroups.Clear();
                foreach (var g in groups)
                    _LGroups.Add(g);
            }

            string itemsJson = File.ReadAllText(ItemsFilePath);
            var items = JsonConvert.DeserializeObject<ObservableCollection<LItem>>(itemsJson);

            if (items != null)
            {
                _LItems.Clear();
                foreach (var i in items)
                    _LItems.Add(i);
            }
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", $"{ex}", ButtonEnum.Ok);
            await box.ShowAsync();
            SetCollectionDataDefault();
        }
    }

    public async Task SaveCollectionsToFileAsync()
    {
        try
        {
            string GroupsJson = JsonConvert.SerializeObject(_LGroups, Formatting.Indented);
            File.WriteAllText(GroupsFilePath, GroupsJson);
            string ItemsJson = JsonConvert.SerializeObject(_LItems, Formatting.Indented);
            File.WriteAllText(ItemsFilePath, ItemsJson);
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", $"{ex}", ButtonEnum.Ok);
            await box.ShowAsync();
        }
    }


    public void SetCollectionDataDefault()
    {
        _LGroups.Clear();
        _LItems.Clear();
        Guid GroupID = Guid.NewGuid();
        _LGroups.Add(new LGroup(GroupID, 1, "(K)Ubuntu system"));
        _LItems.Add(new LItem(
           groupID: GroupID,
           itemID: Guid.NewGuid(),
           itemPos: 1,
           itemName: "Refresh package list && Install standard updates",
           ItemCommand1: "sudo apt update && sudo apt upgrade -y"
       ));
        _LItems.Add(new LItem(
           groupID: GroupID,
           itemID: Guid.NewGuid(),
           itemPos: 2,
           itemName: "Remove unnecessary files",
           ItemCommand1: "sudo apt autoremove"
       ));
        _LItems.Add(new LItem(
               groupID: GroupID,
               itemID: Guid.NewGuid(),
               itemPos: 3,
               itemName: "Clearing the APT cache",
               ItemCommand1: "sudo apt clean"
           ));
        GroupID = Guid.NewGuid();
        _LGroups.Add(new LGroup(GroupID, 2, "GRUB"));
        _LItems.Add(new LItem(
               groupID: GroupID,
               itemID: Guid.NewGuid(),
               itemPos: 1,
               itemName: "STEP 1: Open GRUB",
               ItemCommand1: "sudo nano /etc/default/grub"
           ));
        _LItems.Add(new LItem(
               groupID: GroupID,
               itemID: Guid.NewGuid(),
               itemPos: 2,
               itemName: "STEP 2: Update GRUB",
               ItemCommand1: "sudo update-grub"
           ));

    }


    public async Task ExportToFileAsync(Window? owner, Guid groupID)
    {
        var topLevel = TopLevel.GetTopLevel(owner);
        if (topLevel == null)
            return;
        LGroup? group = _LGroups.SingleOrDefault(g => g.GroupID == groupID);
        if (group == null)
            return;

        try
        {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                SuggestedFileName = $"{group.GroupName}.json",
                Title = "Export a node to a file",
            });
            if (file is not null)
            {
                var Items = _LItems.Where(g => g.GroupID == groupID).ToList();
                string ItemsJson = JsonConvert.SerializeObject(Items, Formatting.Indented);
                File.WriteAllText(file.Path.AbsolutePath, ItemsJson);
            }
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", $"{ex}", ButtonEnum.Ok);
            await box.ShowAsync();
        }
    }

    public async Task<string> ImportFromFileAsync(Window? owner)
    {
        var topLevel = TopLevel.GetTopLevel(owner);
        if (topLevel == null)
            return String.Empty;
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            FileTypeFilter = new[]
            {
                new FilePickerFileType("json") { Patterns = new[] { "*.json" } }
            },
            Title = "Import a node from a file",
            AllowMultiple = false
        });
        if (files.Count == 1)
        {
            await AddNewGroupFromFileAsync(files[0].Path.ToString(), files[0].Name.ToString());
        }
        return String.Empty;
    }


    public async Task<string> AddNewGroupFromFileAsync(string jsonFilePath, string jsonFileName)
    {
        string fileName = jsonFileName.Replace(".json", "");
        try
        {
            var GroupID = Guid.NewGuid();
            int maxGroupPos = _LGroups.Any() ? _LGroups.Max(g => g.GroupPos) : 0; //get max GroupPos
            _LGroups.Add(new LGroup(GroupID, maxGroupPos + 1, fileName));
            string Json = File.ReadAllText(jsonFilePath.Replace("file://", ""));
            List<LItem> JsonItems = JsonConvert.DeserializeObject<List<LItem>>(Json) ?? [];
            if (JsonItems != null)
            {
                int i = 0;
                foreach (var itm in JsonItems)
                {
                    itm.GroupID = GroupID;
                    itm.ItemID = new Guid();
                    itm.ItemPos = i++;
                    _LItems.Add(itm);
                }
            }
            return GroupID.ToString();
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", $"{ex}", ButtonEnum.Ok);
            await box.ShowAsync();
            return string.Empty;
        }
    }


    public void Dispose()
    {

    }

}
