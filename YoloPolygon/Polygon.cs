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
        public float X, Y;
    }
    public class Polygon
    {
        public int classID;
        public int cx, cy; //draw
        public double cxf, cyf;
        public int w, h; //draw
        public double wf, hf;
        public List<Point> drawPoints;
        public List<PointF> pointfs;

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(classID.ToString() + ' ');
            sb.Append(cx.ToString() + ' ');
            sb.Append(cy.ToString() + ' ');
            sb.Append(w.ToString() + ' ');
            sb.Append(h.ToString());

            for (int i = 0; i < pointfs.Count; i++)
            {
                PointF p = pointfs[i];
                sb.Append(' ' + p.X + ' ' + p.Y);
            }

            return sb.ToString();
        }
    }
}
