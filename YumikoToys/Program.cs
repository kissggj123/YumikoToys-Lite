using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using YumikoToys.Properties;

namespace YumikoToys
{
    static class Program
    {
        private static NotifyIcon notify;
        private static Dictionary<Guid, MenuItem> plans = new Dictionary<Guid, MenuItem>();
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);



            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);

            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                Power.GetSchemes();

                notify = new NotifyIcon();

                ContextMenu menu = new ContextMenu();
                menu.Popup += new EventHandler(OnMenuPopup);
                menu.MenuItems.AddRange(CreateMenuItems());

                notify.Icon = Properties.Resources.Yumikoico;
                notify.Text = Resources.strings.Title;
                notify.ContextMenu = menu;
                notify.Visible = true;

                // Do initial power plan update.
                UpdatePowerPlans();
                //Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            else
            {

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;

                startInfo.Verb = "runas";
                try
                {
                    //Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch
                {
                    return;
                }

                //notify.Dispose();
                //notify.Visible = false;
                //notify.Icon = null;
                //notify.Icon.Dispose();
                Application.Exit();
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
                    MenuItem plan = new MenuItem();
                    plan.Index = index;
                    plan.Text = name;
                    plan.Checked = active;
                    plan.Click += (obj, e) =>
                    {
                        Power.SetActiveScheme(guid);
                    };

                    // Add the menu item to the dictionary.
                    plans[guid] = plan;

                    notify.ContextMenu.MenuItems.Add(index++, plan);
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
                notify.ContextMenu.MenuItems.Remove(plans[guid]);
                plans[guid].Dispose();
                plans.Remove(guid);
            }
        }

        /// <summary>
        /// Called whenever the context menu opens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnMenuPopup(object sender, EventArgs args)
        {
            UpdatePowerPlans();
        }

        /// <summary>
        /// Construct all of the initial menu items.
        /// </summary>
        /// <returns>An array of MenuItems.</returns>
        private static MenuItem[] CreateMenuItems()
        {
            MenuItem separator = new MenuItem();
            separator.Text = "-";

            MenuItem exitMenuItem = new MenuItem();
            exitMenuItem.Text = Resources.strings.ResourceManager.GetString("Close", CultureInfo.CurrentUICulture);
            exitMenuItem.Click += new EventHandler(ExitMenu);

            return new MenuItem[] { separator, exitMenuItem };
        }

        /// <summary>
        /// Callback for exiting the application.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private static void ExitMenu(object Sender, EventArgs e)
        {
            MessageBox.Show("程序正在运行 无法关闭", "无法实现操作", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //notify.Dispose();
            //notify.Visible = false;
            //notify.Icon = null;
            //notify.Icon.Dispose();
            //Application.Exit();
        }
    }
}
