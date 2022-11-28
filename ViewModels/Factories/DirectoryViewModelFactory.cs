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

        public DirectoryViewModelFactory(IDataService dataService) 
        {
            this.dataService = dataService;
        }

        public DirectoryViewModel GetDirectoryViewModel(LinkDirectory directory)
        {
            return new DirectoryViewModel(dataService, directory);
        }
    }
}
