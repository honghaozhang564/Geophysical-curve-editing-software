using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplicationzhh
{
    public partial class Form1 : Form
    {
        曲线类 地球物理曲线 = null;
        大地电磁曲线类 大地电磁曲线对象;
        List<曲线类> 曲线列表 = new List<曲线类>();
        bool bMouseDown = false;
        鼠标模式 mouseMode = new 鼠标模式();
        Curves currentCurves = null;
        Stack<Curves> undoCurves = new Stack<Curves>();
        Stack<Curves> redoCurves = new Stack<Curves>();
        bool Modified = false;
        Point P1, P2;
        UR模式 urMode = new UR模式();
        

        enum UR模式
        {
            None = 0,
            Undo = 1,
            Redo = 2,
    }

        enum 鼠标模式
        {
            选择 = 0,
            编辑 = 1,
            拖动 = 2,
        }

        public Form1()
        {
            InitializeComponent();
            toolStripButton1.Enabled = false;
            toolStripButton3.Enabled = false;
            toolStripButton4.Enabled = false;
            if (listBox1.Items.Count > 0)
                    listBox1.SelectedIndex = 0;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void DrawCurve(Graphics g)
        {
            g.Clear(Color.White);
            Rectangle rect = new Rectangle(10, 10, pictureBox1.Width - 20, pictureBox1.Height - 20);
            if (urMode == UR模式.Undo)
            {
                if(undoCurves.Peek()!=null)
                undoCurves.Peek().pCurves[0].Draw(g, rect);
            }
            else if (urMode == UR模式.Redo)
            {
                if (redoCurves.Peek() != null)
                    redoCurves.Peek().pCurves[0].Draw(g, rect);
            }
            else if (地球物理曲线 != null)
                地球物理曲线.Draw(g, rect);


        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (地球物理曲线 == null) return;
            Graphics g = e.Graphics;
            DrawCurve(g);
            if (bMouseDown && 地球物理曲线.SelectedCount == 0 && mouseMode == 鼠标模式.选择)
            {
                int x = Math.Min(P1.X, P2.X);
                int y = Math.Min(P1.Y, P2.Y);
                int width = Math.Abs(P1.X - P2.X);
                int height = Math.Abs(P1.Y - P2.Y);
                g.DrawRectangle(Pens.RoyalBlue, x, y, width, height);

            }

        }



        private void 读取电测深数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
       
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in ofd.FileNames)
                {
                    try
                    {
                        地球物理曲线 = new 电测深曲线类();
                        propertyGrid1.SelectedObject = 地球物理曲线;
                        地球物理曲线.Load(filename);
                        曲线列表.Add(地球物理曲线);
                        listBox1.Items.Add(Path.GetFileName(filename));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("数据读取错误，错误信息为：" + ex.Message);
                    }
                }
                pictureBox1.Invalidate();
                listBox1.SelectedIndex = 0;
            }
        }


        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void 读取大地测量数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in ofd.FileNames)
                {
                    try
                    {
                        地球物理曲线 = new 大地电磁曲线类();
                        propertyGrid1.SelectedObject = 地球物理曲线;
                        地球物理曲线.Load(filename);
                        曲线列表.Add(地球物理曲线);
                        listBox1.Items.Add(Path.GetFileName(filename));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("数据读取错误，错误信息为：" + ex.Message);
                    }
                }
                pictureBox1.Invalidate();
                listBox1.SelectedIndex = 0;
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            pictureBox1.Cursor = Cursors.Arrow;
            mouseMode = 鼠标模式.选择;
            buttons_Enable();
            toolStripButton1.Enabled = false;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            pictureBox1.Cursor = Cursors.SizeNS;
            mouseMode = 鼠标模式.编辑;
            buttons_Enable();
            toolStripButton2.Enabled = false;
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            pictureBox1.Cursor = Cursors.Hand;
            mouseMode = 鼠标模式.拖动;
            buttons_Enable();
            toolStripButton8.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            urMode = UR模式.Undo;
            pictureBox1.Refresh();
            var cur = new Curves(); 
            cur.pCurves.Add(地球物理曲线.Clone());//修改后状态
            redoCurves.Push(cur.Copy());
            var snap = undoCurves.Pop();//修改前状态
            地球物理曲线 = snap.pCurves[0].Clone();//恢复修改前状态
            if (listBox1.SelectedIndex >= 0)
            {
                曲线列表[listBox1.SelectedIndex] = 地球物理曲线;
                propertyGrid1.SelectedObject = 地球物理曲线;
            }
            urMode = UR模式.None;
            if (redoCurves.Count > 0) toolStripButton4.Enabled = true;
            if (undoCurves.Count == 0) toolStripButton3.Enabled = false;
            

        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            urMode = UR模式.Redo;
            pictureBox1.Refresh();
            var cur = new Curves(); 
            cur.pCurves.Add(地球物理曲线.Clone());//undo后状态
            undoCurves.Push(cur.Copy());
            var snap = redoCurves.Pop();//undo前状态
            地球物理曲线 = snap.pCurves[0].Clone();//恢复undo前状态
            if (listBox1.SelectedIndex >= 0)
            {
                曲线列表[listBox1.SelectedIndex] = 地球物理曲线;
                propertyGrid1.SelectedObject = 地球物理曲线;
            }
            urMode = UR模式.None;
            if (undoCurves.Count > 0) toolStripButton3.Enabled = true;
            if (redoCurves.Count == 0) toolStripButton4.Enabled = false;
            
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (地球物理曲线 == null) return;
            currentCurves = new Curves();
            currentCurves.pCurves.Add(地球物理曲线.Clone());
            undoCurves.Push(currentCurves.Copy());
            toolStripButton3.Enabled = true;
            redoCurves.Clear();
            toolStripButton4.Enabled = false;
            地球物理曲线.ZoomIn();
            pictureBox1.Invalidate();
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (地球物理曲线 == null) return;
            currentCurves = new Curves();
            currentCurves.pCurves.Add(地球物理曲线.Clone());
            undoCurves.Push(currentCurves.Copy());
            toolStripButton3.Enabled = true;
            redoCurves.Clear();
            toolStripButton4.Enabled = false;
            地球物理曲线.ZoomOut();
            pictureBox1.Invalidate();
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (地球物理曲线 == null) return;
            currentCurves = new Curves();
            currentCurves.pCurves.Add(地球物理曲线.Clone());
            undoCurves.Push(currentCurves.Copy());
            toolStripButton3.Enabled = true;
            redoCurves.Clear();
            toolStripButton4.Enabled = false;
            地球物理曲线.ZoomReset();
            pictureBox1.Invalidate();
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (地球物理曲线 == null) return;
            if (e.Button == MouseButtons.Left)
            {
                Point p = e.Location;
                if (!bMouseDown && 地球物理曲线!=null && mouseMode == 鼠标模式.选择) 地球物理曲线.DoSelection(p);
                pictureBox1.Invalidate();
                P1 = p;
                bMouseDown = true;
                if(mouseMode == 鼠标模式.编辑)
                {
                    currentCurves = new Curves();
                    currentCurves.pCurves.Add(地球物理曲线.Clone());
                }
                
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (地球物理曲线 != null && bMouseDown && 地球物理曲线.SelectedCount > 0 && mouseMode != 鼠标模式.拖动)
            {
                Point p = e.Location;
                double dy = Math.Abs(p.Y - P1.Y);
                if (dy > 5)
                {
                    if (mouseMode == 鼠标模式.编辑)
                    {
                        地球物理曲线.DoModify(P1, p);
                        Modified = true;

                    }
                    pictureBox1.Invalidate();
                }
                P1 = p;

            }
            else if (bMouseDown && 地球物理曲线 != null && mouseMode == 鼠标模式.拖动)
            {
                Point p = e.Location;
                地球物理曲线.DoPan(P1, p);
                P1 = p;
                pictureBox1.Invalidate();

            }
            else if (bMouseDown && e.Button == MouseButtons.Left)
            {
                P2 = e.Location;
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {   
            P2 = e.Location;
            
            double dy = Math.Abs(P1.Y - P2.Y);
            double dx = Math.Abs(P1.X - P2.X);
            if (Math.Min(dx,dy) > 5 && bMouseDown && 地球物理曲线!=null && mouseMode == 鼠标模式.选择)
            {
                地球物理曲线.DoSelection(P1, P2);
                pictureBox1.Invalidate();
            }
            if (Modified)
            {
                undoCurves.Push(currentCurves.Copy());
                Modified = false;
                toolStripButton3.Enabled = true;
                redoCurves.Clear();
                toolStripButton4.Enabled = false;
            }
            bMouseDown = false;
            
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            地球物理曲线 = 曲线列表[listBox1.SelectedIndex];
            propertyGrid1.SelectedObject = 地球物理曲线;
            pictureBox1.Invalidate();
            pictureBox1.Cursor = Cursors.Arrow;
            mouseMode = 鼠标模式.选择;
            buttons_Enable();
            toolStripButton1.Enabled = false;
            toolStripButton3.Enabled = false;
            toolStripButton4.Enabled = false;
            redoCurves.Clear();
            undoCurves.Clear();
            Modified = false;

        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About 关于窗口 = new About();
            关于窗口.Show();
        }

        private void 数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 帮助HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/honghaozhang564/WindowsFormsApplicationzhh/blob/main/%E5%B8%AE%E5%8A%A9.md";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        private void 保存数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (地球物理曲线 != null)
            {
                地球物理曲线.Save(地球物理曲线.FileName);
                MessageBox.Show("数据已保存！");
            }
            else
            {
                MessageBox.Show("没有数据保存！");
            }
        }

        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoCurves.Count == 0) return;
            toolStripButton3_Click(sender, e);
        }

        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoCurves.Count == 0) return;
            toolStripButton4_Click(sender, e);
        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton5_Click(sender, e);
        }
        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton6_Click(sender, e);
        }
        private void 平移ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            toolStripButton8_Click(sender, e);
        }
        private void 还原ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton7_Click(sender, e);
        }

        private void buttons_Enable()
        {
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = true;
            toolStripButton8.Enabled = true;
        }
    }

}
