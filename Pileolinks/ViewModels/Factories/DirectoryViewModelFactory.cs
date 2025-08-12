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
    public class DirectoryViewModelFactory : ILinkDirectoryViewModelFactory
    {
        private readonly IDataService dataService;
        private readonly IEssentialsService essentialsService;

        public DirectoryViewModelFactory(IDataService dataService, IEssentialsService essentialsService) 
        {
            this.dataService = dataService;
            this.essentialsService = essentialsService;
        }

        public DirectoryViewModel GetDirectoryViewModel(LinkDirectory directory)
        {
            return new DirectoryViewModel(dataService, essentialsService, directory);
        }
    }
}
