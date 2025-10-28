using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace WindowsFormsApplicationzhh
{
    


    public class 大地电磁曲线类:曲线类
    {

        List<string> Lines = new List<string>();
        List<数据参数结构> Points = new List<数据参数结构>();
        List<选择点结构体> SelectedPoints = new List<选择点结构体>();
        

        struct 数据参数结构
        {
            public double 频率;
            public double ZXY;
            public double ZYX;
            public double PhXY;
            public double PhYX;
         }

        struct 选择点结构体
        {
            public int Index;
            public 曲线类型 CurveType;
            public 极化模式 CurveMode;

            public 选择点结构体(int index, 
                曲线类型 type = 曲线类型.视电阻率曲线, 
                极化模式 mode = 极化模式.BOTH)
            {
                Index = index;
                CurveType = type;
                CurveMode = mode;
            }
        }

        public enum 极化模式
        {
            BOTH = 0,
            TE = 1,
            TM = 2,
        }

        public enum 曲线类型 
        {
            视电阻率曲线 = 0,
            相位曲线 = 1,
        }

        public 曲线类型 CurveType { get; set; }
        public 极化模式 CurveMode { get; set; }
        override public int Count
        {
            get { return Points.Count; }
        }
        override public int SelectedCount
        {
            get { return SelectedPoints.Count; }
        }

        String SearchBlock(string Mark, StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                Lines.Add(line);
                line = line.Trim();
                if (line.Length < 1) continue;

                if (line[0] == '>')
                {
                    string[] ss = line.Split(new char[] { ' ', ',', '\t', '，','>' },
                                                                StringSplitOptions.RemoveEmptyEntries);
                    if (ss[0] == Mark)
                    {
                        return line;
                    }
                }
            }
            return null;
        }

        List<double> ReadData(StreamReader sr, int nfreq)
        {
            List<double> values = new List<double>();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine().Trim();
                string[] ss = line.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < ss.Length; i++)
                {
                    values.Add(double.Parse(ss[i]));
                }
                if (values.Count >= nfreq) break;
            }
            return values;
        }

        public override void Save(string path)
        {
            StreamWriter wr = new StreamWriter(path);
            for (int i = 0; i < Lines.Count; i++)
            {
                wr.WriteLine(Lines[i]);
                string line = Lines[i];
                if (line.Length > 0 && line[0] == '>')
                {
                    string[] ss = line.Split(new char[] { ' ', '\t', ',', '>', '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ss[0] == "FREQ")
                    {
                        string str = "";
                        for (int j = 0; j < Points.Count; j++)
                        {
                            str += Math.Pow(10, Points[j].频率) + " ";
                            if (j % 5 == 0)
                            {
                                wr.WriteLine(str);
                                str = "";
                            }
                        }
                        wr.WriteLine(str);
                    }
                    if (ss[0] == "RHOXY")
                    {
                        string str = "";
                        for (int j = 0; j < Points.Count; j++)
                        {
                            str += Math.Pow(10, Points[j].ZXY) + " ";
                            if (j % 5 == 0)
                            {
                                wr.WriteLine(str);
                                str = "";
                            }
                        }
                        wr.WriteLine(str);
                    }
                    if (ss[0] == "RHOYX")
                    {
                        string str = "";
                        for (int j = 0; j < Points.Count; j++)
                        {
                            str += Math.Pow(10, Points[j].ZYX) + " ";
                            if (j % 5 == 0)
                            {
                                wr.WriteLine(str);
                                str = "";
                            }
                        }
                        wr.WriteLine(str);
                    }
                    if (ss[0] == "PHSXY")
                    {
                        string str = "";
                        for (int j = 0; j < Points.Count; j++)
                        {
                            str += Math.Pow(10, Points[j].PhXY) + " ";
                            if (j % 5 == 0)
                            {
                                wr.WriteLine(str);
                                str = "";
                            }
                        }
                        wr.WriteLine(str);
                    } if (ss[0] == "RHSYX")
                    {
                        string str = "";
                        for (int j = 0; j < Points.Count; j++)
                        {
                            str += Math.Pow(10, Points[j].PhYX) + " ";
                            if (j % 5 == 0)
                            {
                                wr.WriteLine(str);
                                str = "";
                            }
                        }
                        wr.WriteLine(str);
                    }
                }
            }
            wr.Close();
        }

        override public void Load(String path)
        {
            FileName = path;
            Points.Clear();
            StreamReader sr = new StreamReader(path);
            string line = SearchBlock("FREQ", sr);
            int nfreq = 0;
            if (line != null)
            {
                string[] ss = line.Split(new char[] { ' ', '\t', ',', '>' ,'='}, StringSplitOptions.RemoveEmptyEntries);
                nfreq = int.Parse(ss[2]);
                List<double> values = ReadData(sr, nfreq);
                for (int i = 0; i < values.Count; i++)
                {
                    数据参数结构 p = new 数据参数结构();
                    p.频率 = Math.Log10(values[i]);
                    Points.Add(p);
                }


            }

            line = SearchBlock("RHOXY",sr);
            if (line != null)
            {
                List<double> values = ReadData(sr, nfreq);
                for(int i = 0; i< values.Count; i++)
                {
                    数据参数结构 p = Points[i];
                    p.ZXY =  Math.Log10(values[i]);
                    Points[i] = p;
                }
            }
            line = SearchBlock("RHOYX", sr);
            if (line != null)
            {
                List<double> values = ReadData(sr, nfreq);
                for (int i = 0; i < values.Count; i++)
                {
                    数据参数结构 p = Points[i];
                    p.ZYX =  Math.Log10(values[i]);
                    Points[i] = p;
                }
            }
            line = SearchBlock("PHSXY", sr);
            if (line != null)
            {
                List<double> values = ReadData(sr, nfreq);
                for (int i = 0; i < values.Count; i++)
                {
                    数据参数结构 p = Points[i];
                    p.PhXY = values[i];
                    Points[i] = p;
                }
            }
            line = SearchBlock("PHSYX", sr);
            if (line != null)
            {
                List<double> values = ReadData(sr, nfreq);
                for (int i = 0; i < values.Count; i++)
                {
                    数据参数结构 p = Points[i];
                    p.PhYX = values[i];
                    Points[i] = p;
                }
            }
            UpdateDataRange();
            sr.Close();

        }

        override public void UpdateDataRange()
        {
            minx = 1e30;
            maxx = -1e30;
            miny = 1e30;
            maxy = -1e30;
            for (int i = 0; i < Points.Count; i++)
            {
                数据参数结构 p = Points[i];
                if (p.频率 < minx) minx = p.频率;
                if (p.频率 > maxx) maxx = p.频率;
                if(CurveType == 曲线类型.视电阻率曲线)
                {
                    if (p.ZXY < miny) miny = p.ZXY;
                    if (p.ZXY > maxy) maxy = p.ZXY;
                    if (p.ZYX < miny) miny = p.ZYX;
                    if (p.ZYX > maxy) maxy = p.ZYX;
                }
                else
                {
                    if (p.PhXY < miny) miny = p.PhXY;
                    if (p.PhXY > maxy) maxy = p.PhXY;
                    if (p.PhYX < miny) miny = p.PhYX;
                    if (p.PhYX > maxy) maxy = p.PhYX;
                }
            }
        }

        void DrawZXY(Graphics g, Rectangle rect)
        {
            DrawRect = rect;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                double x1 = Points[i].频率;
                double y1 = Points[i].ZXY;
                double x2 = Points[i + 1].频率;
                double y2 = Points[i + 1].ZXY;
                PointF p1 = new PointF((float)x1, (float)y1);
                PointF p2 = new PointF((float)x2, (float)y2);
                p1 = LPtoDP(p1);
                p2 = LPtoDP(p2);
                g.DrawLine(Pens.Blue, p1, p2);
            }
            for (int i = 0; i < Points.Count; i++)
            {
                double x1 = Points[i].频率;
                double y1 = Points[i].ZXY;
                PointF p1 = new PointF((float)x1, (float)y1);
                p1 = LPtoDP(p1);
                RectangleF rect1 = new RectangleF(p1.X-6,p1.Y-6,12,12);

                if (IsInSelection(i,CurveType,极化模式.TE)) 
                    g.FillEllipse(Brushes.Red, rect1);
                else g.FillEllipse(Brushes.CadetBlue, rect1);
            }
        }
        void DrawZYX(Graphics g, Rectangle rect)
        {
            DrawRect = rect;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                double x1 = Points[i].频率;
                double y1 = Points[i].ZYX;
                double x2 = Points[i + 1].频率;
                double y2 = Points[i + 1].ZYX;
                PointF p1 = new PointF((float)x1, (float)y1);
                PointF p2 = new PointF((float)x2, (float)y2);
                p1 = LPtoDP(p1);
                p2 = LPtoDP(p2);
                g.DrawLine(Pens.BurlyWood, p1, p2);
            }
            for (int i = 0; i < Points.Count; i++)
            {
                double x1 = Points[i].频率;
                double y1 = Points[i].ZYX;
                PointF p1 = new PointF((float)x1, (float)y1);
                p1 = LPtoDP(p1);
                RectangleF rect1 = new RectangleF(p1.X - 6, p1.Y - 6, 12, 12);

                if (IsInSelection(i, CurveType, 极化模式.TM)) 
                    g.FillEllipse(Brushes.Red, rect1);
                else g.FillEllipse(Brushes.CadetBlue, rect1);
            }
        }
        void DrawPhXY(Graphics g, Rectangle rect)
        {
            DrawRect = rect;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                double x1 = Points[i].频率;
                double y1 = Points[i].PhXY;
                double x2 = Points[i + 1].频率;
                double y2 = Points[i + 1].PhXY;
                PointF p1 = new PointF((float)x1, (float)y1);
                PointF p2 = new PointF((float)x2, (float)y2);
                p1 = LPtoDP(p1);
                p2 = LPtoDP(p2);
                g.DrawLine(Pens.Blue, p1, p2);
            }
            for (int i = 0; i < Points.Count; i++)
            {
                double x1 = Points[i].频率;
                double y1 = Points[i].PhXY;
                PointF p1 = new PointF((float)x1, (float)y1);
                p1 = LPtoDP(p1);
                RectangleF rect1 = new RectangleF(p1.X-6,p1.Y-6,12,12);

                if (IsInSelection(i, CurveType, 极化模式.TE)) 
                    g.FillEllipse(Brushes.Red, rect1);
                else g.FillEllipse(Brushes.CadetBlue, rect1);
            }
        }
        void DrawPhYX(Graphics g, Rectangle rect)
        {
            DrawRect = rect;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                double x1 = Points[i].频率;
                double y1 = Points[i].PhYX;
                double x2 = Points[i + 1].频率;
                double y2 = Points[i + 1].PhYX;
                PointF p1 = new PointF((float)x1, (float)y1);
                PointF p2 = new PointF((float)x2, (float)y2);
                p1 = LPtoDP(p1);
                p2 = LPtoDP(p2);
                g.DrawLine(Pens.BurlyWood, p1, p2);
            }
            for (int i = 0; i < Points.Count; i++)
            {
                double x1 = Points[i].频率;
                double y1 = Points[i].PhYX;
                PointF p1 = new PointF((float)x1, (float)y1);
                p1 = LPtoDP(p1);
                RectangleF rect1 = new RectangleF(p1.X - 6, p1.Y - 6, 12, 12);
                if (IsInSelection(i, CurveType, 极化模式.TM)) 
                    g.FillEllipse(Brushes.Red, rect1);
                else g.FillEllipse(Brushes.CadetBlue, rect1);
            }
        }
        override public void Draw(Graphics g, Rectangle rect)
        {
            UpdateDataRange();
            if(CurveType == 曲线类型.视电阻率曲线)
            {
                if (CurveMode == 极化模式.TE || CurveMode == 极化模式.BOTH)
                {
                    DrawZXY(g, rect);
                }
                if (CurveMode == 极化模式.TM || CurveMode == 极化模式.BOTH)
                {
                    DrawZYX(g, rect);
                }
                
            }
            else
            {
                if (CurveMode == 极化模式.TE || CurveMode == 极化模式.BOTH)
                {
                    DrawPhXY(g, rect);
                }
                if (CurveMode == 极化模式.TM || CurveMode == 极化模式.BOTH)
                {
                    DrawPhYX(g, rect);
                }
            }

        }

        void DoSelectionTE(Point p)
        {
            double x1 = 0, y1 = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                x1 = Points[i].频率;

                if (CurveType == 曲线类型.视电阻率曲线) y1 = Points[i].ZXY;
                else y1 = Points[i].PhXY;

                PointF p1 = new PointF((float)x1, (float)y1);
                p1 = LPtoDP(p1);

                RectangleF rect1 = new RectangleF(p1.X - 6, p1.Y - 6, 12, 12);
                if (rect1.Contains(p))
                {
                    SelectedPoints.Add(new 选择点结构体(i, CurveType, 极化模式.TE));
                }
            }
        }
        void DoSelectionTE(Point p1,Point p2)
        {
            double x1 = 0, y1 = 0;
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);
            RectangleF rect1 = new RectangleF(x, y, width, height);
            for (int i = 0; i < Points.Count; i++)
            {
                x1 = Points[i].频率;

                if (CurveType == 曲线类型.视电阻率曲线) y1 = Points[i].ZXY;
                else y1 = Points[i].PhXY;

                PointF p3 = new PointF((float)x1, (float)y1);
                p3 = LPtoDP(p3);

                if (rect1.Contains(p3))
                {
                    SelectedPoints.Add(new 选择点结构体(i, CurveType, 极化模式.TE));
                }
            }
        }
        void DoSelectionTM(Point p)
        {
            double x1 = 0, y1 = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                x1 = Points[i].频率;

                if (CurveType == 曲线类型.视电阻率曲线) y1 = Points[i].ZYX;
                else y1 = Points[i].PhYX;

                PointF p1 = new PointF((float)x1, (float)y1);
                p1 = LPtoDP(p1);

                RectangleF rect1 = new RectangleF(p1.X - 6, p1.Y - 6, 12, 12);
                if (rect1.Contains(p))
                {
                    SelectedPoints.Add(new 选择点结构体(i, CurveType, 极化模式.TM));
                }
            }
        }
        void DoSelectionTM(Point p1, Point p2)
        {
            double x1 = 0, y1 = 0;
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);
            RectangleF rect1 = new RectangleF(x, y, width, height);
            for (int i = 0; i < Points.Count; i++)
            {
                x1 = Points[i].频率;

                if (CurveType == 曲线类型.视电阻率曲线) y1 = Points[i].ZYX;
                else y1 = Points[i].PhYX;

                PointF p3 = new PointF((float)x1, (float)y1);
                p3 = LPtoDP(p3);

                if (rect1.Contains(p3))
                {
                    SelectedPoints.Add(new 选择点结构体(i, CurveType, 极化模式.TM));
                }
            }
        }

        public override void DoSelection(Point p)
        {
            SelectedPoints.Clear();
            if (CurveMode == 极化模式.TE || CurveMode == 极化模式.BOTH)
                DoSelectionTE(p);
            if (CurveMode == 极化模式.TM || CurveMode == 极化模式.BOTH)
                DoSelectionTM(p);
        }

        public override void DoSelection(Point p1,Point p2)
        {
            SelectedPoints.Clear();
            if (CurveMode == 极化模式.TE || CurveMode == 极化模式.BOTH)
                DoSelectionTE(p1,p2);
            if (CurveMode == 极化模式.TM || CurveMode == 极化模式.BOTH)
                DoSelectionTM(p1,p2);

        }

        bool IsInSelection(int index, 曲线类型 type, 极化模式 mode)
        {
            foreach (选择点结构体 p in SelectedPoints)
            {
                if (index == p.Index &&
                    type == p.CurveType &&
                    mode == p.CurveMode) return true;
            }
            return false;
        }

        public override void DoModify(Point p1,Point p2)
        {
            PointF v1 = DPtoLP(p1);
            PointF v2 = DPtoLP(p2);
            double dy = v1.Y - v2.Y;
            for (int i = 0; i < SelectedPoints.Count; i++)
            {
                选择点结构体 selected = SelectedPoints[i];
                数据参数结构 p = Points[selected.Index];
                if (selected.CurveType == 曲线类型.视电阻率曲线 &&
                    selected.CurveMode == 极化模式.TE)
                    p.ZXY += dy;
                if (selected.CurveType == 曲线类型.视电阻率曲线 &&
                    selected.CurveMode == 极化模式.TM)
                    p.ZYX += dy;
                if (selected.CurveType == 曲线类型.相位曲线 &&
                    selected.CurveMode == 极化模式.TE)
                    p.PhXY += dy;
                if (selected.CurveType == 曲线类型.相位曲线 &&
                    selected.CurveMode == 极化模式.TM)
                    p.PhYX += dy;
                Points[selected.Index] = p;
            }

                
        }

        public override 曲线类 Clone()
        {
            var copy = new 大地电磁曲线类();
            copy.minx = this.minx;
            copy.maxx = this.maxx;
            copy.miny = this.miny;
            copy.maxy = this.maxy;
            copy.DrawRect = this.DrawRect;
            copy.FileName = this.FileName;
            copy.CurveType = this.CurveType;
            copy.CurveMode = this.CurveMode;
            copy.Lines = new List<string>(this.Lines);
            copy.Points = new List<数据参数结构>(this.Points);
            copy.SelectedPoints = new List<选择点结构体>(this.SelectedPoints);

            return copy;
        }


    }
}
