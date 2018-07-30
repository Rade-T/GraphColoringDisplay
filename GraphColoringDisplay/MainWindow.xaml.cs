using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphColoringDisplay.model;
using Microsoft.Win32;

namespace GraphColoringDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color[] colors = {
            Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.White,
            Colors.Purple, Colors.Orange, Colors.Black, Colors.Brown, Colors.Pink,
            Colors.Cyan, Colors.Gray, Colors.Magenta, Colors.Linen, Colors.LimeGreen
        };
        private Random random = new Random();
        private bool canMove = false;
        private Graph graph = null;
        private List<Edge> edges = null;
        private List<Ellipse> circles = null;
        private int[] greedyColors;
        private int[] geneticColors;
        private int[] backtrackColors;
        private int greedyColorNumber = 0;
        private int geneticColorNumber = 0;
        int backtrackColorNumber = 0;

        public MainWindow()
        {
            InitializeComponent();
            edges = new List<Edge>();
            circles = new List<Ellipse>();
            lblCursorPosition.Text = "Vertices: " + circles.Count + ", Edges: " + edges.Count;
        }

        public void DrawGraph(Graph graph)
        {
            DrawNodeArray(graph.Nodes);
            DrawEdges(graph.AdjacencyList, graph.Nodes);
        }

        public void DrawEdges(List<int>[] adjacencyList, List<Node> nodes)
        {
            int[][] arrayRepresentation = graph.toDoubleArray();
            for (int i = 1; i < arrayRepresentation.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (arrayRepresentation[i][j] == 1)
                    {
                        Node start = nodes[i];
                        Node end = nodes[j];
                        int x1 = (int)(double)(start.Circle.GetValue(Canvas.LeftProperty));
                        int y1 = (int)(double)(start.Circle.GetValue(Canvas.TopProperty));
                        int x2 = (int)(double)(end.Circle.GetValue(Canvas.LeftProperty));
                        int y2 = (int)(double)(end.Circle.GetValue(Canvas.TopProperty));
                        Line line = new Line();
                        line.X1 = (int)x1 + 25;
                        line.X2 = (int)x2 + 25;
                        line.Y1 = (int)y1 + 25;
                        line.Y2 = (int)y2 + 25;
                        line.Stroke = System.Windows.Media.Brushes.Black;
                        line.StrokeThickness = 1;
                        line.SetValue(Panel.ZIndexProperty, 2);
                        Edge edge = new Edge(start, end, line);
                        edges.Add(edge);
                        myCanvas.Children.Add(line);
                    }
                }
            }
        }

        public void DrawNodeEdges(Node node)
        {
            foreach (var edge in edges)
            {
                if (edge.Node1 == node || edge.Node2 == node)
                {
                    int x1 = (int)(double)(edge.Node1.Circle.GetValue(Canvas.LeftProperty));
                    int y1 = (int)(double)(edge.Node1.Circle.GetValue(Canvas.TopProperty));
                    int x2 = (int)(double)(edge.Node2.Circle.GetValue(Canvas.LeftProperty));
                    int y2 = (int)(double)(edge.Node2.Circle.GetValue(Canvas.TopProperty));
                    Line line = new Line();
                    line.X1 = (int)x1 + 25;
                    line.X2 = (int)x2 + 25;
                    line.Y1 = (int)y1 + 25;
                    line.Y2 = (int)y2 + 25;
                    line.Stroke = System.Windows.Media.Brushes.Black;
                    line.StrokeThickness = 1;
                    line.SetValue(Panel.ZIndexProperty, 2);
                    edge.EdgeLine = line;
                    myCanvas.Children.Add(edge.EdgeLine);
                }
            }
        }

        public void DeleteEdges()
        {
            foreach (var edge in edges)
            {
                myCanvas.Children.Remove(edge.EdgeLine);
            }
        }

        public void DeleteNodeEdges(Node node)
        {
            foreach (var edge in edges)
            {
                if (edge.Node1 == node || edge.Node2 == node)
                {
                    myCanvas.Children.Remove(edge.EdgeLine);
                }
            }
        }

        public void DrawNodeArray(List<Node> nodes)
        {
            double beginLeft = 100;
            double beginTop = 80;
            int counter = 0;
            double stopingPointDouble = Math.Sqrt((double)nodes.Count);
            int stopingPoint = (int)stopingPointDouble + 1;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (counter == stopingPoint)
                {
                    beginTop += 90;
                    beginLeft = 120;
                    counter = 0;
                }
                nodes[i].Top = beginTop + random.Next(80);
                nodes[i].Left = beginLeft + random.Next(80);
                DrawNode(nodes[i]);
                beginLeft += 100;
                counter++;
            }
        }

        public void DrawNode(Node node)
        {
            Ellipse circle = new Ellipse();
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = colors[node.Color];
            circle.Fill = solidColorBrush;
            circle.StrokeThickness = 2;
            circle.Stroke = Brushes.Black;
            circle.Width = 50;
            circle.Height = 50;
            circle.SetValue(Panel.ZIndexProperty, 1);
            circle.SetValue(Canvas.LeftProperty, node.Left);
            circle.SetValue(Canvas.TopProperty, node.Top);
            circle.MouseLeftButtonDown += new MouseButtonEventHandler(CircleMouseDown);
            circle.MouseLeftButtonUp += new MouseButtonEventHandler(CircleMouseUp);
            circle.MouseMove += new MouseEventHandler(CircleMouseMove);
            node.Circle = circle;
            circles.Add(circle);
            myCanvas.Children.Add(circle);
        }

        public void CircleMouseMove(object sender, MouseEventArgs e)
        {
            if (canMove && System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Node clicked = null;
                Ellipse circle = (Ellipse)sender;
                foreach (var node in graph.Nodes)
                {
                    if (node.Circle == circle)
                    {
                        clicked = node;
                    }
                }
                Point mouseCoordinates = e.GetPosition(myCanvas);
                int mouseX = (int)mouseCoordinates.X;
                int mouseY = (int)mouseCoordinates.Y;
                circle.SetValue(Canvas.LeftProperty, (double)mouseX - 25.0);
                circle.SetValue(Canvas.TopProperty, (double)mouseY - 25.0);
                clicked.Circle = circle;
                DeleteNodeEdges(clicked);
                DrawNodeEdges(clicked);
            }
        }

        public void CircleMouseDown(object sender, MouseButtonEventArgs e)
        {
            canMove = true;
        }

        public void CircleMouseUp(object sender, MouseButtonEventArgs e)
        {
            canMove = false;
        }

        public void SaveGraph(Graph graph, string filename)
        {
            StreamWriter writer = new StreamWriter(filename);
            writer.WriteLine(graph.NumberVertices);
            for (int i = 0; i < graph.AdjacencyList.Length; i++)
            {
                foreach (var vertex in graph.AdjacencyList[i])
                {
                    writer.WriteLine(i + "," + vertex);
                }
            }
            writer.Close();
        }

        public Graph LoadGraph(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            int numberVertex = Convert.ToInt32(reader.ReadLine());
            Graph graph = new Graph(numberVertex);
            while (reader.Peek() != -1)
            {
                string edge = reader.ReadLine();
                string[] vertices = edge.Split(',');
                int x = Convert.ToInt32(vertices[0]);
                int y = Convert.ToInt32(vertices[1]);
                graph.AdjacencyList[x].Add(y);
            }
            reader.Close();
            return graph;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            GenerateGraphWindow w = new GenerateGraphWindow();
            if (w.ShowDialog().Equals(true))
            {
                if (circles != null)
                {
                    foreach (var circle in circles)
                    {
                        myCanvas.Children.Remove(circle);
                    }
                }
                circles = new List<Ellipse>();
                if (edges != null)
                {
                    foreach (var edge in edges)
                    {
                        myCanvas.Children.Remove(edge.EdgeLine);
                    }
                }
                edges = new List<Edge>();

                int numberVertices = w.NumberVertices;
                double edgeDensity = w.EdgeDensity;

                GraphGenerator graphGenerator = new GraphGenerator();
                graph = graphGenerator.GenerateGraph(numberVertices, edgeDensity);
                graph.GreedyColoring();
                greedyColors = graph.GetColors();
                
                greedyColorNumber = 1;
                for (int i = 0; i < greedyColors.Length; i++)
                {
                    if (greedyColorNumber == greedyColors[i]) greedyColorNumber++;
                }

                geneticColorNumber = 1;
                bool foundGenetic = false;
                while (!foundGenetic)
                {
                    if (graph.GeneticColoring(geneticColorNumber))
                    {
                        Console.WriteLine("Genetic solution found");
                        foundGenetic = true;
                    }
                    else
                    {
                        Console.WriteLine("Genetic solution not found");
                        graph.GeneticColoring(++geneticColorNumber);
                    }
                }               
                geneticColors = graph.GetColors();

                backtrackColorNumber = 1;
                bool foundBacktrack = false;
                while (!foundBacktrack)
                {
                    if (graph.BacktrackColoring(backtrackColorNumber))
                    {
                        Console.WriteLine("Backtrack solution found");
                        foundBacktrack = true;
                    }
                    else
                    {
                        Console.WriteLine("Backtrack solution not found");
                        graph.GeneticColoring(++backtrackColorNumber);
                    }
                }
                backtrackColors = graph.GetColors();

                graph.ApplyColors(greedyColors);
                Console.WriteLine("Greedy colors: " + greedyColorNumber);
                Console.WriteLine("Genetic colors: " + geneticColorNumber);
                Console.WriteLine("Backtrack colors: " + backtrackColorNumber);
                DrawGraph(graph);
                lblCursorPosition.Text = "Vertices: " + circles.Count + ", Edges: " + edges.Count + ", Colors: " + greedyColorNumber;
            }
        }

        private void LoadGraph_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine(dialog.FileName);
                graph = LoadGraph(dialog.FileName);
                if (circles != null)
                {
                    foreach (var circle in circles)
                    {
                        myCanvas.Children.Remove(circle);
                    }
                }
                circles = new List<Ellipse>();
                if (edges != null)
                {
                    foreach (var edge in edges)
                    {
                        myCanvas.Children.Remove(edge.EdgeLine);
                    }
                }
                edges = new List<Edge>();

                //int numberVertices = w.NumberVertices;
                //double edgeDensity = w.EdgeDensity;

                //GraphGenerator graphGenerator = new GraphGenerator();
                //graph = graphGenerator.GenerateGraph(numberVertices, edgeDensity);
                graph.GreedyColoring();
                greedyColors = graph.GetColors();

                greedyColorNumber = 1;
                for (int i = 0; i < greedyColors.Length; i++)
                {
                    if (greedyColorNumber == greedyColors[i]) greedyColorNumber++;
                }

                geneticColorNumber = 1;
                bool foundGenetic = false;
                while (!foundGenetic)
                {
                    if (graph.GeneticColoring(geneticColorNumber))
                    {
                        Console.WriteLine("Genetic solution found");
                        foundGenetic = true;
                    }
                    else
                    {
                        Console.WriteLine("Genetic solution not found");
                        graph.GeneticColoring(++geneticColorNumber);
                    }
                }
                geneticColors = graph.GetColors();

                backtrackColorNumber = 1;
                bool foundBacktrack = false;
                while (!foundBacktrack)
                {
                    if (graph.BacktrackColoring(backtrackColorNumber))
                    {
                        Console.WriteLine("Backtrack solution found");
                        foundBacktrack = true;
                    }
                    else
                    {
                        Console.WriteLine("Backtrack solution not found");
                        graph.GeneticColoring(++backtrackColorNumber);
                    }
                }
                backtrackColors = graph.GetColors();

                graph.ApplyColors(greedyColors);
                Console.WriteLine("Greedy colors: " + greedyColorNumber);
                Console.WriteLine("Genetic colors: " + geneticColorNumber);
                Console.WriteLine("Backtrack colors: " + backtrackColorNumber);
                DrawGraph(graph);
                lblCursorPosition.Text = "Vertices: " + circles.Count + ", Edges: " + edges.Count + ", Colors: " + greedyColorNumber;
            }
        }

        private void SaveGraph_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text file (*.txt)|*.txt";
            if (dialog.ShowDialog() == true)
            {
                SaveGraph(graph, dialog.FileName);
            }
        }

        private void Greedy_Click(object sender, RoutedEventArgs e)
        {
            graph.ApplyColors(greedyColors);
            for (int i = 0;i < graph.Nodes.Count;i++)
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush();
                solidColorBrush.Color = colors[greedyColors[i]];
                graph.Nodes[i].Circle.Fill = solidColorBrush;
            }
            lblCursorPosition.Text = "Vertices: " + circles.Count + ", Edges: " + edges.Count + ", Colors: " + greedyColorNumber;
        }

        private void Genetic_Click(object sender, RoutedEventArgs e)
        {
            graph.ApplyColors(geneticColors);
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush();
                solidColorBrush.Color = colors[geneticColors[i]];
                graph.Nodes[i].Circle.Fill = solidColorBrush;
            }
            lblCursorPosition.Text = "Vertices: " + circles.Count + ", Edges: " + edges.Count + ", Colors: " + geneticColorNumber;
        }

        private void Backtracking_Click(object sender, RoutedEventArgs e)
        {
            graph.ApplyColors(backtrackColors);
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush();
                solidColorBrush.Color = colors[backtrackColors[i]];
                graph.Nodes[i].Circle.Fill = solidColorBrush;
            }
            lblCursorPosition.Text = "Vertices: " + circles.Count + ", Edges: " + edges.Count + ", Colors: " + backtrackColorNumber;
        }
    }
}
