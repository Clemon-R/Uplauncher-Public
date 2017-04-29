using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace UpLauncher_Public.data
{
    class network
    {
        private WebClient client;

        public bool download_file(string url, string file)
        {
            if (client.IsBusy)
                return false;
            else
            {
                try
                {
                    string dir = file.Substring(0, file.LastIndexOf('\\'));
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    client.DownloadFile(new Uri(url), file);
                }catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return true;
        }

        public network(WebClient client)
        {
            this.client = client;
        }

        public WebClient get_client()
        {
            return this.client;
        }
    }
}
