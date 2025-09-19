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
        //public int cx, cy; //draw
        //public double cxf, cyf;
        //public int w, h; //draw
        //public Size size;
        //public SizeF sizeF;



        //public Point center;
        //public Point centerF;
        public List<Point> points;
        public List<PointF> pointFs;

        public string status;

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(classID + " ");

            for (int i = 0; i < pointFs.Count; i++)
            {
                PointF p = pointFs[i];
                sb.Append(' ' + p.X + ' ' + p.Y);
            }

            return sb.ToString();
        }

        //public Rectangle GetBounding()
        //{
        //    Rectangle bounding = new Rectangle(w, h, 0, 0);

        //    for (int i = 0; i < points.Count; i++)
        //    {
        //        Point p = points[i];
        //        if (p.X < bounding.X) bounding.X = p.X;
        //        if (p.Y < bounding.Y) bounding.Y = p.Y;
        //        if (p.X > bounding.X + bounding.Width) bounding.Width = p.X - bounding.X;
        //        if (p.Y > bounding.Y + bounding.Height) bounding.Height = p.Y - bounding.Y;
        //    }

        //    w = bounding.Width;
        //    h = bounding.Height;

        //    cx = bounding.X + (w / 2);
        //    cy = bounding.Y + (h / 2);

        //    //Rectangle r = new Rectangle();
        //    //r.Width = w;
        //    //r.Height = h;
        //    //r.X = cx - r.Width / 2;
        //    //r.Y = cy - r.Height / 2;

        //    return bounding;
        //}
    }
}
