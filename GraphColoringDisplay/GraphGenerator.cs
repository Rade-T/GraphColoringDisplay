using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoringDisplay
{
    public class GraphGenerator
    {
        private Random random = new Random();

        public GraphGenerator()
        {

        }

        public Graph GenerateGraph(int numberOfVertices, double edgeDensity)
        {
            Graph graph = new Graph(numberOfVertices);
            int maxEdgeNumber = (numberOfVertices * (numberOfVertices - 1)) / 2;
            int edgeNumber = (int) (maxEdgeNumber * edgeDensity);
            int[][] arrayRepresentation = graph.toDoubleArray();
            for (int i = 1; i < arrayRepresentation.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (!(random.NextDouble() <= (1.0 - edgeDensity)))
                    {
                        graph.AddEdge(i, j);
                    }   
                }
            }
            Console.WriteLine(edgeNumber);
            return graph;
        }
    }
}
