using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    public class RequestDeleteLinkEventArgs : AlertEventArgs
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
