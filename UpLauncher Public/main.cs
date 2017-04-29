using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpLauncher_Public.utils;
using UpLauncher_Public.world;

namespace UpLauncher_Public
{
    public partial class main : Form
    {
        private UpLauncher launcher;
        public bool first;

        public main()
        {
            if (Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name).Length > 1)
            {
                MessageBox.Show("Votre program est déjà en cour d'éxécution.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                close_process(null, null);
                return;
            }
            this.FormClosing += new FormClosingEventHandler(close_process);
            InitializeComponent();
            config.BTN_REFRESH = this.refresh;
            first = true;
            this.new_loading();
        }

        public void check_new_upl()
        {
            try
            {
                WebClient client = new WebClient();
                string md5 = client.DownloadString(config.MAIN_URL + "check_upl.php");
                if (md5.Length > 0)
                {
                    if (!cryptor.fileCryptor_md5(Assembly.GetExecutingAssembly().GetName().Name + ".exe").Equals(md5))
                    {
                        MessageBox.Show("Nouvelle version disponible !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        string now = Assembly.GetExecutingAssembly().GetName().Name + ".exe";
                        string old = Assembly.GetExecutingAssembly().GetName().Name + "_old.exe";
                        if (File.Exists(old))
                            File.Delete(old);
                        File.Move(now, old);
                        client = new WebClient();
                        client.DownloadFile(new Uri(config.MAIN_URL + "UpLauncher Public.exe"), now);
                        while (client.IsBusy) ;
                        Process.Start(now);
                        close_process(null, null);
                    }
                }
            }
            catch (Exception e) {
                string old = (Assembly.GetExecutingAssembly().GetName().Name + ".exe").Replace("_old", "");
                if (File.Exists(old))
                    File.Delete(old);
                File.Move(Assembly.GetExecutingAssembly().GetName().Name + ".exe", old);
            }
        }

        private void new_loading()
        {
            this.Name = "0";
            launcher = new UpLauncher(this);
            set_main_function();
        }

        private void set_main_function()
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                if (config.MODE_DEBUG)
                    Debug.WriteLine(Environment.NewLine + "Form movement add.");
                this.MouseDown += new MouseEventHandler(mouse.mouse_down);
                this.MouseMove += new MouseEventHandler(mouse.mouse_move);
            }
        }

        private static void close_process(object sender, FormClosingEventArgs e)
        {
            if (UpLauncher.upd != null)
                UpLauncher.upd.close = true;
            Process my = Process.GetCurrentProcess();
            clear_all();
            if (config.MODE_DEBUG)
                Debug.WriteLine(Environment.NewLine + "Stop the process.");
            my.Kill();
        }

        private static void clear_all()
        {
            foreach (component ctrl in UpLauncher.components.Values)
            {
                ctrl.kill();
            }
            foreach (List<object> ctrl in UpLauncher.timers.Values)
            {
                System.Windows.Forms.Timer timer = null;
                foreach (object obj in ctrl)
                {
                    if (obj is System.Windows.Forms.Timer)
                    {
                        timer = obj as System.Windows.Forms.Timer;
                        timer.Stop();
                        timer.Dispose();
                    }
                }
            }
            if (UpLauncher.upd != null)
                UpLauncher.upd.close = true;
            UpLauncher.upd = null;
            UpLauncher.timers.Clear();
            UpLauncher.components.Clear();
            action.list.Clear();
            action.listc.Clear();
        }

        private void refresh_click(object sender, EventArgs e)
        {
            clear_all();
            this.new_loading();
        }
    }
}
