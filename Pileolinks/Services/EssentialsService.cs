using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Pileolinks.Services.Interfaces;

namespace Pileolinks.Services
{
    internal class EssentialsService : IEssentialsService
    {
        public async Task SetClipboardTextAsync(string text)
        {
            await Clipboard.Default.SetTextAsync(text);
        }

        public async Task ShowToastAsync(string toastText, ToastDuration toastDuration = ToastDuration.Short, CancellationTokenSource cancellationTokenSource = null)
        {
            var toast = Toast.Make(toastText, toastDuration);

            if (cancellationTokenSource is null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
