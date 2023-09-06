using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.Core.Models
{
    public class Node
    {
        public ObservableCollection<Node>? SubNodes { get; set; }
        public string Title { get; set; }
    }
}
