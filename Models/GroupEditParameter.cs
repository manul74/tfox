using System.Text.RegularExpressions;
using Avalonia.Controls;

namespace tfox.ViewModels;

public partial class GroupEditParameter
{
    public Window Owner { get; set; }
    public Group SelectedGroup { get; set; }


    public GroupEditParameter(Window owner, Group group)
    {
        Owner = owner;
        SelectedGroup = group;
    }
}
