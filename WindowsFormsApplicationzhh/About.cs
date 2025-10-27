using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplicationzhh
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var productAttr = (AssemblyProductAttribute)assembly.GetCustomAttribute(typeof(AssemblyProductAttribute));
            var CopyAttr = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
            var trademarkAttr = (AssemblyTrademarkAttribute)assembly.GetCustomAttribute(typeof(AssemblyTrademarkAttribute));
            var Version = assembly.GetName().Version.ToString();

            Text = productAttr.Product;

            label4.Text = CopyAttr.Copyright;
            label5.Text = Version;
            label6.Text = trademarkAttr.Trademark;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
