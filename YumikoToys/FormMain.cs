using HtmlAgilityPack;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Application = System.Windows.Forms.Application;

namespace YumikoToys
{
    public partial class FormMain : Form
    {
        //DisplayInformation displayInfo;

        private static NotifyIcon notify;
        private static Dictionary<Guid, MenuItem> plans = new Dictionary<Guid, MenuItem>();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern void LockWorkStation();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);
        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        private static extern uint PowerEnumerate(IntPtr RootPowerKey, IntPtr SchemeGuid, IntPtr SubGroupOfPowerSettingGuid, UInt32 AcessFlags, UInt32 Index, ref Guid Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        private static extern uint PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, IntPtr SubGroupOfPowerSettingGuid, IntPtr PowerSettingGuid, IntPtr Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        private static extern uint PowerSetActiveScheme(IntPtr RootPowerKey, ref Guid SchemeGuid);

        [DllImport("PowrProf.dll", CharSet = CharSet.Unicode)]
        private static extern uint PowerGetActiveScheme(IntPtr RootPowerKey, ref IntPtr ActivePolicyGuid);


        public enum AccessFlags : uint
        {
            ACCESS_SCHEME = 16,
            ACCESS_SUBGROUP = 17,
            ACCESS_INDIVIDUAL_SETTING = 18
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        PrivateFontCollection pfc = new PrivateFontCollection();
        public const int WM_SYSCOMMAND = 274;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        //private static uint SC_MONITORPOWER = 0xF170;
        private const int HWND_BROADCAST = 0xFFFF;
        private int SC_MONITORPOWER = 61808;

        /// <summary>
        /// 通过Windows的API控制窗体的拖动
        /// </summary>
        /// <param name="hwnd"></param>

        public FormMain()
        {
            InitializeComponent();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            version.Focus();
            //this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
        }

        public static void MoveDown(IntPtr hwnd)
        {
            ReleaseCapture();
            SendMessage(hwnd, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }



        private void minbox_Click(object sender, EventArgs e)
        {
            version.Focus();
            Hide();
            NotifyIcon.Visible = true;
            NotifyIcon.ShowBalloonTip(1000);
        }

        private void ExitBox_Click(object sender, EventArgs e)
        {
            //NotifyIcon.Dispose();
            //notify.Dispose();
            //清除两个托盘图标
            version.Focus();
            System.Environment.Exit(System.Environment.ExitCode);
            NotifyIcon.Dispose();
            notify.Dispose();
            notify.Visible = false;
            notify.Icon = null;
            notify.Icon.Dispose();
            while (notify.Visible)
            {
                Application.DoEvents();
            }
            TaskBarUtil.RefreshNotification();
            this.Dispose();
            this.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Application.ExitThread();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            version.Focus();
            DialogResult MsgBoxResult;
            MsgBoxResult = MessageBox.Show("马上关闭显示器吗", "确定要进行这个操作吗", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (MsgBoxResult == DialogResult.Yes)
            {
                //PowerHelpers.TurnOffMonitor();
                SendMessage(FindWindow((string)null, (string)null).ToInt32(), WM_SYSCOMMAND, this.SC_MONITORPOWER, 2);


            }
            if (MsgBoxResult == DialogResult.No)
            {
                MessageBox.Show("你取消了操作", "没有发生任何事", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            NotifyIcon.Visible = false;
        }

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            MoveDown(this.Handle);
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            MoveDown(this.Handle);
        }

        private void Help_Click(object sender, EventArgs e)
        {
            version.Focus();
            Form AboutToys = new About();
            AboutToys.ShowDialog();
            //MessageBox.Show("版本@0.1\r\n1.一个WPF开发的简陋版本\r\n\r\n版本@1.0.1 Base\r\n1.重绘窗体界面\r\n2.重构关闭逻辑代码\r\n\r\n版本@1.0.2\r\n1.移除Base和Lite版本标签\r\n2.修复部分修改版系统无法休眠的bug\r\n3.尝试兼容最新预览版并降低绿屏死机概率\r\n\r\n版本@1.1.0\r\n1.改名为YumikoToys Lite\r\n2.修复某些情况下Win11可能出现奇怪的问题\r\n3.使用VS 2022 Peview重新编译优化大小\r\n\r\n版本@1.1.2\r\n1.支持最新的Windows 11 Dev预览版系统\r\n2.修复了一些小问题", "更新日志", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static string ReadFriendlyName(Guid schemeGuid)
        {
            uint sizeName = 1024;
            IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);

            string friendlyName;

            try
            {
                PowerReadFriendlyName(IntPtr.Zero, ref schemeGuid, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName);
                friendlyName = Marshal.PtrToStringUni(pSizeName);
            }
            finally
            {
                Marshal.FreeHGlobal(pSizeName);
            }

            return friendlyName;
        }

        public static IEnumerable<Guid> GetAll()
        {
            var schemeGuid = Guid.Empty;

            uint sizeSchemeGuid = (uint)Marshal.SizeOf(typeof(Guid));
            uint schemeIndex = 0;

            while (PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint)AccessFlags.ACCESS_SCHEME, schemeIndex, ref schemeGuid, ref sizeSchemeGuid) == 0)
            {
                yield return schemeGuid;
                schemeIndex++;
            }
        }

        public static void SetActiveScheme(Guid schemeGuid)
        {
            PowerSetActiveScheme(IntPtr.Zero, ref schemeGuid);
        }

        public static Guid GetActiveScheme()
        {
            IntPtr activeGuid = IntPtr.Zero;

            if (PowerGetActiveScheme(IntPtr.Zero, ref activeGuid) != 0)
                throw new Win32Exception();

            return (Guid)Marshal.PtrToStructure(activeGuid, typeof(Guid));
        }






        static string RunProcess(string executable, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using (Process proc = new Process())
            {
                proc.StartInfo = startInfo;
                proc.Start();
                string stdout = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new InvalidOperationException(executable + " " + arguments + " returned non-zero exit code " + proc.ExitCode);
                }
                return stdout;
            }
        }






        private void FormMain_Load(object sender, EventArgs e)
        {
            //加载字体
            byte[] fontRegular = Properties.Resources.PingFang_Jian;
            BaseFont font = BaseFont.CreateFont("PingFang_Jian.ttf", BaseFont.WINANSI, BaseFont.EMBEDDED, BaseFont.CACHED, fontRegular, null);
            int fontLength = Properties.Resources.PingFang_Jian.Length;

            // create a buffer to read in to
            byte[] fontdata = Properties.Resources.PingFang_Jian;

            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);

            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);
            //改变字体
            title.UseCompatibleTextRendering = true;
            title.Font = new Font(pfc.Families[0], label1.Font.Size);
            label1.UseCompatibleTextRendering = true;
            label1.Font = new Font(pfc.Families[0], label1.Font.Size);
            version.Focus();
            //label1.Text = "工具运行时，你的设备不会进入睡眠状态\r\n这可以让你可以执行羞羞(∩•̀ω•́)⊃--*⋆的东西或通过投屏实现播放\r\n而不必担心因为按下电源按钮或关闭显示器时设备会进入睡眠状态\r\n你可以使用设备上的电源按钮来关闭显示器";


            // Load all the power plans.
            Power.GetSchemes();
            var guidPlans = GetAll();

            foreach (Guid guidPlan in guidPlans)
            {
                //Console.WriteLine(ReadFriendlyName(guidPlan));
                ChangePowerPlan.Text = "托盘图标右键菜单";
                nowpowerPlan.Text = ReadFriendlyName(guidPlan);
                PowerStatus pwr = SystemInformation.PowerStatus;

                String strBatteryChargingStatus;
                strBatteryChargingStatus = pwr.BatteryChargeStatus.ToString();
                PowerStatus status = SystemInformation.PowerStatus;
                BatteryPercent.Text = status.BatteryLifePercent.ToString("P0");

                var saveLocation = System.AppDomain.CurrentDomain.BaseDirectory + "battery-report.html";
                if (File.Exists(saveLocation))
                {
                    // 创建一个 WebClient 对象
                    WebClient client = new WebClient();
                    // 下载网页的 HTML 内容
                    string batteryreporthtml = client.DownloadString(saveLocation);
                    // 将网页的 HTML 内容保存到 .txt 文件中
                    File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "bp.txt", batteryreporthtml);
                    string report = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "bp.txt");
                    // 正则表达式匹配 Design Capacity
                    Regex regex = new Regex(@"\d +,\d +\s + mWh");
                    Match match = regex.Match(report);

                    // 获取匹配结果
                    if (match.Success)
                    {
                        Console.WriteLine("Design Capacity: " + match.Groups[1].Value + " mWh");
                        DesignResultText.Text = match.Groups[1].Value;
                    }
                }
                else
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C powercfg /batteryreport /output " + '"' + saveLocation + '"';
                    process.StartInfo = startInfo;
                    process.Start();
                    //MessageBox.Show("已生成电池数据\r\n重启程序后即可查看数据", "成功初始化", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private static void UpdatePowerPlans()
        {
            // Get the currently active scheme.
            Guid activeScheme = Power.GetActiveScheme();

            IEnumerable<Guid> availablePlans = Power.GetSchemes();

            // Create list of Guids to remove (by default all).
            List<Guid> remove = new List<Guid>(plans.Keys);

            int index = 0;

            // Update all current plans.
            foreach (Guid guid in availablePlans)
            {
                string name = Power.ReadFriendlyName(guid);
                bool active = guid == activeScheme;

                if (!plans.ContainsKey(guid))
                {
                    // Create the menu item.
                    //MenuItem plan = new MenuItem();
                    //plan.Index = index;
                    //plan.Text = name;
                    //plan.Checked = active;
                    //plan.Click += (obj, e) =>
                    {
                        Power.SetActiveScheme(guid);
                    };

                    // Add the menu item to the dictionary.
                    //plans[guid] = plan;

                    //notify.ContextMenu.MenuItems.Add(index++, plan);
                }
                else
                {
                    // Update the menu item details.
                    plans[guid].Text = name;
                    plans[guid].Checked = active;
                }

                // We won't remove this item anymore.
                remove.Remove(guid);
            }

            // Remove all stale plans.
            foreach (Guid guid in remove)
            {
                //notify.ContextMenu.MenuItems.Remove(plans[guid]);
                plans[guid].Dispose();
                plans.Remove(guid);
            }
        }


