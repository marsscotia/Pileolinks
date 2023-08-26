using Pileolinks.Services.Interfaces;

namespace Pileolinks.Services
{
    internal class EssentialsService : IEssentialsService
    {
        public async Task SetClipboardTextAsync(string text)
        {
            await Clipboard.Default.SetTextAsync(text);
        }
    }
}
