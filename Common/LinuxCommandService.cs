using System.Diagnostics;
using System.Threading.Tasks;
using tfox.Common;
using tfox.ViewModels;

public partial class LinuxCommandService
{

    public LinuxCommandService()
    {
    }

    public void RunCommandAsync(LItem item)
    {
        Task.Run(() =>
        {
            var suffix = item.AnyKeyClose ? "read -n 1 -s -r -p 'Press any key to close...'" : "cd $HOME; exec bash";
            Process.Start(new ProcessStartInfo
            {
                FileName = "x-terminal-emulator",
                Arguments = $"-e bash -c \"trap 'echo Interrupted; exit 1' SIGINT; echo '$ {item.ItemCommand1}'; {item.ItemCommand1}; echo; {suffix}\"",
                UseShellExecute = false
            });
        });
    }


    public void OpenSavedCollectionsFolder()
    {
        Task.Run(() =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "xdg-open",
                Arguments = StorageService.StorageFolder,
                UseShellExecute = false
            });
        });
    }

}