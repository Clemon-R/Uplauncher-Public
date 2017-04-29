using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using UpLauncher_Public.utils;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Net;
using System.Xml;
using UpLauncher_Public.world;

namespace UpLauncher_Public.data
{
    class update
    {
        private Thread start;
        private WebClient client;
        public Control infos;
        private bool locked;
        public bool close;
        public int percentage_file;
        public int percentage_udp;
        public string files;
        public string file;

        public update()
        {
            if (UpLauncher.upd != null)
                return;
            locked = false;
            close = false;
            percentage_udp = 0;
            percentage_file = 0;
            files = "";
            this.infos = null;
            start = new Thread(new ThreadStart(scanning));
            this.client = new WebClient();
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.download_finish);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.download_progress);
            UpLauncher.upd = this;
        }

        private void download_progress(object sender, DownloadProgressChangedEventArgs e)
        {
            percentage_file = e.ProgressPercentage;
        }

        private void download_finish(object sender, AsyncCompletedEventArgs e)
        {
            locked = false;
        }

        private void scanning()
        {
            if (close)
            {
                close = false;
                return;
            }
            action.load_action(4);
            if (infos != null)
                {
                    utils.utils.set_text(this.infos, "Téléchargement des informations...");
                    utils.utils.center_text(this.infos, UpLauncher.main.Size.Width);
                }
                if (config.MODE_DEBUG)
                    Debug.WriteLine("Download informations...");
                string args = client.DownloadString(config.CHILD_URL + "files.php").Replace("client/", "").Replace('/', '\\');
                if (infos != null)
                {
                    utils.utils.set_text(this.infos, "Vérification du/des fichier(s)...");
                    utils.utils.center_text(this.infos, UpLauncher.main.Size.Width);
                }
                if (config.MODE_DEBUG)
                    Debug.WriteLine("Check files...");
                if (this.close)
                {
                    close = false;
                    return;
                }
                string[] web = args.Split(';');
                int index = 0;
                double percent = 689 / (double)web.Length;
                foreach (string value in web)
                {
                    if (this.close)
                    {
                        close = false;
                        return;
                    }
                    else if (!value.Contains(","))
                        break;
                    action.load_action(5);
                    index++;
                    string[] arg = value.Split(',');
                    if (config.MODE_DEBUG)
                        Debug.WriteLine("Check file : " + arg[0]);
                file = arg[0];
                    if (!(File.Exists(arg[0]) && cryptor.fileCryptor_md5(arg[0]).Equals(arg[1])))
                    {
                        if (infos != null)
                        {
                            utils.utils.set_text(this.infos, "Téléchargement du fichier : " + arg[0]);
                            utils.utils.center_text(this.infos, UpLauncher.main.Size.Width);
                        }
                        if (config.MODE_DEBUG)
                            Debug.WriteLine("Download : " + arg[0]);
                        locked = true;
                        if (arg[0].Contains('\\'))
                        {
                            string dir = arg[0].Substring(0, arg[0].LastIndexOf('\\'));
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                        }
                        client.DownloadFileAsync(new Uri(config.CHILD_URL + "client/" + arg[0].Replace('\\', '/')), arg[0]);
                        while (locked)
                            Thread.Sleep(10);
                    }
                files = index +"/"+ web.Length ;
                percentage_udp = (int)(100.00/ (float)web.Length * (float)index);
            }
            if (infos != null)
            {
                utils.utils.set_text(this.infos, "Mise à jour terminée.");
                utils.utils.center_text(this.infos, UpLauncher.main.Size.Width);
            }
            percentage_udp = 100;
            percentage_file = 100;
            if (config.MODE_DEBUG)
                Debug.WriteLine("End of the update.");
            if (this.close)
            {
                close = false;
                return;
            }
            action.load_action(6);
        }

        public Thread get_start()
        {
            return (this.start);
        }
    }
}
