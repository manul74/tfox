using System.Diagnostics;
using System.Threading.Tasks;
using tfox.Common;
using tfox.ViewModels;

public partial class LinuxCommandService
{

    public LinuxCommandService()
    {
    }

    public async Task RunCommandAsync(LItem item)
    {
        if (item.AnyKeyClose == true)   //Any key to close the terminal after command execution
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "x-terminal-emulator",
                Arguments = $"-e bash -c \"trap 'echo Interrupted; exit 1' SIGINT; echo '$ {item.ItemCommand1}'; {item.ItemCommand1}; echo; read -n 1 -s -r -p 'Press any key to close...'\"",
                UseShellExecute = false
            });
        }
        else
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "x-terminal-emulator",
                Arguments = $"-e bash -c \"trap 'echo Interrupted; exit 1' SIGINT; echo '$ {item.ItemCommand1}'; {item.ItemCommand1}; echo; cd $HOME; exec bash\"",
                UseShellExecute = false
            });
        }
    }


    public async Task OpenSavedCollectionsFolderAsync()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "xdg-open",
            Arguments = StorageService.StorageFolder,
            UseShellExecute = false
        });
    }

}