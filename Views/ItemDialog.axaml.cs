using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace tfox;

public partial class ItemDialog : Window
{
    public ItemDialog()
    {
        InitializeComponent();
        itemName.Focus();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        itemName.Focus();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) 
            Ok_Click(null, null);
    }

    private void Ok_Click(object? sender, RoutedEventArgs? e)
    {
        Close("OK");
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }


}