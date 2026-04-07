using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using tfox.Common;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;


namespace tfox.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public ObservableCollection<LGroup> LGroups { get; } = [];
        public ObservableCollection<LItem> LItems { get; } = [];

        public ReactiveCommand<Window?, Unit> QuitCommand { get; }

        public ObservableCollection<LItem> FilteredItems { get; } = [];

        public void UpdateFilteredItems(LGroup group)
        {
            FilteredItems.Clear();
            foreach (var item in LItems.Where(x => x.GroupID == group.GroupID))
            {
                FilteredItems.Add(item);
            }
        }

        public MainWindowViewModel()
        {
            QuitCommand = ReactiveCommand.Create<Window?>(QuitAction);
        }

        private void QuitAction(Window? window)
        {
            window?.Close();
        }

        public void OpenSavedCollectionsFolder(Window? window)
        {
            _ = new LinuxCommandService().OpenSavedCollectionsFolderAsync();
        }


        
    }
}