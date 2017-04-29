using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpLauncher_Public
{
    class config
    {
        //Admin
        public static string MAIN_URL = "http://upl-public.alwaysdata.net/"; //Url du site de location
        public static bool MODE_DEBUG = false;
        public static Control BTN_REFRESH = null;

        //Client
        public static string CHILD_URL = ""; //Url de locaion
        public static string PATH_DESIGN = "upl\\design.config"; //Chemin d'accés local du fichier design.config
    }
}
