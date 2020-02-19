using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace OperationResearch
{
    public enum NodeColor
    {
        NOT_COLORED,
        RED,
        BLUE,
        WHITE,
        BLACK
    }
    public class NodeGraph
    {
        public string name;
        public int degree;
        public NodeColor color = NodeColor.NOT_COLORED;
        public Point point;

        public int d;   //distant for The Dijkstra Algorithm
        public NodeGraph prev;   //for The Dijkstra Algorithm

        public bool star;
        public NodeGraph()
        {
            this.name = "";
            this.degree = 0;
            this.star = false;
        }
    }
}