        private static void OnMenuPopup(object sender, EventArgs args)
        {
            UpdatePowerPlans();
        }

        /// <summary>
        /// Construct all of the initial menu items.
        /// </summary>
        /// <returns>An array of MenuItems.</returns>
        //private static MenuItem[] CreateMenuItems()
        //{
        //MenuItem separator = new MenuItem();
        //separator.Text = "-";

        //MenuItem exitMenuItem = new MenuItem();
        //exitMenuItem.Text = Resources.strings.ResourceManager.GetString("Close", CultureInfo.CurrentUICulture);
        //exitMenuItem.Click += new EventHandler(ExitMenu);

        //return new MenuItem[] { separator, exitMenuItem };
        //}

        /// <summary>
        /// Callback for exiting the application.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        ///private static void ExitMenu(object Sender, EventArgs e)
        //{
        //    Application.Exit();
        //}


        private void title_Click(object sender, EventArgs e)
        {
            //Startup.SetStartup(true);
            //NotifyIcon.ShowBalloonTip(30, "程序已跟随Windows启动", "已设置自启", ToolTipIcon.Info);
        }

        private void noRotation_CheckedChanged(object sender, EventArgs e)
        {
            noRotation.Image = Properties.Resources.click2_2x;
            noRotationText.ForeColor = Color.HotPink;
            rotate90.Image = Properties.Resources.checkbox__2x;
            rotate90Text.ForeColor = Color.Black;
            rotate180.Image = Properties.Resources.checkbox__2x;
            rotate180Text.ForeColor = Color.Black;
            rotate270.Image = Properties.Resources.checkbox__2x;
            rotate270Text.ForeColor = Color.Black;
            Display.Rotate(1, Display.Orientations.DEGREES_CW_0);
        }

