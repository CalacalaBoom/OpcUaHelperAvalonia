using Opc.Ua;
using System;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.Core.Models
{
    public class Node
    {
        public ObservableCollection<Node>? SubNodes { get; set; }=new ObservableCollection<Node>();
        public string Title { get; set; }
        public NodeId NodeId { get; set; }
        public Bitmap? Image { get; set; } 
    }
}
