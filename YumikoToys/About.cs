using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YumikoToys
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Startup.SetStartup(true);
            //NotifyIcon.ShowBalloonTip(30, "程序已跟随Windows启动", "已设置自启", ToolTipIcon.Info);
        }
    }
}