        private void rotate90_CheckedChanged(object sender, EventArgs e)
        {
            noRotation.Image = Properties.Resources.checkbox__2x;
            noRotationText.ForeColor = Color.Black;
            rotate90.Image = Properties.Resources.click2_2x;
            rotate90Text.ForeColor = Color.HotPink;
            rotate180.Image = Properties.Resources.checkbox__2x;
            rotate180Text.ForeColor = Color.Black;
            rotate270.Image = Properties.Resources.checkbox__2x;
            rotate270Text.ForeColor = Color.Black;
            Display.Rotate(1, Display.Orientations.DEGREES_CW_90);
        }

        private void rotate180_CheckedChanged(object sender, EventArgs e)
        {
            noRotation.Image = Properties.Resources.checkbox__2x;
            noRotationText.ForeColor = Color.Black;
            rotate180.Image = Properties.Resources.click2_2x;
            rotate180Text.ForeColor = Color.HotPink;
            rotate90.Image = Properties.Resources.checkbox__2x;
            rotate90Text.ForeColor = Color.Black;
            rotate270.Image = Properties.Resources.checkbox__2x;
            rotate270Text.ForeColor = Color.Black;
            Display.Rotate(1, Display.Orientations.DEGREES_CW_180);
        }

        private void rotate270_CheckedChanged(object sender, EventArgs e)
        {
            noRotation.Image = Properties.Resources.checkbox__2x;
            noRotationText.ForeColor = Color.Black;
            rotate270.Image = Properties.Resources.click2_2x;
            rotate270Text.ForeColor = Color.HotPink;
            rotate180.Image = Properties.Resources.checkbox__2x;
            rotate180Text.ForeColor = Color.Black;
            rotate90.Image = Properties.Resources.checkbox__2x;
            rotate90Text.ForeColor = Color.Black;
            Display.Rotate(1, Display.Orientations.DEGREES_CW_270);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //无操作
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            NotifyIcon.Dispose();
            notify.Dispose();
            notify.Visible = false;
            notify.Icon = null;
            notify.Icon.Dispose();
            //notify.Visible = false;
            TaskBarUtil.RefreshNotification();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            NotifyIcon.Dispose();
            notify.Dispose();
            notify.Visible = false;
            notify.Icon = null;
            notify.Icon.Dispose();
            //notify.Visible = false;
            TaskBarUtil.RefreshNotification();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //Font font = new Font(pfc.Families[0], 16);
            e.Graphics.DrawString("不休眠关闭显示器", this.Font, Brushes.White, 160, 10);
            //StringAlignment.Center;
        }

        private void label1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //Font font = new Font(pfc.Families[0], 16);
            //g.DrawString("工具运行时，你的设备不会进入睡眠状态\r\n这可以让你可以执行羞羞(∩•̀ω•́)⊃--*⋆的东西或通过投屏实现播放\r\n而不必担心因为按下电源按钮或关闭显示器时设备会进入睡眠状态\r\n你可以使用设备上的电源按钮来关闭显示器", font, new SolidBrush(Color.Black), 49, 47);
        }

        private void noRotation_MouseClick(object sender, MouseEventArgs e)
        {
            //noRotation.Image = Properties.Resources.click2_2x;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DESIGNCAPACITYBox_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawString("未获取", this.Font, Brushes.White, 40, -5);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FULLCHARGECAPACITYBox_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawString("未获取", this.Font, Brushes.White, 40, 10);
        }


        /*
            Startup.SetStartup(false);
            NotifyIcon.ShowBalloonTip(30, "程序已移除Windows启动", "已禁用自启", ToolTipIcon.Info);
       */
    }
}
