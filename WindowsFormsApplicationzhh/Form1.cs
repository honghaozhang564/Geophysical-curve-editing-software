using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplicationzhh
{
    public partial class Form1 : Form
    {
        曲线类 地球物理曲线 = null;
        大地电磁曲线类 大地电磁曲线对象;
        List<曲线类> 曲线列表 = new List<曲线类>();
        bool bMouseDown = false;
        鼠标模式 mouseMode = new 鼠标模式();

        Point P1, P2;

        enum 鼠标模式
        {
            选择 = 0,
            编辑 = 1,
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 读取数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        void DrawCurve(Graphics g)
        {
            g.Clear(Color.White);
            Rectangle rect = new Rectangle(10,10, pictureBox1.Width - 20, pictureBox1.Height - 20);
            if (地球物理曲线 != null)
                地球物理曲线.Draw(g, rect);

        }
        

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (地球物理曲线 == null) return;
            Graphics g = e.Graphics;
            DrawCurve(g);
            if (bMouseDown && 地球物理曲线.SelectedCount == 0)
            {
                int x = Math.Min(P1.X, P2.X);
                int y = Math.Min(P1.Y, P2.Y);
                int width = Math.Abs(P1.X - P2.X);
                int height = Math.Abs(P1.Y - P2.Y);
                g.DrawRectangle(Pens.RoyalBlue, x, y, width, height);
                
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
        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (地球物理曲线!=null && bMouseDown && 地球物理曲线.SelectedCount>0 ) //修改
            {
                Point p = e.Location;
                double dy = Math.Abs(p.Y - P1.Y);
                if (dy > 5)
                {
                    if(mouseMode==鼠标模式.编辑) 地球物理曲线.DoModify(P1, p);
                    pictureBox1.Invalidate();
                }
                P1 = p;
                
            }
            else if (bMouseDown && e.Button == MouseButtons.Left) 
            {
                P2 = e.Location;
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p = e.Location;
                if (!bMouseDown && 地球物理曲线!=null && mouseMode == 鼠标模式.选择) 地球物理曲线.DoSelection(p);
                pictureBox1.Invalidate();
                P1 = p;
                bMouseDown = true;
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
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 图像ToolStripMenuItem_Click(object sender, EventArgs e)
        {

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



        private void buttons_Enable()
        {
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = true;
            toolStripButton3.Enabled = true;
        }
    }

}
