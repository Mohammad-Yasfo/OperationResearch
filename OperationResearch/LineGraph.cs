using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationResearch
{
    public class LineGraph
    {
        public string name;
        public NodeGraph begin;
        public NodeGraph end;
        public int w;  //Weight for The Dijkstra Algorithm

        public bool visited;
        public LineGraph()
        {
            this.visited = false;
        }
    }
}
