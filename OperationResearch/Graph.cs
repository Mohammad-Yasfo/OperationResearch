using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Media;
using System.Xml;
//using Newtonsoft.Json;

namespace OperationResearch
{
    public class Gr
    {
        public List<NGraph> nodes;
        public List<LGraph> lines;
    }

    public class NGraph
    {
        public string name { get; set; }
    }

    public class LGraph
    {
        public string name { get; set; }
        public string begin { get; set; }
        public string end { get; set; }
    }
    public class Graph
    {
        public XmlDocument XmlDoc;
        public StreamReader Txt;

        public List<NodeGraph> Nodes;
        public List<LineGraph> Lines;

        public List<NodeGraph> open = new List<NodeGraph>();
        public List<NodeGraph> closed = new List<NodeGraph>();

        public Graph()
        {
            Nodes = new List<NodeGraph>();
            Lines = new List<LineGraph>();
        }

        //Export To Xml and Text
        public void SaveToXML(string fileName)
        {
            XmlTextWriter xmlw = new XmlTextWriter(fileName, Encoding.UTF8);
            xmlw.WriteString("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            xmlw.WriteStartElement("Graph");
            xmlw.WriteString("\n");
            xmlw.WriteStartElement("nodes");
            xmlw.WriteString("\n");

            foreach (NodeGraph n in Nodes)
            {
                xmlw.WriteStartElement("node");
                xmlw.WriteString("\n");
                xmlw.WriteStartElement("name");
                xmlw.WriteString(n.name);
                xmlw.WriteEndElement();
                xmlw.WriteString("\n");
                xmlw.WriteEndElement();
                xmlw.WriteString("\n");
                xmlw.WriteString("\n");
            }
            xmlw.WriteEndElement();
            xmlw.WriteString("\n");
            xmlw.WriteString("\n");
            xmlw.WriteStartElement("lines");
            xmlw.WriteString("\n");

            foreach (LineGraph c in Lines)
            {

                xmlw.WriteStartElement("line");
                xmlw.WriteString("\n");
                xmlw.WriteStartElement("name");
                xmlw.WriteString(c.name);
                xmlw.WriteEndElement();
                xmlw.WriteString("\n");
                xmlw.WriteStartElement("begin");
                xmlw.WriteString(c.begin.name);
                xmlw.WriteEndElement();
                xmlw.WriteString("\n");
                xmlw.WriteStartElement("end");
                xmlw.WriteString(c.end.name);
                xmlw.WriteEndElement();
                xmlw.WriteString("\n");
                xmlw.WriteEndElement();
                xmlw.WriteString("\n");
                xmlw.WriteString("\n");

            }
            xmlw.WriteEndElement();
            xmlw.WriteString("\n");
            xmlw.WriteEndElement();

            xmlw.Close();
        }

        public void SaveToText(string fileName)
        {
            StreamWriter txtw = new StreamWriter(fileName);
            foreach (NodeGraph item in Nodes)
            {
                if (Nodes[Nodes.Count - 1] == item)
                {
                    txtw.Write(item.name);
                }
                else
                {
                    txtw.Write(item.name + " ");
                }
            }
            txtw.WriteLine();
            foreach (LineGraph item in Lines)
            {
                txtw.WriteLine(item.name + " " + item.begin.name + " " + item.end.name);
            }
            txtw.Close();
        }
        //Reader Files

        public void ReaderJSON(string Doc)
        {
            string jsonText = File.ReadAllText(Doc);
            Gr gr = new JavaScriptSerializer().Deserialize<Gr>(jsonText);
            //Gr gr = JsonConvert.DeserializeObject<Gr>(jsonText);

            foreach (var item in gr.nodes)
            {
                NodeGraph n = new NodeGraph();
                n.name = item.name;
                Nodes.Add(n);
            }

            foreach (var item in gr.lines)
            {
                LineGraph l = new LineGraph();
                l.name = item.name;
                foreach (var t in Nodes) 
                {
                    if (item.begin == t.name)
                    {
                        l.begin = t;
                        break;
                    }
                }

                foreach (var t in Nodes)
                {
                    if (item.end == t.name)
                    {
                        l.end = t;
                        break;
                    }
                }
                Lines.Add(l);
            }
        }
        public void ReaderXML(XmlDocument XmlDoc, string Doc)
        {
            XmlDoc.Load(Doc);
            XmlNode node1;
            XmlNodeList node2;
            node1 = XmlDoc.DocumentElement;
            int nodesCount = node1.ChildNodes[0].ChildNodes.Count;
            int linesCount = node1.ChildNodes[1].ChildNodes.Count;

            for (int m = 0; m < nodesCount; m++)
            {
                NodeGraph newNode = new NodeGraph();
                node2 = node1.ChildNodes[0].ChildNodes;
                if (node2.Item(m).Name == "node")
                {
                    node2 = node1.ChildNodes[0].ChildNodes[m].ChildNodes;
                    newNode.name = node2.Item(0).InnerText.Trim();
                    Nodes.Add(newNode);
                }
            }
            for (int m = 0; m < linesCount; m++)
            {
                LineGraph newLine = new LineGraph();
                node2 = node1.ChildNodes[1].ChildNodes;
                if (node2.Item(m).Name == "line")
                {
                    node2 = node1.ChildNodes[1].ChildNodes[m].ChildNodes;
                    newLine.name = node2.Item(0).InnerText.Trim();
                    for (int i = 0; i < nodesCount; i++)
                    {
                        if (Nodes[i].name == node2.Item(1).InnerText.Trim())
                        {
                            newLine.begin = Nodes[i];
                        }
                    }
                    for (int i = 0; i < nodesCount; i++)
                    {
                        if (Nodes[i].name == node2.Item(2).InnerText.Trim())
                        {
                            newLine.end = Nodes[i];
                        }
                    }
                    Lines.Add(newLine);
                }
            }

            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Lines.Count; j++)
                {
                    if (Nodes[i].name == Lines[j].begin.name)
                        Nodes[i].degree++;
                    if (Nodes[i].name == Lines[j].end.name)
                        Nodes[i].degree++;
                }
            }
        }
        public void ReaderTXT(StreamReader Txt, string Doc)
        {
            Nodes.Clear();
            Lines.Clear();

            List<String> stTxtLines = new List<String>();

            string line = "";
            while (line != null)
            {
                line = Txt.ReadLine();
                if (line != null)
                {
                    stTxtLines.Add(line);
                }
            }
            Txt.Close();
            String[] stringnode = stTxtLines[0].Split(' ');
            for (int i = 0; i < stringnode.Length; i++)
            {
                NodeGraph newNode = new NodeGraph();
                newNode.name = stringnode[i];
                Nodes.Add(newNode);
            }
            for (int i = 1; i < stTxtLines.Count; i++)
            {
                String[] stringline = stTxtLines[i].Split(' ');

                LineGraph newLine = new LineGraph();
                newLine.name = stringline[0];
                for (int j = 0; j < Nodes.Count; j++)
                    if (Nodes[j].name == stringline[1])
                    {
                        newLine.begin = Nodes[j];
                    }
                for (int j = 0; j < Nodes.Count; j++)
                    if (Nodes[j].name == stringline[2])
                    {
                        newLine.end = Nodes[j];
                    }
                Lines.Add(newLine);
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Lines.Count; j++)
                {
                    if (Nodes[i].name == Lines[j].begin.name)
                        Nodes[i].degree++;
                    if (Nodes[i].name == Lines[j].end.name)
                        Nodes[i].degree++;
                }
            }
        }

        //Begin The BFS Algorithm
        public bool Contain(List<NodeGraph> l, NodeGraph n)
        {
            foreach (NodeGraph item in l)
            {
                if (item.name == n.name)
                {
                    return true;
                }
            }
            return false;
        }
        public List<NodeGraph> GetChildren(NodeGraph Father)
        {
            List<NodeGraph> children = new List<NodeGraph>();
            foreach (LineGraph item in Lines)
            {
                if ((Father.name == item.begin.name) && (Father.name != item.end.name))
                    children.Add(item.end);

                if ((Father.name == item.end.name) && (Father.name != item.begin.name))
                    children.Add(item.begin);
            }

            return children;
        }
        public bool TwoParts()
        {
            foreach (LineGraph item in Lines)
            {
                if (item.begin.color == item.end.color)
                {
                    return false;
                }
            }
            return true;
        }
        public void BFS_Algorithm(NodeGraph Root)
        {
            open.Clear();
            closed.Clear();

            open.Add(Root);
            while (open.Count != 0)
            {
                for (int i = 0; i < GetChildren(open[0]).Count; i++)
                {
                    if ((!Contain(open, GetChildren(open[0])[i])) && (!Contain(closed, GetChildren(open[0])[i])))
                    {
                        open.Add(GetChildren(open[0])[i]);
                        if (open[0].color == NodeColor.RED)
                            GetChildren(open[0])[i].color = NodeColor.BLUE;
                        if (open[0].color == NodeColor.BLUE)
                            GetChildren(open[0])[i].color = NodeColor.RED;
                    }
                }
                closed.Add(open.ElementAt(0));
                open.RemoveAt(0);
            }
        }
        //Begin The DFS Algorithm
        public List<NodeGraph> InsertChildrenInFirstOpen(List<NodeGraph> Source, List<NodeGraph> N)
        {
            List<NodeGraph> temp = new List<NodeGraph>();
            temp.Add(Source[0]);
            Source.RemoveAt(0);
            foreach (NodeGraph item in N)
            {
                temp.Add(item);
            }
            foreach (NodeGraph item in Source)
            {
                temp.Add(item);
            }
            return temp;
        }
        public void DFS_Algorithm(NodeGraph Root)
        {
            open.Add(Root);
            while (open.Count != 0)
            {
                List<NodeGraph> Children = new List<NodeGraph>();
                for (int i = 0; i < GetChildren(open[0]).Count; i++)
                {
                    if ((!Contain(open, GetChildren(open[0])[i])) && (!Contain(closed, GetChildren(open[0])[i])))
                    {
                        Children.Add(GetChildren(open[0])[i]);
                    }
                }
                open = InsertChildrenInFirstOpen(open, Children);

                closed.Add(open.ElementAt(0));
                open.RemoveAt(0);
            }
        }
        //End The BFS and DFS Algorithm

        public bool Simple()
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].begin.name == Lines[i].end.name)
                {
                    return false;
                }
                else
                {
                    for (int j = 0; j < Lines.Count; j++)
                    {
                        if (i != j)
                        {
                            if (Lines[i].begin.name == Lines[j].begin.name && Lines[i].end.name == Lines[j].end.name || Lines[i].begin.name == Lines[j].end.name && Lines[i].end.name == Lines[j].begin.name)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        public bool Complete()
        {
            bool IsComplete = true;
            List<NodeGraph> nodesChildren = new List<NodeGraph>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                nodesChildren = GetChildren(Nodes[i]);
                if (nodesChildren.Count != Nodes.Count - 1)
                {
                    return false;
                }
                for (int j = 0; j < nodesChildren.Count; j++)
                {
                    for (int k = 0; k < nodesChildren.Count; k++)
                    {
                        if (j != k)
                        {
                            if (nodesChildren.ToArray()[j].name == nodesChildren.ToArray()[k].name)
                            {
                                IsComplete = false;
                            }
                        }
                    }
                }
            }
            return IsComplete;
        }

        //Begin The Dijkstra Algorithm
        public void init()
        {
            foreach (NodeGraph item in Nodes)
            {
                item.d = int.MaxValue;
                item.color = NodeColor.WHITE;
            }
        }
        public NodeGraph ExtractMin()
        {
            int Min = int.MaxValue;
            int index = -1;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if ((Nodes[i].d <= Min) && (Nodes[i].color == NodeColor.WHITE))
                {
                    index = i;
                    Min = Nodes[i].d;
                }
            }
            if (index != -1)
            {
                return Nodes[index];
            }
            else
                return null;
        }
        public int Weight(NodeGraph begin, NodeGraph end)
        {
            foreach (LineGraph item in Lines)
            {
                if (item.begin.name == begin.name && item.end.name == end.name)
                {
                    return item.w;
                }
            }
            return 0;
        }
        public bool isWhite()
        {
            foreach (NodeGraph item in Nodes)
            {
                if (item.color == NodeColor.WHITE)
                {
                    return true;
                }
            }
            return false;
        }
        public List<NodeGraph> nighbores(NodeGraph n)
        {
            List<NodeGraph> child = new List<NodeGraph>();
            foreach (LineGraph item in Lines)
            {
                if (item.begin == n)
                    child.Add(item.end);
            }
            return child;
        }
        public void Dijexst(NodeGraph Start)
        {
            init();
            Start.d = 0;
            Start.prev = null;
            while (isWhite())
            {
                NodeGraph u = ExtractMin();
                List<NodeGraph> Children = nighbores(u);
                for (int i = 0; i < Children.Count; i++)
                {
                    if (u.d + Weight(u, Children[i]) < Children[i].d)
                    {
                        Children[i].d = u.d + Weight(u, Children[i]);
                        Children[i].prev = u;
                    }
                    u.color = NodeColor.BLACK;
                }
            }
        }
        //End The Dijkstra Algorithm

        public List<NodeGraph> U = new List<NodeGraph>();
        public List<LineGraph> T = new List<LineGraph>();
        public LineGraph GetLine(NodeGraph n1, NodeGraph n2)
        {
            foreach (LineGraph item in Lines)
            {
                if (item.begin.name == n1.name && item.end.name == n2.name || item.begin.name == n2.name && item.end.name == n1.name)
                {
                    return item;
                }
            }
            return null;
        }
        public void Search(NodeGraph n)
        {
            U.Add(n);
            for (int i = 0; i < GetChildren(n).Count; i++)
            {
                if (!GetChildren(n)[i].star)
                {
                    GetChildren(n)[i].star = true;
                    T.Add(GetLine(n, GetChildren(n)[i]));
                    Search(GetChildren(n)[i]);
                }
            }
        }
        public bool ISConnect2()
        {
            Nodes[0].star = true;
            Search(Nodes[0]);
            if (U.Count == Nodes.Count)
            {
                return true;
            }
            return false;
        }

        //Begin The Eular Path or Eular Circuit Algorithm
        public NodeGraph FirstOddNode;
        public NodeGraph SecondOddNode;
        public int numOfOdd()
        {
            int numDegree = 0;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].degree % 2 != 0)
                {

                    if (FirstOddNode != null)
                    {
                        SecondOddNode = Nodes[i];
                    }
                    else
                        FirstOddNode = Nodes[i];

                    numDegree++;
                }
            }
            return numDegree;
        }
        public bool IsConnect()
        {
            BFS_Algorithm(Nodes[0]);
            if (closed.Count == Nodes.Count)
            {
                return true;
            }
            return false;
        }
        public LineGraph GetLineUnVisited(NodeGraph n1, NodeGraph n2)
        {
            foreach (LineGraph item in Lines)
            {
                if (item.begin.name == n1.name && item.end.name == n2.name || item.begin.name == n2.name && item.end.name == n1.name)
                {
                    if (!item.visited)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public NodeGraph Location = new NodeGraph();
        public Stack<NodeGraph> Stack = new Stack<NodeGraph>();
        public List<NodeGraph> EularPathCircuit = new List<NodeGraph>();
        public bool IsBridge(NodeGraph n)
        {
            for (int i = 0; i < GetChildren(n).Count; i++)
            {

                return true;
            }
            return false;
        }
        public bool LinesUnVisited()
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].visited != true)
                {
                    return true;
                }

            }
            return false;
        }
        public List<NodeGraph> GetChildrenForLinesUnvisited(NodeGraph Father)
        {
            List<NodeGraph> children = new List<NodeGraph>();
            foreach (LineGraph item in Lines)
            {
                if (!item.visited)
                {
                    if ((Father.name == item.begin.name) && (Father.name != item.end.name))
                        children.Add(item.end);

                    else if ((Father.name == item.end.name) && (Father.name != item.begin.name))
                        children.Add(item.begin);
                }
            }
            return children;
        }
        public LineGraph GetLineHasChild(NodeGraph F)
        {
            foreach (LineGraph item in Lines)
            {
                if (!item.visited && (item.begin.name == F.name || item.end.name == F.name))
                {
                    item.visited = true;
                    return item;
                }
            }
            return null;
        }
        public int Eular_Path_Circuit(NodeGraph Start)
        {
            if (IsConnect())
            {
                Location = Start;
                LineGraph L = null;

                if (numOfOdd() == 0 || numOfOdd() == 2)
                {
                    if ((L = GetLineHasChild(Location)) != null)
                    {
                        Stack.Push(Location);
                        if (L.begin.name == Location.name)
                        {
                            Location = L.end;
                        }
                        else
                        {
                            Location = L.begin;
                        }
                        L = GetLineHasChild(Location);

                        while (Stack.Count > 0 || L != null)
                        {
                            if (L == null)
                            {
                                EularPathCircuit.Add(Location);
                                Location = Stack.Pop();
                            }
                            else
                            {
                                Stack.Push(Location);
                                if (L.begin.name == Location.name)
                                {
                                    Location = L.end;
                                }
                                else
                                {
                                    Location = L.begin;
                                }
                            }
                            L = GetLineHasChild(Location);
                        }
                        EularPathCircuit.Add(Location);
                        return 1;
                    }
                    else
                        return 0;
                }
            }
            return 0;
        }

        //Begin The Path or Cycle Hamiltonian Algorithm

        void CopyCycleToPath()
        {
            foreach (NodeGraph item in CycleHamiltonian)
            {
                NodeGraph n = new NodeGraph();
                n.name = item.name;
                PathHamiltonian.Add(n);
            }
        }
        public void FindHamiltonian(NodeGraph Start)
        {
            if (GetLine(CycleHamiltonian[0], Start) != null && CycleHamiltonian.Count == Nodes.Count)
            {
                if (PathHamiltonian.Count == 0)
                {
                    CopyCycleToPath();
                }
                throw new Exception("Find Path and Cycle Hamiltonian");
            }
            if (CycleHamiltonian.Count == Nodes.Count)
            {
                if (PathHamiltonian.Count == 0)
                {
                    CopyCycleToPath();
                }
                return;
            }
            for (int i = 0; i < GetChildren(Start).Count; i++)
            {
                if (!GetLine(Start, GetChildren(Start)[i]).visited && !Contain(CycleHamiltonian, GetChildren(Start)[i]))
                {
                    LineGraph L = GetLine(Start, GetChildren(Start)[i]);
                    L.visited = true;

                    CycleHamiltonian.Add(GetChildren(Start)[i]);
                    FindHamiltonian(GetChildren(Start)[i]);


                    L.visited = false;
                    CycleHamiltonian.RemoveAt(CycleHamiltonian.Count - 1);
                }
            }

        }
        public List<NodeGraph> CycleHamiltonian = new List<NodeGraph>();
        public List<NodeGraph> PathHamiltonian = new List<NodeGraph>();
        public bool maybeHamiltonian()
        {
            bool maybeFindHamiltonian = false;
            if (Nodes.Count >= 3)
            {
                foreach (NodeGraph item in Nodes)
                {
                    if (item.degree >= Nodes.Count / 2)
                    {
                        maybeFindHamiltonian = true;
                    }
                }
            }
            return maybeFindHamiltonian;
        }
        public int Hamiltonian(NodeGraph Start)
        {
            CycleHamiltonian.Clear();
            PathHamiltonian.Clear();
            foreach (LineGraph item in Lines)
            {
                item.visited = false;
            }

            if (Start == null)
            {
                Start = Nodes[0];
            }
            if (IsConnect())
            {
                CycleHamiltonian.Add(Start);
                try
                {
                    FindHamiltonian(Start);
                    if (PathHamiltonian.Count == 0)
                    {
                        return 0;
                    }
                    return 1;
                }
                catch (Exception e)
                {
                    if (e.Message == "Find Path and Cycle Hamiltonian")
                    {
                        return 2;
                    }
                }
            }
            return 0;
        }

        //Connection Array and Starting Array
        public List<LineGraph> getNumLines(NodeGraph node1, NodeGraph node2)
        {
            List<LineGraph> numLine = new List<LineGraph>();
            foreach (LineGraph item in Lines)
            {
                if (item.begin.name == node1.name && item.end.name == node2.name || item.end.name == node1.name && item.begin.name == node2.name)
                {
                    numLine.Add(item);
                }
            }
            return numLine;
        }
        public int[,] ConnectionArray()
        {
            int[,] ArrayConnection = new int[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    ArrayConnection[i, j] = getNumLines(Nodes[i], Nodes[j]).Count;
                }
            }
            return ArrayConnection;
        }
        public int[,] StartingArray()
        {
            int[,] A = new int[Nodes.Count, Lines.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Lines.Count; j++)
                {
                    int n = 0;
                    if (Nodes[i].name == Lines[j].begin.name)
                    {
                        n++;
                    }
                    if (Nodes[i].name == Lines[j].end.name)
                    {
                        n++;
                    }
                    A[i, j] = n;
                }
            }
            return A;
        }
        
        // ^_^ ^_^
        public bool IsNodeHole(NodeGraph n)
        {
            foreach (LineGraph item in Lines)
            {
                if (item.begin.name == n.name)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsNodeSpring(NodeGraph n)
        {
            foreach (LineGraph item in Lines)
            {
                if (item.end.name == n.name)
                {
                    return false;
                }
            }
            return true;
        }
        public List<NodeGraph> NodeHole(NodeGraph n)
        {
            if (IsNodeHole(n))
            {
                List<NodeGraph> temp = new List<NodeGraph>();
                foreach (LineGraph item in Lines)
                {
                    if (item.end.name == n.name)
                    {
                        temp.Add(item.end);
                    }
                }
                return temp;
            }
            return null;
        }
        public List<NodeGraph> NodeSpring(NodeGraph n)
        {
            if (IsNodeSpring(n))
            {
                List<NodeGraph> temp = new List<NodeGraph>();
                foreach (LineGraph item in Lines)
                {
                    if (item.begin.name == n.name)
                    {
                        temp.Add(item.end);
                    }
                }
                return temp;
            }
            return null;
        }


        //Begin The BFS Algorithm For Test NodesComeDown and NodsGoUp
        public List<NodeGraph> GetChildrenForDirectedGraph(NodeGraph Father)
        {
            List<NodeGraph> children = new List<NodeGraph>();
            foreach (LineGraph item in Lines)
            {
                if ((Father.name == item.begin.name) && (Father.name != item.end.name))
                    children.Add(item.end);
            }
            return children;
        }
        public bool BFS_AlgorithmForTest(NodeGraph Root, NodeGraph Goal)
        {
            open.Clear();
            closed.Clear();

            open.Add(Root);
            while (open.Count != 0)
            {
                for (int i = 0; i < GetChildrenForDirectedGraph(open[0]).Count; i++)
                {
                    if ((!Contain(open, GetChildrenForDirectedGraph(open[0])[i])) && (!Contain(closed, GetChildrenForDirectedGraph(open[0])[i])))
                    {
                        if (Goal.name == GetChildrenForDirectedGraph(open[0])[i].name)
                        {
                            return true;
                        }
                        open.Add(GetChildrenForDirectedGraph(open[0])[i]);
                    }
                }
                closed.Add(open.ElementAt(0));
                open.RemoveAt(0);
            }
            return false;
        }
        public List<NodeGraph> BFS_AlgorithmForGoal(NodeGraph Root)
        {
            open.Clear();
            closed.Clear();

            open.Add(Root);
            while (open.Count != 0)
            {
                for (int i = 0; i < GetChildrenForDirectedGraph(open[0]).Count; i++)
                {
                    if ((!Contain(open, GetChildrenForDirectedGraph(open[0])[i])) && (!Contain(closed, GetChildrenForDirectedGraph(open[0])[i])))
                    {
                        open.Add(GetChildrenForDirectedGraph(open[0])[i]);
                    }
                }
                closed.Add(open.ElementAt(0));
                open.RemoveAt(0);
            }
            return closed;
        }
        public List<NodeGraph> NodesGoUp(NodeGraph n)
        {
            List<NodeGraph> temp = new List<NodeGraph>();
            foreach (var item in Nodes)
            {
                if (BFS_AlgorithmForTest(item, n)) ;
                {
                    temp.Add(item);
                }
            }
            return temp;
        }
        public List<NodeGraph> NodesComeDown(NodeGraph n)
        {
            return BFS_AlgorithmForGoal(n);
        }

        //Test The Walk
        public LineGraph GetLineUnvisitedForDirectedGraph(NodeGraph n)  // For Finding Walk
        {
            foreach (var item in LinesWalk)
            {
                if (!item.visited && item.begin.name == n.name)
                {
                    item.visited = true;
                    return item;
                }
            }
            return null;
        }
        List<NodeGraph> NodesWalk = new List<NodeGraph>();
        List<LineGraph> LinesWalk = new List<LineGraph>();
        public bool TestWalk(string Walk)       // For Test Walk
        {

            string[] strWalk = Walk.Split(',');

            for (int i = 0; i < strWalk.Length; i++)
            {
                if (i % 2 == 0)
                {
                    NodeGraph n = new NodeGraph();
                    n.name = strWalk[i];
                    NodesWalk.Add(n);
                }
            }
            for (int i = 0; i < strWalk.Length; i++)
            {
                if (i % 2 != 0)
                {
                    LineGraph l = new LineGraph();
                    l.name = strWalk[i];
                    foreach (NodeGraph item in NodesWalk)
                    {
                        if (item.name == strWalk[i - 1])
                        {
                            l.begin = item;
                        }
                    }
                    foreach (NodeGraph item in NodesWalk)
                    {
                        if (item.name == strWalk[i + 1])
                        {
                            l.end = item;
                        }
                    }
                    LinesWalk.Add(l);
                }
            }

            List<NodeGraph> WalkList = new List<NodeGraph>();
            LineGraph line = null;
            Location = NodesWalk[0];
            if ((line = GetLineUnvisitedForDirectedGraph(Location)) != null)
            {
                Stack.Push(Location);

                Location = line.end;

                line = GetLineUnvisitedForDirectedGraph(Location);

                while (Stack.Count > 0 || line != null)
                {
                    if (line == null)
                    {
                        WalkList.Add(Location);
                        Location = Stack.Pop();
                    }
                    else
                    {
                        Stack.Push(Location);

                        Location = line.end;
                    }
                    line = GetLineUnvisitedForDirectedGraph(Location);
                }
                WalkList.Add(Location);
                return true;
            }
            else
                return false;
        }

        #region Karp Sipser
        //Karp Sipser Algorithms 
        public List<LineGraph> linesNotVisted()
        {
            List<LineGraph> list = new List<LineGraph>();
            foreach (var item in Lines)
            {
                if (!item.visited)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public List<NodeGraph> singleton()
        {
            List<NodeGraph> listSingleton = new List<NodeGraph>();
            foreach (var item in Nodes)
            {
                if (item.degree == 1 && !item.star)
                {
                    listSingleton.Add(item);
                }
            }
            return listSingleton;
        }

        public List<NodeGraph> getChildrenNotVisited(NodeGraph father)
        {
            List<NodeGraph> children = new List<NodeGraph>();
            foreach (LineGraph item in Lines)
            {
                if ((father == item.begin) && (father != item.end) && !item.visited)
                {
                    item.visited = true;
                    children.Add(item.end);
                }
                if ((father == item.end) && (father != item.begin) && !item.visited)
                {
                    item.visited = true;
                    children.Add(item.begin);
                }
            }
            return children;
        }

        public NodeGraph minDegreeNode()
        {
            NodeGraph min = new NodeGraph();
            min.degree = int.MaxValue;
            foreach (var item in Nodes)
            {
                item.degree = 0;
            } 
            foreach (var n in Nodes)
            {
                foreach (var l in Lines)
                {
                    if (n == l.begin && !l.visited)
                    {
                        n.degree++;
                    }
                    if (n == l.end && !l.visited)
                    {
                        n.degree++;
                    }
                }
            }
            foreach (var item in Nodes)
            {

                if (min.degree > item.degree && !item.star)
                {
                    min = item;
                }
            }
            return min;
        }

        public NodeGraph minNode(List<NodeGraph> l)
        {
            NodeGraph min = new NodeGraph();
            min.degree = int.MaxValue;
            foreach (var item in l)
            {
                if (min.degree > item.degree)
                {
                    min = item;
                }
            }
            return min;
        }

        public List<NodeGraph> karpSipser()
        {
            List<NodeGraph> m = new List<NodeGraph>();

            while (linesNotVisted().Count != 0)   // While There were Nodes Not Stared
            {
                NodeGraph nn = minDegreeNode();
                nn.star = true;
                m.Add(nn);
                NodeGraph n = minNode(getChildrenNotVisited(nn));
                m.Add(n);
                getChildrenNotVisited(n);
                n.star = true;
            }

            //while (linesNotVisted().Count != 0)   // While There were Nodes Not Stared
            //{
            //    var singletonNodes =  singleton();
            //    if (singletonNodes.Count != 0)
            //    {
            //        singletonNodes[0].star = true;
            //        m.Add(singletonNodes[0]);
            //        NodeGraph n = getChildrenNotVisited(singletonNodes[0])[0];
            //        m.Add(n);
            //        getChildrenNotVisited(n);
            //        n.star = true;
            //    }
            //    else
            //    {
            //        NodeGraph nn = minDegreeNode();
            //        nn.star = true;
            //        m.Add(nn);
            //        NodeGraph n = getChildrenNotVisited(nn)[0];
            //        m.Add(n);
            //        getChildrenNotVisited(n);
            //        n.star = true;
            //    }
            //}
            return m;
        }

        #endregion

        //Karp Sipser TT
        public List<NodeGraph> Karp()
        {
            List<NodeGraph> CH = new List<NodeGraph>();
            NodeGraph min1 = new NodeGraph();
            NodeGraph M;
            List<NodeGraph> Matching = new List<NodeGraph>();

            while (lineNotVisited().Count != 0)   // While There were Nodes Not Stared
            {
                var singletonNodes = singleton();
                if (singletonNodes.Count != 0)
                {
                    Random r=new Random();
                    int tt=(int)r.NextDouble() * (singletonNodes.Count + (0 - singletonNodes.Count));
                    M = singletonNodes[tt];
                }
                else
                {
                    M = minDegreeNodeT();
                }
                    Matching.Add(M);
                    M.star = true;
                    CH = GetChildrenT(M);
                    GetDegree();
                    min1.degree = int.MaxValue;
                    foreach (var item in CH)
                    {
                        if ((min1.degree > item.degree) && (!item.star))
                        {
                            min1 = item;
                        }
                    }
                    Matching.Add(min1);
                    min1.star = true;
                    GetChildrenT(min1);
                    GetDegree();

                
            }
            return Matching;
        }

        //أصغر عقدة 
        public NodeGraph minDegreeNodeT()
        {
            NodeGraph min = new NodeGraph();
            min.degree = int.MaxValue;

            GetDegree();

            foreach (var item in Nodes)
            {
                if ((min.degree > item.degree) && (!item.star) &&(item.degree!=0))
                {
                    min = item;
                }
            }
            return min;
        }




        public List<LineGraph> lineNotVisited()
        {
            List<LineGraph> L = new List<LineGraph>();
            foreach (LineGraph C in Lines)
            {
                if (!C.visited)
                {
                    L.Add(C);
                }
            }
            return L;
        }


        public List<NodeGraph> GetChildrenT(NodeGraph Father)
        {
            List<NodeGraph> children = new List<NodeGraph>();
            foreach (LineGraph item in Lines)
            {
                if ((Father.name == item.begin.name) && (Father.name != item.end.name) && (!item.visited))
                {
                    if (!item.end.star)
                    {
                        children.Add(item.end);
                        item.visited = true;
                    }
                }

                if ((Father.name == item.end.name) && (Father.name != item.begin.name) && (!item.visited))
                {
                    if (!item.begin.star)
                    {
                        children.Add(item.begin);
                        item.visited = true;
                    }

                }
            }

            return children;
        }


        public void GetDegree()
        {
            foreach (var item in Nodes)
            {
                item.degree = 0;
            }

            foreach (var n in Nodes)
            {
                foreach (var l in Lines)
                {
                    if (n == l.begin && !l.visited)
                    {
                        n.degree++;
                    }

                    if (n == l.end && !l.visited)
                    {
                        n.degree++;
                    }
                }
            }
        }

    }
}
