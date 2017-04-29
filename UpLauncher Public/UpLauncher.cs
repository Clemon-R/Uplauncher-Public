using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpLauncher_Public.data;
using UpLauncher_Public.world;
using UpLauncher_Public;
using System.Windows.Forms;
using System.Diagnostics;

namespace UpLauncher_Public
{
    class UpLauncher
    {
        public static network client;
        public static design design;
        public static main main;
        public static update upd;
        public static Dictionary<int, component> components;
        public static Dictionary<int, List<object>> timers;

        public UpLauncher(main m)
        {
            main = m;
            components = new Dictionary<int, component>();
            timers = new Dictionary<int, List<object>>();
            client = new network(new System.Net.WebClient());
            if (!client.get_client().DownloadString(config.MAIN_URL + "check.php").Equals("1"))
            {
                MessageBox.Show("UpLauncher désactiver.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UpLauncher.main.Close();
                return;
            }
            new design(main, client, this, true);

            if (!design.get_design_set()) //Si aucune désign présent
            {
                if (config.MODE_DEBUG)
                    Debug.WriteLine("Design not loaded." + Environment.NewLine + "Form closed");
                if (!design.get_design_set()) //Si toujours aucune fermer l'application
                    m.Close();
            }
        }
    }
}
