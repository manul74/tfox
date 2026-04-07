using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace tfox.ViewModels;

public partial class LItem : ObservableObject
{
    [ObservableProperty]
    private Guid groupID;          //Унікальний номер основного запису GUID

    [ObservableProperty]
    private Guid itemID;            //Унікальний номер запису GUID

    [ObservableProperty]
    private int itemPos;             //Позиція у списку

    [ObservableProperty]
    private string itemName = "";

    [ObservableProperty]
    private string itemCommand1 = "";

    [ObservableProperty]
    private Boolean anyKeyClose = true;

    public LItem(Guid groupID, Guid itemID, int itemPos, string itemName, string ItemCommand1, Boolean anyKeyClose)
    {
        if (itemName == "") itemName = "NewItem";
        {
            this.groupID = groupID;
            this.itemID = itemID;
            this.itemPos = itemPos;
            this.itemName = itemName;
            this.itemCommand1 = ItemCommand1;
            this.anyKeyClose = anyKeyClose;
        }
    }
}