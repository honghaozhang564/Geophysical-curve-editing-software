using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Security.Cryptography.ECCurve;
using static WindowsFormsApplicationzhh.大地电磁曲线类;

namespace WindowsFormsApplicationzhh
{
    public class 电测深曲线类 : 曲线类
    {
        List<PointF> Points = new List<PointF>();
        List<选择点结构体> SelectedPoints = new List<选择点结构体>();

        struct 选择点结构体
        {
            public int Index;

            public 选择点结构体(int index)
            {
                Index = index;
            }
        }
        override public int Count
        {
            get { return Points.Count; }
        }
        override public int SelectedCount
        {
            get { return SelectedPoints.Count; }
        }
        public override void DoSelection(Point p)
        {
            SelectedPoints.Clear();
            double x1 = 0, y1 = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                x1 = Points[i].X;
                y1 = Points[i].Y;

                PointF p1 = new PointF((float)x1, (float)y1);
                p1 = LPtoDP(p1);

                RectangleF rect1 = new RectangleF(p1.X - 6, p1.Y - 6, 12, 12);
                if (rect1.Contains(p))
                {
                    SelectedPoints.Add(new 选择点结构体(i));
                }
            }

        }
        public override void DoSelection(Point p1, Point p2)
        {
            SelectedPoints.Clear();
            double x1 = 0, y1 = 0;
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);
            RectangleF rect1 = new RectangleF(x, y, width, height);
            for (int i = 0; i < Points.Count; i++)
            {
                x1 = Points[i].X;
                y1 = Points[i].Y;
                PointF p3 = new PointF((float)x1, (float)y1);
                p3 = LPtoDP(p3);

                if (rect1.Contains(p3))
                {
                    SelectedPoints.Add(new 选择点结构体(i));
                }
            }

        }
        override public void Load(String path)
        {
            Points.Clear();
            StreamReader reader = new StreamReader(path);
            reader.ReadLine();
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;
                string[] ss = line.Split(',');
                float x = float.Parse(ss[0]);
                float y = float.Parse(ss[1]);
                PointF p = new PointF(x, y);
                Points.Add(p);
            }
            UpdateDataRange();
            reader.Close();
        }
        override public void UpdateDataRange()
        {
            minx = 1e30;
            maxx = -1e30;
            miny = 1e30;
            maxy = -1e30;
            for (int i = 0; i < Points.Count; i++)
            {
                PointF p = Points[i];
                if (p.X < minx) minx = p.X;
                if (p.X > maxx) maxx = p.X;
                if (p.Y < miny) miny = p.Y;
                if (p.Y > maxy) maxy = p.Y;
            }

        }
        override public void Draw(Graphics g, Rectangle rect)
        {
            UpdateDataRange();
            DrawRect = rect;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                PointF p1 = Points[i];
                PointF p2 = Points[i + 1];
                p1 = LPtoDP(p1);
                p2 = LPtoDP(p2);
                g.DrawLine(Pens.Blue, p1, p2);

            }

            for (int i = 0; i < Points.Count; i++)
            {
                PointF p1 = Points[i];
                p1 = LPtoDP(p1);
                RectangleF rect1 = new RectangleF(p1.X-6,p1.Y-6,12,12);
                if (IsInSelection(i))
                    g.FillEllipse(Brushes.Red, rect1);
                else g.FillEllipse(Brushes.CadetBlue, rect1);
            }
        }
        bool IsInSelection(int index)
        {
            foreach (选择点结构体 p in SelectedPoints)
            {
                if (index == p.Index) return true;
            }
            return false;
        }

        public override void DoModify(Point p1, Point p2)
        {
            PointF v1 = DPtoLP(p1);
            PointF v2 = DPtoLP(p2);
            double dy = v1.Y - v2.Y;
            for (int i = 0; i < SelectedPoints.Count; i++)
            {
                选择点结构体 selected = SelectedPoints[i];
                PointF p = Points[selected.Index];
                p.Y += (float)dy;
                Points[selected.Index] = p;
            }


        }

        public override 曲线类 Clone()
        {
            var copy = new 电测深曲线类();
            copy.minx = this.minx;
            copy.maxx = this.maxx;
            copy.miny = this.miny;
            copy.maxy = this.maxy;
            copy.DrawRect = this.DrawRect;
            copy.FileName = this.FileName;
            copy.Points = new List<PointF>(this.Points);
            copy.SelectedPoints = new List<选择点结构体>(this.SelectedPoints);

            return copy;
        }
    }

}

