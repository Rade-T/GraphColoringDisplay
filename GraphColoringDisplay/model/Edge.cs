using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GraphColoringDisplay.model
{
    public class Edge
    {
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public Line EdgeLine { get; set; }

        public Edge()
        {

        }

        public Edge(Node node1, Node node2, Line line)
        {
            Node1 = node1;
            Node2 = node2;
            EdgeLine = line;
        }
    }
}
