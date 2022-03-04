using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Components.Tree
{
    public interface ITreeViewModel
    {
        ObservableCollection<ITreeItemViewModel> Items { get; }
    }
}
