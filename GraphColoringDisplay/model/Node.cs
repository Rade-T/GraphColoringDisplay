using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GraphColoringDisplay.model
{
    public class Node
    {
        public int Id { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public int Color { get; set; }
        public Ellipse Circle { get; set; }
        public List<Node> Edges { get; set; }

        public Node(int id)
        {
            Id = id;
            Edges = new List<Node>();
        }
    }
}
