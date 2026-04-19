using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Interactivity;

namespace tfox.Common
{
    public class InputDialogResult
    {
        public bool IsOk { get; set; }

        // null if Cancel
        public string? Text { get; set; }
    }

    public class InputDialog : Window
    {
        private readonly TextBox _textBox;
        private readonly TaskCompletionSource<InputDialogResult> _tcs;

        public InputDialog(string title, string message, string defaultText = "")
        {
            Title = title;
            Width = 400;
            Height = 150;
            CanResize = false; 
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            _tcs = new TaskCompletionSource<InputDialogResult>();

            _textBox = new TextBox
            {
                Text = defaultText ?? string.Empty,
                Margin = new Thickness(0, 10, 0, 10)
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Margin = new Thickness(5),
                IsDefault = true // Enter
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Margin = new Thickness(5),
                IsCancel = true // Esc
            };

            okButton.Click += OkClicked;
            cancelButton.Click += CancelClicked;

            Content = new StackPanel
            {
                Margin = new Thickness(15),
                Children =
                {
                    new TextBlock
                    {
                        Text = message
                    },
                    _textBox,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Children =
                        {
                            okButton,
                            cancelButton
                        }
                    }
                }
            };

            // focus on open.
            Opened += (_, __) =>
            {
                _textBox.Focus();
                _textBox.CaretIndex = _textBox.Text?.Length ?? 0;
            };
        }

        private void OkClicked(object? sender, RoutedEventArgs e)
        {
            _tcs.TrySetResult(new InputDialogResult
            {
                IsOk = true,
                Text = _textBox.Text
            });

            Close();
        }

        private void CancelClicked(object? sender, RoutedEventArgs e)
        {
            _tcs.TrySetResult(new InputDialogResult
            {
                IsOk = false,
                Text = null
            });

            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            // close without clicking OK or Cancel, treat as Cancel.
            if (!_tcs.Task.IsCompleted)
            {
                _tcs.TrySetResult(new InputDialogResult
                {
                    IsOk = false,
                    Text = null
                });
            }

            base.OnClosed(e);
        }

        public Task<InputDialogResult> ShowDialogAsync(Window owner)
        {
            _ = ShowDialog(owner);
            return _tcs.Task;
        }
    }
}