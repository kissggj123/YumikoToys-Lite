using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YumikoToys
{ 
    internal class PowerBoard
    {
        private int WM_SYSCOMMAND = 274;
        private int SC_MONITORPOWER = 61808;
        private const int MONITOR_OFF = 2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public void TurnOffScreen()
        {
            PowerBoard.SendMessage(PowerBoard.FindWindow((string)null, (string)null).ToInt32(), this.WM_SYSCOMMAND, SC_MONITORPOWER, 2);
        }
    }
}
