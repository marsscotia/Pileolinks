using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    public class DialogRequestedEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Confirm { get; set; }
        public string Cancel { get; set; }

        public DialogRequestedEventArgs(string title, string message, string confirm, string cancel = null)
        {
            Title = title;
            Message = message;
            Confirm = confirm;
            Cancel = cancel;
        }
    }
}
