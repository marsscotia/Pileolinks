using Pileolinks.Models;
using Pileolinks.Services.Interfaces;
using Pileolinks.ViewModels.Factories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels.Factories
{
    public class LinkViewModelFactory : ILinkViewModelFactory
    {
        private readonly IDataService dataService;

        public LinkViewModelFactory(IDataService dataService)
        {
            this.dataService = dataService;
        }

        public LinkViewModel GetLinkViewModel(Link link)
        {
            return new LinkViewModel(dataService, link);
        }
    }
}
