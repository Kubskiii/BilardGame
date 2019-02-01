using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsEngine
{
    class Filler
    {
        class Node
        {
            float xmin;
            public int Ymax { get; }
            public float Xmin { get => xmin; }
            public float fracM { get; }
            public Node next { get; set; }
            public Node(Vector3 p1, Vector3 p2, Node _next = null)
            {
                Ymax = Math.Max((int)p1.Y, (int)p2.Y);
                xmin = Ymax == p1.Y ? p2.X : p1.X;
                fracM = (float)(p1.X - p2.X) / (p1.Y - p2.Y);
                next = _next;
            }
            public static Node operator ++(Node n)
            {
                n.xmin += n.fracM;
                return n;
            }
        }
        Node[] bucketTable;
        Node scanLine;
        int Ymax = 0;
        int Ymin = int.MaxValue;
        public Filler(int size)
        {
            bucketTable = new Node[size];
        }
        void FillTable(IEnumerable<Vector3> Polygon)
        {
            Ymax = 0; Ymin = int.MaxValue;
            Vector3 prevPoint = Polygon.LastOrDefault();
            foreach (var p in Polygon)
            {
                int position = Math.Min((int)prevPoint.Y, (int)p.Y);
                bucketTable[position] = new Node(prevPoint, p, bucketTable[position]);
                prevPoint = p;
                Ymax = Math.Max((int)Ymax, (int)p.Y);
                Ymin = Math.Min((int)Ymin, (int)p.Y);
            }
        }
        void InsertSorted(Node newNode)
        {
            if (scanLine == null || scanLine.Xmin > newNode.Xmin ||
                (scanLine.Xmin == newNode.Xmin && scanLine.fracM > newNode.fracM))
            {
                newNode.next = scanLine;
                scanLine = newNode;
            }
            else
            {
                Node tmp = scanLine;
                while (tmp.next != null &&
                    (tmp.next.Xmin < newNode.Xmin ||
                        (tmp.next.Xmin == newNode.Xmin &&
                         tmp.next.fracM <= newNode.fracM)))
                    tmp = tmp.next;
                newNode.next = tmp.next;
                tmp.next = newNode;
            }
        }
        static float RoundUp(float number)
        {
            if (number % 1.0 != 0) return number + 1;
            return number;
        }
        public void Draw(IEnumerable<Vector3> Polygon, Action<int, int> Fill)
        {
            scanLine = null;
            // sort edges
            FillTable(Polygon);
            for (int y = Ymin; y <= Ymax; y++)
            {
                // delete edges and correct crossing points
                for (Node n = scanLine, prevn = null; n != null; n = n.next)
                    if (n.Ymax < y)
                    {
                        if (prevn == null)
                        {
                            scanLine = n.next;
                            prevn = null;
                        }
                        else prevn.next = n.next;
                    }
                    else
                    {
                        n++;
                        prevn = n;
                    }

                // draw lines
                if (scanLine != null)
                {
                    bool draw = true;
                    for (Node n = scanLine; n != null && n.next != null; n = n.next)
                        if (draw)
                        {
                            for (int x = (int)RoundUp(n.Xmin); x <= (int)n.next.Xmin; x++) Fill(x, y);
                            draw = false;
                        }
                        else draw = true;
                }

                // insert new elements to scan line
                while (bucketTable[y] != null)
                {
                    var tmp = bucketTable[y];
                    bucketTable[y] = tmp.next;
                    tmp.next = null;
                    InsertSorted(tmp);
                }
            }
        }
    }
}
