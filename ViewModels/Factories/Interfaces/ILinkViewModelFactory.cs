using Pileolinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels.Factories.Interfaces
{
    public interface ILinkViewModelFactory
    {
        LinkViewModel GetLinkViewModel(Link link, bool hasOpenParent);
    }
}
