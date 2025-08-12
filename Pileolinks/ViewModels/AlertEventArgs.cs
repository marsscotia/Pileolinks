using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    public class AlertEventArgs
    {
        public string Message { get; set; } = "Message";
        public string Title { get; set; } = "Title";
        public string ConfirmButton { get; set; } = "OK";
        public string CancelButton { get; set; } = "Cancel";
    }
}
