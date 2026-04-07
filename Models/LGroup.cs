
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace tfox.ViewModels;

public partial class LGroup : ObservableObject
{
    [ObservableProperty]
    private Guid groupID;

    [ObservableProperty]
    private int groupPos;

    [ObservableProperty]
    private string groupName = "";

    // Конструктор
    public LGroup(Guid groupID, int groupPos, string groupName)
    {
        if (groupName == "") groupName = "NewGroup";
        this.groupID = groupID;
        this.groupPos = groupPos;
        this.groupName = groupName;
    }
}
    