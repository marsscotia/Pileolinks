using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Services.Interfaces
{
    public interface IEssentialsService
    {
        Task SetClipboardTextAsync(string text);
    }
}
