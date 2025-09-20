using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoloPolygon
{
    public class PointF
    {
        public double X, Y;
    }

    public class SizeF
    {
        public double Width, Height;
    }

    public class Line
    {
        public Point p1, p2;

        public Line(Point _p1, Point _p2)
        {
            this.p1 = _p1;
            this.p2 = _p2;
        }
    }

    public class Polygon
    {
        public int classID;

        //public Point center;
        //public Point centerF;
        public List<Point> points;
        public List<PointF> pointFs;

        public string status;

        public Polygon()
        {
            points = new List<Point>();
            pointFs = new List<PointF>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(classID);

            for (int i = 0; i < pointFs.Count; i++)
            {
                PointF p = pointFs[i];
                sb.Append(" " + p.X + " " + p.Y);
            }

            return sb.ToString();
        }
    }
}
