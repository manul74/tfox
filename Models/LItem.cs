using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace tfox.ViewModels;

public partial class LItem : ObservableObject
{
    [ObservableProperty]
    private Guid groupID;          //Уникальный номер основной записи GUID

    [ObservableProperty]
    private Guid itemID;           //Уникальный номер запсис GUID

    [ObservableProperty]
    private int itemPos;            //Позиция в списке

    [ObservableProperty]
    private string itemName = "";   //Имя группы 

    [ObservableProperty]
    private string itemCommand1 = "";

    public LItem(Guid groupID,Guid itemID, int itemPos, string itemName,string ItemCommand1)
    {
        this.groupID = groupID;
        this.itemID = itemID;
        this.itemPos = itemPos;
        this.itemName = itemName;
        this.itemCommand1 = ItemCommand1;
    }

}