using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace WindowsFormsApplicationzhh
{
    
    public class 曲线类
    {        
        public double minx, maxx, miny, maxy;
        public Rectangle DrawRect = new Rectangle();
        public string FileName;

        List<PointF> Points = new List<PointF>();
        virtual public int SelectedCount
        {
            get { return 0; }
        }
        virtual public int Count
        {
            get { return 0; }
        }
        

        virtual public void UpdateDataRange()
        {

        }

        virtual public void Draw(Graphics g, Rectangle rect)
        {

        }

        virtual public void Load(String path)
        {

        }

        public PointF LPtoDP(PointF p)
        {
            double x = p.X;
            double y = p.Y;
            double left = DrawRect.Left;
            double top = DrawRect.Top;
            double width = DrawRect.Width;
            double height = DrawRect.Height;
            x = left + width * (x - minx) / (maxx - minx);
            y = top + height - height * (y - miny) / (maxy - miny);
            return new PointF((float)x, (float)y);
        }
        public PointF DPtoLP(PointF p)
        {
            double x = p.X;
            double y = p.Y;
            double left = DrawRect.Left;
            double bottom = DrawRect.Bottom;
            double width = DrawRect.Width;
            double height = DrawRect.Height;
            x = minx + (maxx-minx) * (x - left) / width;
            y = miny + (maxy - miny) * (y - bottom) / height;
            return new PointF((float)x, (float)y);
        }

        

        virtual public void DoSelection(Point p)
        {

        }

        virtual public void DoSelection(Point p1,Point p2)
        {

        }

        virtual public void DoModify(Point p1,Point p2)
        {

        }

        virtual public void Save(string path)
        {

        }

    }
}
