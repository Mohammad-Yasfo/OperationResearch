using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using OperationResearch;

namespace OperationResearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public XmlDocument xmlDoc;
        public StreamReader txt;
        public String PathXmlDoc;
        public String PathTxtDoc;
        public String PathJsonDoc;
        public static Graph graph = new Graph();


        #region Drawing Property

        static List<Ellipse> ellipses;

        Ellipse beginEllipse;
        Ellipse endEllipse;

        List<Ellipse> LoopsShape;
        List<Line> linesShapes;
        List<Label> labels;
        System.Windows.Point beginPoint;
        System.Windows.Point endPoint;
        bool beginPointAssigned;
        int nodecount;
        int linecount;
        bool waitingForCreation;
        MouseButtonState el_state;
        FrameworkElement clickedControl;
        bool importedNode;
        int loopcount;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            ellipses = new List<Ellipse>();
            LoopsShape = new List<Ellipse>();
            linesShapes = new List<Line>();
            labels = new List<Label>();
            nodecount = 0;
            linecount = 0;
            loopcount = 0;
        }
        #endregion

        #region Drawing

        void startDrawing()
        {
            waitingForCreation = true;
            importedNode = true;
        }
        private NodeGraph getNode(Ellipse targetedEllipse)
        {
            for (int i = 0; i < ellipses.Count; i++)
            {
                if (ellipses[i] == targetedEllipse)
                    return graph.Nodes[i];
            }
            return null;
        }
        private static Ellipse getEllipse(NodeGraph targetedNode)
        {
            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                if (graph.Nodes[i] == targetedNode)
                    return ellipses[i];
            }
            return null;
        }
        void updateLabels()
        {

            for (int i = 0; i < labels.Count; i++)   //Remove Labels
            {
                canvas.Children.Remove(labels[i]);
            }

            foreach (Ellipse el in ellipses)   // Create Labels
            {
                System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                label.Content = getNode(el).name;
                Canvas.SetZIndex(label, 1);
                Canvas.SetLeft(label, Canvas.GetLeft(el) + (el as FrameworkElement).Width / 12);
                Canvas.SetTop(label, Canvas.GetTop(el) + (el as FrameworkElement).Height / 1.3);
                labels.Add(label);

                canvas.Children.Add(label);
            }
        }

        void updateLines()
        {
            List<LineGraph> LinesDr = graph.Lines;

            List<Line> updatedLines = new List<Line>();
            List<LineGraph> linkedLines = new List<LineGraph>();


            foreach (LineGraph c in LinesDr)
            {
                foreach (Line l in linesShapes)
                {
                    if (linesShapes.IndexOf(l) == LinesDr.IndexOf(c))
                    {
                        updatedLines.Add(l);
                        linkedLines.Add(c);
                    }
                }
            }

            foreach (Line l in updatedLines)
            {

                Ellipse beginEllipse;
                Ellipse endEllipse;

                LineGraph line = graph.Lines[updatedLines.IndexOf(l)];

                beginEllipse = getEllipse(line.begin);
                endEllipse = getEllipse(line.end);


                Point beginPoint = new Point();
                beginPoint.X = Canvas.GetLeft(beginEllipse) + (beginEllipse as FrameworkElement).Width / 2;
                beginPoint.Y = Canvas.GetTop(beginEllipse) + (beginEllipse as FrameworkElement).Height / 2;

                Point endPoint = new Point();
                endPoint.X = Canvas.GetLeft(endEllipse) + (endEllipse as FrameworkElement).Width / 2;
                endPoint.Y = Canvas.GetTop(endEllipse) + (endEllipse as FrameworkElement).Height / 2;

                if (beginPoint == endPoint)           //For Move Loop But Not Moving  v_v 
                {
                    foreach (Ellipse item in LoopsShape)
                    {
                        if (l.Name == item.Name)
                        {
                            Canvas.SetLeft(item, beginPoint.X - ((Math.Sqrt(2) * item.Width) / 2 - item.Width / 2));
                            Canvas.SetTop(item, beginPoint.Y - ((Math.Sqrt(2) * item.Height) / 2 - item.Height / 2));
                        }
                    }
                }
                l.X1 = beginPoint.X;
                l.Y1 = beginPoint.Y;
                l.X2 = endPoint.X;
                l.Y2 = endPoint.Y;

            }
        }
        void el_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            el_state = MouseButtonState.Pressed;
            clickedControl = sender as FrameworkElement;
        }

        void el_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            updateLines();
            updateLabels();
            el_state = MouseButtonState.Released;

        }

        void el_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (el_state == MouseButtonState.Pressed && clickedControl == sender as FrameworkElement)
            {
                el_Clicked();
            }
            el_state = MouseButtonState.Released;

        }
        void drawEllipse(Point beginPoint, Point endPoint, string nameEllipse)
        {
            loopcount++;
            Line l = new Line();
            l.StrokeThickness = 2;
            l.X1 = beginPoint.X;
            l.Y1 = beginPoint.Y;
            l.X2 = endPoint.X;
            l.Y2 = endPoint.Y;
            l.ToolTip = nameEllipse;
            l.Name = "e" + loopcount;

            l.Stroke = (Brush)new SolidColorBrush(Colors.Black);
            linesShapes.Add(l);


            Ellipse Loop = new Ellipse();            // This is Line in Code But It is Ellipse in Canvas
            Loop.Stroke = new SolidColorBrush(Colors.Black) as Brush;
            Loop.ToolTip = nameEllipse;
            Loop.Name = "e" + loopcount;
            Loop.StrokeThickness = 2;
            Loop.Width = 50;
            Loop.Height = 50;
            Canvas.SetZIndex(Loop, 5);
            Canvas.SetLeft(Loop, (beginPoint.X - ((Math.Sqrt(2) * Loop.Width) / 2 - Loop.Width / 2)));
            Canvas.SetTop(Loop, (beginPoint.Y - ((Math.Sqrt(2) * Loop.Height) / 2 - Loop.Height / 2)));
            LoopsShape.Add(Loop);
            canvas.Children.Add(Loop);

        }

        void el_Clicked()
        {

            double left = Canvas.GetLeft(clickedControl);
            double top = Canvas.GetTop(clickedControl);

            if (beginPointAssigned)
            {

                endPoint.X = left + (clickedControl.Width / 2);
                endPoint.Y = top + (clickedControl.Height / 2);
                endEllipse = clickedControl as Ellipse;

                linecount = graph.Lines.Count;
                linecount++;
                NodeGraph beginNode = getNode(beginEllipse);
                NodeGraph endNode = getNode(endEllipse);
                LineGraph lg = new LineGraph();
                lg.name = "e" + linecount;
                lg.begin = beginNode;
                lg.end = endNode;
                graph.Lines.Add(lg);
                if (beginPoint == endPoint)
                {
                    drawEllipse(beginPoint, endPoint, lg.name);
                }
                else
                {
                    drawLine(beginPoint, endPoint, lg.name);
                }

                beginEllipse.Fill = new SolidColorBrush(Colors.LightCyan);

                beginPointAssigned = false;
            }
            else
            {

                beginPoint.X = left + (clickedControl.Width / 2);
                beginPoint.Y = top + (clickedControl.Height / 2);
                beginEllipse = clickedControl as Ellipse;

                beginEllipse.Fill = new SolidColorBrush(Colors.LightGreen);

                beginPointAssigned = true;
            }
        }
        private void drawLine(Point beginPoint, Point endPoint, string nameLine)
        {
            Line l = new Line();
            l.StrokeThickness = 2;
            l.X1 = beginPoint.X;
            l.Y1 = beginPoint.Y;
            l.X2 = endPoint.X;
            l.Y2 = endPoint.Y;
            l.ToolTip = nameLine;


            Canvas.SetZIndex(l, 0);

            l.Stroke = (Brush)new SolidColorBrush(Colors.Black);


            linesShapes.Add(l);
            canvas.Children.Add(l);
        }
        public static void ColoringNodes()
        {
            foreach (NodeGraph N in graph.Nodes)
            {
                Ellipse El = getEllipse(N);
                if (N.color == NodeColor.RED)
                    El.Fill = new SolidColorBrush(Colors.Red);
                else if (N.color == NodeColor.BLUE)
                    El.Fill = new SolidColorBrush(Colors.Blue);
            }
        }

        #endregion

        private void Main_Load(object sender, EventArgs e)
        {

        }
        private void rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void rightBtn_Copy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        public static void BrowsingXml(ref XmlDocument XML, ref string path, ref OpenFileDialog op)
        {
            //bring Xml File using OpenFileDialog
            op.Filter = "XML Files (*.xml)|*.xml";
            if (op.ShowDialog() == true)
            {
                XML = new XmlDocument();
                path = "" + op.FileName;
            }
        }
        public static void BrowsingText(ref StreamReader txt, ref string path, ref OpenFileDialog op)
        {
            //bring Text File using OpenFileDialog
            op.Filter = "Text Files (*.txt)|*.txt";
            if (op.ShowDialog() == true)
            {
                path = "" + op.FileName;
                txt = new StreamReader(path);
            }
        }
        private void btn_ReadXml_Click(object sender, RoutedEventArgs e)
        {
            label1.Content = "Result:  ";
            rtb_Nodes.Document.Blocks.Clear();
            rtb_Lines.Document.Blocks.Clear();
            rtb_Degree.Document.Blocks.Clear();
            rtb_Children.Document.Blocks.Clear();
            graph.Nodes.Clear();
            graph.Lines.Clear();

            OpenFileDialog ofd = new OpenFileDialog();
            BrowsingXml(ref xmlDoc, ref PathXmlDoc, ref ofd);
            graph.ReaderXML(xmlDoc, PathXmlDoc);

            linesShapes.Clear();
            ellipses.Clear();
            canvas.Children.Clear();
            cb_Nodes.Items.Clear();
            richTextBox.Document.Blocks.Clear();
            richTextBox.AppendText("The Nodes: ");
            foreach (NodeGraph item in graph.Nodes)
            {
                richTextBox.AppendText(item.name);
                richTextBox.AppendText("   ");
                cb_Nodes.Items.Add(item.name);
                rtb_Nodes.AppendText(item.name);
                rtb_Nodes.AppendText(" ");
                rtb_Degree.AppendText(Convert.ToString(item.degree));
                rtb_Degree.AppendText(" ");
            }
            richTextBox.AppendText("\nThe Lines:\n");
            foreach (LineGraph item in graph.Lines)
            {
                richTextBox.AppendText(item.name);
                richTextBox.AppendText("  begin: ");
                richTextBox.AppendText(item.begin.name);
                richTextBox.AppendText("  end: ");
                richTextBox.AppendText(item.end.name);
                richTextBox.AppendText("\n");
                rtb_Lines.AppendText(item.name);
                rtb_Lines.AppendText(" ");
            }
            startDrawing();
        }

        private void btn_ReadText_Click(object sender, RoutedEventArgs e)
        {
            label1.Content = "Result:  ";
            rtb_Nodes.Document.Blocks.Clear();
            rtb_Lines.Document.Blocks.Clear();
            rtb_Degree.Document.Blocks.Clear();
            rtb_Children.Document.Blocks.Clear();
            graph.Nodes.Clear();
            graph.Lines.Clear();

            OpenFileDialog ofd = new OpenFileDialog();
            BrowsingText(ref txt, ref PathTxtDoc, ref ofd);
            graph.ReaderTXT(txt, PathXmlDoc);

            linesShapes.Clear();
            ellipses.Clear();
            canvas.Children.Clear();
            cb_Nodes.Items.Clear();
            richTextBox.Document.Blocks.Clear();
            richTextBox.AppendText("The Nodes: ");
            foreach (NodeGraph item in graph.Nodes)
            {
                richTextBox.AppendText(item.name);
                richTextBox.AppendText("   ");
                cb_Nodes.Items.Add(item.name);
                rtb_Nodes.AppendText(item.name);
                rtb_Nodes.AppendText(" ");
                rtb_Degree.AppendText(Convert.ToString(item.degree));
                rtb_Degree.AppendText(" ");
            }
            richTextBox.AppendText("\nThe Lines:\n");
            foreach (LineGraph item in graph.Lines)
            {
                richTextBox.AppendText(item.name);
                richTextBox.AppendText("  begin: ");
                richTextBox.AppendText(item.begin.name);
                richTextBox.AppendText("  end: ");
                richTextBox.AppendText(item.end.name);
                richTextBox.AppendText("\n");
                rtb_Lines.AppendText(item.name);
                rtb_Lines.AppendText(" ");
            }
            startDrawing();
        }

        private void btn_Test_Click(object sender, RoutedEventArgs e)
        {
            NodeGraph root = new NodeGraph();
            foreach (NodeGraph item in graph.Nodes)
            {
                if (item.name == cb_Nodes.Text)
                {
                    root = item;
                    break;
                }
            }
            if (root.name == "")
            {
                root = graph.Nodes[0];
            }
            root.color = NodeColor.RED;
            graph.BFS_Algorithm(root);
            if (graph.TwoParts())
            {
                label1.Content = "The Graph is Two Parts.";
            }
            else
            {
                label1.Content = "The Graph isn't Two Parts.";
            }
        }

        private void btn_Simple_Click(object sender, RoutedEventArgs e)
        {
            if (graph.Simple())
            {
                label1.Content = "The Graph is Simple.";
            }
            else
            {
                label1.Content = "The Graph isn't Simple.";
            }
        }

        private void btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            if (graph.Complete())
            {
                label1.Content = "The Graph is Complete.";
            }
            else
            {
                label1.Content = "The Graph isn't Complete.";
            }
        }

        private void btn_Hamilton_Click(object sender, RoutedEventArgs e)
        {
            foreach (LineGraph item in graph.Lines)
            {
                item.visited = false;
            }
            string s = "";
            NodeGraph Start = new NodeGraph();
            foreach (NodeGraph item in graph.Nodes)
            {
                if (item.name == cb_Nodes.Text)
                {
                    Start = item;
                    break;
                }
            }
            if (Start.name == "")
            {
                Start = graph.Nodes[0];
            }
            int flag = graph.Hamiltonian(Start);
            if (flag == 1)
            {
                s = "Find Only Path Hamiltonian \nThe Path: ";
                for (int i = 0; i < graph.PathHamiltonian.Count; i++)
                {
                    s += graph.PathHamiltonian[i].name;
                }
                MessageBox.Show(s);
            }
            else if (flag == 2)
            {
                s = "Find Path and Cycle Hamiltonian \nThe Path: ";
                for (int i = 0; i < graph.PathHamiltonian.Count; i++)
                {
                    s += graph.PathHamiltonian[i].name;
                }
                s += "\nThe Cycle: ";
                for (int i = 0; i < graph.CycleHamiltonian.Count; i++)
                {
                    s += graph.CycleHamiltonian[i].name;
                }
                MessageBox.Show(s);
            }
            else if (flag == 0)
            {
                MessageBox.Show("Not Find Path and Cycle Hamiltonian.");
            }
        }

        private void btn_CreateBtnDraw_Click(object sender, RoutedEventArgs e)
        {
            if (waitingForCreation)
            {
                waitingForCreation = false;
                canvas.Cursor = System.Windows.Input.Cursors.Arrow;
                btn_CreateBtnDraw.Content = "Draw";
            }
            else
            {
                canvas.Cursor = System.Windows.Input.Cursors.Cross;
                waitingForCreation = true;
                btn_CreateBtnDraw.Content = "Stop";
            }
        }

        private void canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (waitingForCreation)
            {
                btn_SaveToXml.IsEnabled = true;
                btn_SaveToText.IsEnabled = true;
                btn_Clear.IsEnabled = true;

                Ellipse el = new Ellipse();
                el.Stroke = new SolidColorBrush(Colors.Black) as Brush;
                el.Fill = new SolidColorBrush(Colors.LightCyan) as Brush;
                el.Width = 20;
                el.Height = 20;

                Canvas.SetZIndex(el, 5);



                el.PreviewMouseMove += el_PreviewMouseMove;
                el.MouseUp += el_MouseUp;

                el.PreviewMouseLeftButtonDown += el_PreviewMouseLeftButtonDown;


                Canvas.SetLeft(el, e.GetPosition(canvas).X - el.Height / 2);
                Canvas.SetTop(el, e.GetPosition(canvas).Y - el.Width / 2);

                ellipses.Add(el);  //Add the el to ellipses list
                canvas.Children.Add(el);  //show el on canvas

                if (!importedNode)
                {
                    nodecount = graph.Nodes.Count;
                    nodecount++;
                    NodeGraph ng = new NodeGraph();
                    ng.name = Convert.ToString(nodecount);
                    graph.Nodes.Add(ng);
                    cb_Nodes.Items.Add(ng.name);
                }
                else // It's Imported from existing Xml or Text
                {

                    if (ellipses.Count == graph.Nodes.Count)
                    {

                        waitingForCreation = false;
                        importedNode = false;

                        // Now draw The Lines

                        foreach (LineGraph c in graph.Lines)
                        {
                            Ellipse beginEllipse = getEllipse(c.begin);
                            double left = Canvas.GetLeft(beginEllipse);
                            double top = Canvas.GetTop(beginEllipse);
                            beginPoint.X = left + (beginEllipse.Width / 2);
                            beginPoint.Y = top + (beginEllipse.Height / 2);

                            Ellipse endEllipse = getEllipse(c.end);
                            left = Canvas.GetLeft(endEllipse);
                            top = Canvas.GetTop(endEllipse);
                            endPoint.X = left + (beginEllipse.Width / 2);
                            endPoint.Y = top + (beginEllipse.Height / 2);

                            if (beginPoint == endPoint)
                            {
                                drawEllipse(beginPoint, endPoint, c.name);
                            }
                            else
                            {
                                drawLine(beginPoint, endPoint, c.name);
                            }
                        }
                        if (graph.TwoParts())
                        {
                            ColoringNodes();
                        }
                    }
                }
            }
        }


        private void btn_Eular_Click(object sender, RoutedEventArgs e)
        {
            foreach (LineGraph item in graph.Lines)
            {
                item.visited = false;
            }
            graph.EularPathCircuit.Clear();
            NodeGraph Start = new NodeGraph();
            foreach (NodeGraph item in graph.Nodes)
            {
                if (item.name == cb_Nodes.Text)
                {
                    Start = item;
                    break;
                }
            }
            if (Start.name == "")
            {
                Start = graph.Nodes[0];
            }
            bool IsPath = false;
            if (graph.numOfOdd() == 2)
            {
                Start = graph.FirstOddNode;
                IsPath = true;
            }
            int flag = graph.Eular_Path_Circuit(Start);
            if (flag == 1)
            {
                string s = string.Empty;
                if (IsPath)
                {
                    s = "The Path: ";
                }
                else
                {
                    s = "The Circuit: ";
                }
                foreach (NodeGraph item in graph.EularPathCircuit)
                {
                    s += item.name;
                }
                MessageBox.Show(s);
            }
            else
            {
                MessageBox.Show("Not Find Path and Circuit Eularian.");
            }
        }

        private void btn_SaveToXml_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML Files | *.xml";
            if (sfd.ShowDialog() == true)
            {
                string path = sfd.FileName;
                graph.SaveToXML(path);
            }
        }

        private void btn_SaveToText_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files | *.txt";
            if (sfd.ShowDialog() == true)
            {
                string path = sfd.FileName;
                graph.SaveToText(path);
            }
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox1.Document.Blocks.Clear();
            rtb_Nodes.Document.Blocks.Clear();
            rtb_Lines.Document.Blocks.Clear();
            rtb_Degree.Document.Blocks.Clear();
            rtb_Children.Document.Blocks.Clear();
            cb_Nodes.Items.Clear();
            nodecount = 0;
            linecount = 0;
            graph.Nodes.Clear();
            graph.Lines.Clear();
            linesShapes.Clear();
            ellipses.Clear();
            canvas.Children.Clear();
        }

        private void cb_Nodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rtb_Children.Document.Blocks.Clear();
            rtb_Nodes.Document.Blocks.Clear();
            List<NodeGraph> Child = new List<NodeGraph>();
            NodeGraph father = new NodeGraph();
            foreach (NodeGraph item in graph.Nodes)
            {
                if (item.name == cb_Nodes.Text)
                {
                    father = item;
                    break;
                }
            }
            Child = graph.GetChildren(father);
            foreach (NodeGraph item in Child)
            {
                rtb_Children.AppendText(item.name);
                rtb_Children.AppendText(" ");
            }

            rtb_Nodes.AppendText(cb_Nodes.Text);
        }

        private void btn_json_Click(object sender, RoutedEventArgs e)
        {
            label1.Content = "Result:  ";
            rtb_Nodes.Document.Blocks.Clear();
            rtb_Lines.Document.Blocks.Clear();
            rtb_Degree.Document.Blocks.Clear();
            rtb_Children.Document.Blocks.Clear();
            richTextBox1.Document.Blocks.Clear();
            graph.Nodes.Clear();
            graph.Lines.Clear();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON Files (*.json)|*.json";

            if (ofd.ShowDialog() == true)
            {
                PathJsonDoc = "" + ofd.FileName;
            }
            graph.ReaderJSON(PathJsonDoc);

            linesShapes.Clear();
            ellipses.Clear();
            canvas.Children.Clear();
            cb_Nodes.Items.Clear();
            richTextBox.Document.Blocks.Clear();
            richTextBox.AppendText("The Nodes: ");
            foreach (NodeGraph item in graph.Nodes)
            {
                richTextBox.AppendText(item.name);
                richTextBox.AppendText("   ");
                cb_Nodes.Items.Add(item.name);
                rtb_Nodes.AppendText(item.name);
                rtb_Nodes.AppendText(" ");
                rtb_Degree.AppendText(Convert.ToString(item.degree));
                rtb_Degree.AppendText(" ");
            }
            richTextBox.AppendText("\nThe Lines:\n");
            foreach (LineGraph item in graph.Lines)
            {
                richTextBox.AppendText(item.name);
                richTextBox.AppendText("  begin: ");
                richTextBox.AppendText(item.begin.name);
                richTextBox.AppendText("  end: ");
                richTextBox.AppendText(item.end.name);
                richTextBox.AppendText("\n");
                rtb_Lines.AppendText(item.name);
                rtb_Lines.AppendText(" ");
            }
            startDrawing();
        }

        private void btn_karpSipser_Click(object sender, RoutedEventArgs e)
        {
            string NotVisited = "";
            List<NodeGraph> NodesKarpSipser = graph.Karp();
            string s = "Karp Graph is: \n";
            int style = 0;
            foreach (NodeGraph item in NodesKarpSipser)
            {
                if (style % 2 == 0)
                {
                    s += item.name + " ";
                }
                else
                {
                    s += item.name + "\n";
                }

                style++;
            }

            foreach (NodeGraph item in graph.Nodes)
            {
                if (!item.star)
                {
                    NotVisited = "And The isolated Node is \n" + item.name;
                }
            }

            richTextBox1.AppendText(s);
            richTextBox1.AppendText(NotVisited);

        }
    }
}

