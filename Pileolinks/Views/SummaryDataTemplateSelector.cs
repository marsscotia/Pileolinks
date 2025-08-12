using Pileolinks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Views
{
    public class SummaryDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DirectoryDataTemplate {get; set;}
        public DataTemplate LinkDataTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item is DirectoryViewModel ? DirectoryDataTemplate : LinkDataTemplate;
        }
    }
}
