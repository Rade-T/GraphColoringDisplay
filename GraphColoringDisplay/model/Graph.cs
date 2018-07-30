using GraphColoringDisplay.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphColoringDisplay
{
    public class Graph
    {
        private int numberVertices;
        public int NumberVertices
        {
            get { return numberVertices; }
            set { numberVertices = value; }
        }

        private List<int>[] adjacencyList;
        public List<int>[] AdjacencyList
        {
            get { return adjacencyList; }
            set { adjacencyList = value; }
        }
        public List<Node> Nodes { get; set; }
        int V;
        int[] color;

        public Graph()
        {

        }

        public Graph(int numberOfVertices)
        {
            numberVertices = numberOfVertices;
            adjacencyList = new List<int>[numberOfVertices];
            Nodes = new List<Node>();
            for (int i = 0;i < numberOfVertices;i++)
            {
                adjacencyList[i] = new List<int>();
                Nodes.Add(new Node(i));
            }
        }

        public Graph Clone()
        {
            Graph clone = new Graph();
            clone.NumberVertices = numberVertices;
            clone.AdjacencyList = new List<int>[numberVertices];
            clone.Nodes = new List<Node>();
            for (int i = 0;i < numberVertices;i++)
            {
                List<int> adjacencyItem = new List<int>();
                foreach (var vertex in adjacencyList[i])
                {
                    adjacencyItem.Add(vertex);
                }
                clone.AdjacencyList[i] = adjacencyItem;
            }
            foreach (var node in Nodes)
            {
                clone.Nodes.Add(node);
            }
            return clone;
        }

        public int[] GetColors()
        {
            int[] colors = new int[numberVertices];
            for (int i = 0; i < Nodes.Count; i++)
            {
                colors[i] = Nodes[i].Color;
            }
            return colors;
        }

        public int[][] toDoubleArray()
        {
            int[][] representation = new int[numberVertices][];
            for (int i = 0; i < representation.Length; i++)
            {
                representation[i] = new int[numberVertices];
            }
            for (int i = 0; i < numberVertices; i++)
            {
                for (int j = 0; j < numberVertices; j++)
                {
                    representation[i][j] = 0;
                }
            }
            for (int i = 0; i < adjacencyList.Length; i++)
            {
                foreach (var vertex in adjacencyList[i])
                {
                    representation[i][vertex] = 1;
                }
            }
            return representation;
        }

        public void AddEdge(int x, int y)
        {
            adjacencyList[x].Add(y);
            adjacencyList[y].Add(x);
            Nodes[x].Edges.Add(Nodes[y]);
            Nodes[y].Edges.Add(Nodes[x]);
        }

        public void GreedyColoring()
        {
            int[] result = new int[numberVertices];
            //Array.Fill(result, -1);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = -1;
            }
            result[0] = 0;

            bool[] Available = new bool[numberVertices];
            //Array.Fill(Available, true);
            for (int i = 0; i < Available.Length; i++)
            {
                Available[i] = true;
            }

            for (int i = 1; i < numberVertices; i++)
            {
                foreach (var adjacency in adjacencyList[i])
                {
                    if (result[adjacency] != -1) { Available[result[adjacency]] = false; }
                }

                int color;
                for (color = 0;color < numberVertices;color++)
                {
                    if (Available[color]) { break; }
                }
                result[i] = color;
                //Array.Fill(Available, true);
                for (int i1 = 0; i1 < Available.Length; i1++)
                {
                    Available[i1] = true;
                }
            }

            for (int i = 0; i < numberVertices; i++)
            {
                //Console.WriteLine("Vertex " + i + " --->  Color "
                //                + result[i]);
                Nodes[i].Color = result[i];
            }
        }

        public bool GeneticColoring(int numCols)
        {
            double probability = 0.25;
            int n = Nodes.Count;
            int generations = 100 * n;
            int population = n;
            int restart = 0;
            List<Node> solution = null;
            //int numCols = 4;
            Random random = new Random();

            bool found = EHC(
                probability,
                generations,
                numCols,
                population,
                Nodes,
                ref restart,
                ref solution,
                ref random);

            if (found)
            {
                for (int i = 0; i < solution.Count; i++)
                    Nodes[i] = solution[i];
            }
            return found;
        }

        private bool violation(Node n1, Node n2, ref int number)
        {
            bool cv = false;

            if (n1.Edges != null)
            {
                bool found = false;

                for (int i = 0; !found && i < n1.Edges.Count; i++)
                {
                    found = n1.Edges[i] == n2;
                }

                if (found)
                    cv = n1.Color == n2.Color;
            }

            if (n2.Edges != null)
            {
                bool found = false;

                for (int i = 0; !found && i < n2.Edges.Count; i++)
                {
                    found = n2.Edges[i] == n1;
                }

                if (found)
                    cv = n1.Color == n2.Color;
            }

            if (cv)
                number++;

            return cv;
        }

        private int Fitness(List<Node> G)
        {
            int fitness = 0;

            for (int i = 0; i < G.Count - 1; i++)
                for (int j = i + 1; j < G.Count; j++)
                    violation(G[i], G[j], ref fitness);

            return fitness;
        }

        private bool EHC(
            double probability,
            int generations,
            int numberColors,
            int population,
            List<Node> G,
            ref int restart,
            ref List<Node> solution,
            ref Random random)
        {
            const int maxRestart = 4;
            int n = G.Count;
            int[] fitness = new int[population];
            List<Node>[] chromosome = new List<Node>[population];

            for (int i = 0; i < population; i++)
            {
                chromosome[i] = new List<Node>();

                for (int j = 0; j < G.Count; j++)
                {
                    Node node = G[j];

                    node.Color = random.Next(numberColors);
                    chromosome[i].Add(node);
                }

                fitness[i] = Fitness(chromosome[i]);

                if (fitness[i] == 0)
                {
                    solution = chromosome[i];
                    return true;
                }
            }

            int g = 0;

            while (g < generations)
            {
                int parent0 = random.Next(population);
                int parent1 = random.Next(population);

                while (parent0 == parent1)
                {
                    parent0 = random.Next(population);
                    parent1 = random.Next(population);
                }

                int index = fitness[parent0] < fitness[parent1]
                    ? parent0 : parent1;

                List<Node> child = chromosome[index];

                for (int i = 0; i < child.Count; i++)
                    if (random.NextDouble() < probability)
                        child[i].Color = random.Next(numberColors);

                int childFitness = Fitness(child);

                if (childFitness == 0)
                {
                    solution = child;
                    return true;
                }

                int maxFitness = fitness[0];

                for (int i = 0; i < population; i++)
                    if (fitness[i] > maxFitness)
                        maxFitness = fitness[i];

                List<int> maxIndex = new List<int>();
                for (int i = 0; i < population; i++)
                    if (fitness[i] == maxFitness)
                        maxIndex.Add(i);

                index = random.Next(maxIndex.Count);

                chromosome[index] = child;
                fitness[index] = childFitness;
                g++;

                bool same = true;
                int min = fitness[0];

                for (int i = 1; same && i < population; i++)
                    same = min == fitness[i];

                if (same)
                {
                    restart++;

                    if (restart < maxRestart)
                        return EHC(
                            probability,
                            generations,
                            numberColors,
                            population + 2,
                            G,
                            ref restart,
                            ref solution,
                            ref random);
                }
            }

            restart++;

            if (restart < maxRestart)
                return EHC(
                    probability,
                    generations,
                    numberColors,
                    population + 2,
                    G,
                    ref restart,
                    ref solution,
                    ref random);

            return false;
        }

        public bool BacktrackColoring(int colorNumber)
        {
            bool result = false;
            int[][] graph = this.toDoubleArray();
            if (graphColoring(graph, colorNumber))
            {
                ApplyColors(color);
                result = true;
            }
            return result;
        }

        public bool isSafe(int v, int[][] graph, int[] color, int c)
        {
            for (int i = 0; i < V; i++)
            {
                if (graph[v][i] == 1 && c == color[i])
                    return false;
            }
            return true;
        }

        public bool graphColoringUtil(int[][] graph, int m, int[] color, int v)
        {
            if (v == V)
            {
                return true;
            }

            for (int i = 1; i <= m; i++)
            {
                if (isSafe(v, graph, color, i))
                {
                    color[v] = i;

                    if (graphColoringUtil(graph, m, color, v + 1))
                    {
                        return true;
                    }
                    color[v] = 0;
                }
            }

            return false;
        }

        public bool graphColoring(int[][] graph, int m)
        {
            V = graph.Length;
            color = new int[V];
            for (int i = 0; i < V; i++)
            {
                color[i] = 0;
            }
            if (!graphColoringUtil(graph, m, color, 0))
            {
                Console.WriteLine("Solution does not exist");
                return false;
            }
            //printSolution(color);
            return true;
        }

        public void printSolution(int[] color)
        {
            Console.WriteLine("Solution Exists: Following are the assigned colors");
            for (int i = 0; i < V; i++)
                Console.WriteLine(" " + color[i] + " ");
            Console.WriteLine();
        }

        public void ApplyColors(int[] colors)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Nodes[i].Color = colors[i];
            }
        }
    }
}
