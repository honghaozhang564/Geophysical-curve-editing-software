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
        protected double zoomScale = 1.0;
        protected double offsetX = 0.0;
        protected double offsetY = 0.0;

        public List<PointF> Points = new List<PointF>();
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
            double centerX = (minx + maxx) / 2;
            double centerY = (miny + maxy) / 2;
            x = centerX + (x - centerX) * zoomScale + offsetX;
            y = centerY + (y - centerY) * zoomScale + offsetY;
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
            double centerX = (minx + maxx) / 2;
            double centerY = (miny + maxy) / 2;
            x = centerX + (x - centerX - offsetX) / zoomScale;
            y = centerY + (y - centerY - offsetY) / zoomScale;
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
        virtual public void ZoomIn()
        {
            zoomScale *= 1.25;
        }
        virtual public void ZoomOut()
        {
            zoomScale *= 0.8;
        }
        virtual public void ZoomReset()
        {
            zoomScale = 1.0;
            offsetX = 0.0;
            offsetY = 0.0;
        }
        virtual public void DoPan(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            double rangeX = maxx - minx;
            double rangeY = maxy - miny;
            double width = DrawRect.Width;
            double height = DrawRect.Height;

            offsetX += (dx / width) * rangeX / zoomScale;
            offsetY -= (dy / height) * rangeY / zoomScale;
        }

        virtual public 曲线类 Clone()
        {
            return null;
        }

    }
}
