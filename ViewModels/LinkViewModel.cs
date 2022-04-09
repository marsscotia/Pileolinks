using Pileolinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    public class LinkViewModel : ItemViewModel
    {
        protected readonly Link link;

        public LinkViewModel(Link link) : base(link)
        {
            this.link = link;
        }

    }
}
