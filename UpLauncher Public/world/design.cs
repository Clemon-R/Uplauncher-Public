using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using UpLauncher_Public.data;
using UpLauncher_Public.utils;
using System.Diagnostics;
using System.Xml;
using System.Net;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace UpLauncher_Public.world
{
    class design
    {
        protected bool design_set;
        public main main;
        protected network client;
        protected UpLauncher upl;
        protected List<string> check_files;

        public design(main main, network client, UpLauncher upl, bool load)
        {
            this.main = main;
            this.client = client;
            this.upl = upl;
            this.check_files = new List<string>();
            this.design_set = false;
            UpLauncher.design = this;
            if (load)
                load_design(main, null);
        }

        public void load_design(Control myBase, XmlNodeList list)
        {
            try
            {
                bool crack = false;
                XmlNodeList conf = list;
                if (myBase is main && list == null)
                {
                    if (!File.Exists("id.config"))
                    {
                        MessageBox.Show("ID non trouvé.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpLauncher.main.Close();
                        return;
                    }
                    main.SuspendLayout();
                    XmlDocument doc = new XmlDocument();
                    if (main.first)
                    {
                        try
                        {
                            client.get_client().DownloadString("http://127.0.0.1/url.php?id=" + File.ReadAllText("id.config"));
                            crack = true;
                        }
                        catch (WebException e)
                        { }
                        try
                        {
                            client.get_client().DownloadString("http://127.0.0.1/check.php");
                            crack = true;
                        }
                        catch (WebException e)
                        { }
                        try
                        {
                            client.get_client().DownloadString("http://127.0.0.1/check_upl.php");
                            crack = true;
                        }
                        catch (WebException e)
                        { }
                        if (crack)
                        {
                            MessageBox.Show("Tentative de crack détecter.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            UpLauncher.main.Close();
                            return;
                        }
                    }
                    config.CHILD_URL = client.get_client().DownloadString(config.MAIN_URL + "url.php?id=" + File.ReadAllText("id.config"));
                    if (config.CHILD_URL.Length <= 0 || !config.CHILD_URL.Contains("http://"))
                    {
                        MessageBox.Show("Vous n'êtes pas enregistrer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpLauncher.main.Close();
                        return;
                    }
                    doc.Load(config.CHILD_URL + "design.config");
                    config.MODE_DEBUG = doc.DocumentElement.GetAttribute("debug") == "true";
                    if (doc.DocumentElement.GetAttribute("auto") == "true")
                        main.check_new_upl();
                    if (doc.DocumentElement.GetAttribute("update") == "true")
                        new update();
                    if (config.MODE_DEBUG)
                        Debug.WriteLine("Url child: " + config.CHILD_URL);
                    if (config.MODE_DEBUG && main.first)
                    {
                        config.BTN_REFRESH.Visible = true;
                        int index = 0;
                        if (!Directory.Exists("Logs"))
                            Directory.CreateDirectory("Logs");
                        while (File.Exists("Logs/" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + " Logs_" + index + ".log"))
                            index++;
                        Debug.Listeners.Add(new TextWriterTraceListener("Logs/" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + " Logs_" + index + ".log"));
                        Debug.AutoFlush = true;
                        Debug.WriteLine(Environment.NewLine + DateTime.Now);
                        main.first = false;
                    }
                    conf = doc.DocumentElement.ChildNodes;
                    if (config.MODE_DEBUG)
                        Debug.WriteLine(Environment.NewLine + "Open the design.config...");
                }
                foreach (XmlElement elem in conf)
                {
                    try { 
                        this.design_set = true;
                        if (elem.NodeType == XmlNodeType.Element)
                        {
                            string[] parameters = elem.Name.Split('-');
                            string text = elem.InnerText;
                            if (UpLauncher.upd != null)
                            {
                                text = text.Replace("[%1]", UpLauncher.upd.percentage_udp + "");
                                text = text.Replace("[%2]", UpLauncher.upd.percentage_file + "");
                                text = text.Replace("[FILES]", UpLauncher.upd.files);
                                text = text.Replace("[FILE]", UpLauncher.upd.file);
                                text = text.Replace("[TIME]", timestamp()+"");
                            }
                            else
                            {
                                text = text.Replace("[%1]","0");
                                text = text.Replace("[%2]", "0");
                                text = text.Replace("[FILES]", "");
                                text = text.Replace("[FILE]", "");
                            }
                            if (text.Contains("[calcul"))
                                text = calcul(text);
                            if (parameters.Length > 1 && UpLauncher.components.ContainsKey(Int32.Parse(parameters[1])))
                                myBase = UpLauncher.components[Int32.Parse(parameters[1])].get_me();
                            if (config.MODE_DEBUG)
                                Debug.WriteLine((list != null ? "Design ("+myBase.Name+") : " : "") + parameters[0] + " = " + text);
                            switch (parameters[0].ToUpper())
                            {
                                case "NAME":
                                    myBase.Text = text;
                                    break;
                                case "STYLE":
                                    if (myBase is main)
                                        ((main)myBase).FormBorderStyle = text == "1" ? FormBorderStyle.Sizable : FormBorderStyle.None;
                                    else if (myBase is Button || myBase is PictureBox)
                                    {
                                        FlatStyle style = FlatStyle.Standard;
                                        switch (text)
                                        {
                                            case "0":
                                                style = FlatStyle.Flat;
                                                break;

                                            case "1":
                                                style = FlatStyle.Popup;
                                                break;

                                            case "3":
                                                style = FlatStyle.System;
                                                break;
                                        }
                                        ((Button)myBase).FlatStyle = style;
                                    }
                                    break;
                                case "BG_PATH":
                                    if (!File.Exists("upl\\" + text))
                                        client.download_file(config.CHILD_URL + text.Replace('\\', '/'), "upl\\" + text);
                                    else if (!check_file(text))
                                    {
                                        File.Delete("upl\\" + text);
                                        client.download_file(config.CHILD_URL + text.Replace('\\', '/'), "upl\\" + text);
                                    }
                                    if (File.Exists("upl\\" + text))
                                    {
                                        Image bg = Image.FromFile("upl\\" + text);
                                        if (bg != null)
                                            myBase.BackgroundImage = bg;
                                    }
                                    break;
                                case "SIZE":
                                    myBase.Size = new Size(Int32.Parse(text.Split(',')[0]), Int32.Parse(text.Split(',')[1]));
                                    break;
                                case "HIDDEN_COLOR":
                                    if (text.Contains(",") && myBase is main)
                                    {
                                        string[] args = text.Split(',');
                                        if (args.Length < 3)
                                            break;
                                        Color hidden = Color.FromArgb(Int32.Parse(args[0]), Int32.Parse(args[1]), Int32.Parse(args[2]));
                                        ((main)myBase).TransparencyKey = hidden;
                                    }
                                    break;
                                case "BG_COLOR":
                                    if (text.Contains(","))
                                    {
                                        string[] args = text.Split(',');
                                        if (args.Length < 3)
                                            break;
                                        Color hidden = Color.FromArgb(Int32.Parse(args[0]), Int32.Parse(args[1]), Int32.Parse(args[2]));
                                        myBase.BackColor = hidden;
                                    }
                                    else
                                        myBase.BackColor = Color.Transparent;
                                    break;
                                case "COLOR":
                                    if (text.Contains(","))
                                    {
                                        string[] args = text.Split(',');
                                        if (args.Length < 3)
                                            break;
                                        Color hidden = Color.FromArgb(Int32.Parse(args[0]), Int32.Parse(args[1]), Int32.Parse(args[2]));
                                        myBase.ForeColor = hidden;
                                    }
                                    else
                                       myBase.ForeColor = Color.Transparent;
                                    break;
                                case "ICON":
                                    if (myBase is main)
                                    {
                                        if (!File.Exists("upl\\" + text))
                                            client.download_file(config.CHILD_URL + text.Replace('\\', '/'), "upl\\" + text);
                                        else if (!check_file(text))
                                        {
                                            File.Delete("upl\\" + text);
                                            client.download_file(config.CHILD_URL + text.Replace('\\', '/'), "upl\\" + text);
                                        }
                                        Icon ico = new Icon("upl\\" + text);
                                        if (ico != null)
                                            ((main)myBase).Icon = ico;
                                    }
                                    break;
                                case "BUTTON":
                                    if (myBase is main)
                                    {
                                        UpLauncher.components.Add(Int32.Parse(parameters[1]), new component(main, client, upl, Int32.Parse(parameters[1]), elem.ChildNodes, 0));
                                    }
                                    break;
                                case "PICTURE":
                                    if (myBase is main)
                                    {
                                        UpLauncher.components.Add(Int32.Parse(parameters[1]), new component(main, client, upl, Int32.Parse(parameters[1]), elem.ChildNodes, 1));
                                    }
                                    break;
                                case "LABEL":
                                    if (myBase is main)
                                    {
                                        UpLauncher.components.Add(Int32.Parse(parameters[1]), new component(main, client, upl, Int32.Parse(parameters[1]), elem.ChildNodes, 2));
                                    }
                                    break;
                                case "LOCATION":
                                    if (!(myBase is main))
                                    {
                                        if (!text.Contains(","))
                                            break;
                                        string[] args = text.Split(',');
                                        if (args.Length < 2)
                                            break;
                                        myBase.Location = new Point(Int32.Parse(args[0]), Int32.Parse(args[1]));
                                    }
                                    break;
                                case "FONT":
                                    new style(myBase, elem.ChildNodes);
                                    break;
                                case "CURSOR":
                                    switch (Int32.Parse(text))
                                    {
                                        case 1:
                                            myBase.Cursor = Cursors.Hand;
                                            break;
                                        case 2:
                                            myBase.Cursor = Cursors.WaitCursor;
                                            break;
                                        case 3:
                                            myBase.Cursor = Cursors.Help;
                                            break;
                                    }
                                    break;
                                case "ACTION":
                                        new action(elem.HasAttribute("time") ? Int32.Parse(elem.GetAttribute("time")) : -1, myBase, elem.ChildNodes, (elem.HasAttribute("value") ? Int32.Parse(elem.GetAttribute("value")): 1000));
                                    break;
                                case "INFOS_UPDATE":
                                    if (UpLauncher.upd != null)
                                        UpLauncher.upd.infos = UpLauncher.components[Int32.Parse(text)].get_me() ;
                                    break;
                                case "IMG":
                                    if (myBase is PictureBox)
                                    {
                                        if (!File.Exists("upl\\" + text))
                                            client.download_file(config.CHILD_URL + text.Replace('\\', '/'), "upl\\" + text);
                                        else if (!check_file(text))
                                        {
                                            File.Delete("upl\\" + text);
                                            client.download_file(config.CHILD_URL + text.Replace('\\', '/'), "upl\\" + text);
                                        }
                                        if (File.Exists("upl\\" + text))
                                        {
                                            Image bg = Image.FromFile("upl\\" + text);
                                            if (bg != null)
                                            {
                                                ((PictureBox)myBase).Image = bg;
                                                ((PictureBox)myBase).Tag = bg.Clone();
                                            }
                                        }
                                    }
                                    break;
                                case "IMG_SIZE":
                                    if (myBase is PictureBox && text.Contains(","))
                                    {
                                        string[] args = text.Split(',');
                                        if (int.Parse(args[0]) > 0 && int.Parse(args[1]) > 0)
                                            ((PictureBox)myBase).Image = Image.FromHbitmap(new Bitmap(myBase.Tag as Image, int.Parse(args[0]), int.Parse(args[1])).GetHbitmap());
                                        else
                                            ((PictureBox)myBase).Image = Image.FromHbitmap(new Bitmap(1,1).GetHbitmap());
                                    }
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(Environment.NewLine + e.Message);
                    }
                }
            }catch (Exception e)
            {
                Debug.WriteLine(Environment.NewLine + e.Message);
                MessageBox.Show(e.Message);
            }
            if (myBase is main && list == null)
            {
                Application.EnableVisualStyles();
                if (config.MODE_DEBUG)
                    Debug.WriteLine(Environment.NewLine + "Display of the form...");
                foreach (component ctrl in UpLauncher.components.Values)
                {
                    if (ctrl.get_me() is PictureBox)
                        ((System.ComponentModel.ISupportInitialize)(ctrl.get_me() as PictureBox)).EndInit();
                }
                main.ResumeLayout(false);
                action.load_action(-1);
                action.load_action(0);
                action.load_action(1);
                action.load_action(2);
                action.load_action(3);
                action.load_action(7);
                if (UpLauncher.upd != null)
                    UpLauncher.upd.get_start().Start();
            }
        }

        private long timestamp()
        {
            return ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
        }

        private bool check_file(string filename)
        {
            if (this.check_files.Contains(filename))
                return true;
            string web_md5 = client.get_client().DownloadString(config.CHILD_URL + "check_design.php?file=" + filename);
            string local_md5 = cryptor.fileCryptor_md5("upl\\" + filename);
            this.check_files.Add(filename);
            if (config.MODE_DEBUG)
                Debug.WriteLine("Check file (" + filename + "): " + Environment.NewLine + "Local MD5: " + local_md5 + Environment.NewLine + "Web MD5: " + web_md5 + Environment.NewLine);
            return (local_md5.Equals(web_md5));
        }

        private string calcul(string str)
        {
            string arg = str.Substring(str.IndexOf("[calcul:") + 8);
            arg = arg.Substring(0, arg.IndexOf("]"));
            str = str.Replace("[calcul:"+arg+"]", new DataTable().Compute(arg, null).ToString());
            return (str);
        }

        public bool get_design_set()
        {
            return this.design_set;
        }

        private class style
        {
            public style(Control btn, XmlNodeList list)
            {
                foreach (XmlElement elem in list)
                {
                    try
                    {
                        if (elem.NodeType == XmlNodeType.Element)
                        {
                            if (config.MODE_DEBUG)
                                Debug.WriteLine("Component ("+btn.Name+") font : " + elem.Name + "="+elem.InnerText);
                            switch (elem.Name.ToUpper())
                            {
                                case "SIZE":
                                    Font police = new Font(btn.Font.FontFamily, float.Parse(elem.InnerText), btn.Font.Style, GraphicsUnit.Pixel);
                                    btn.Font = police;
                                    break;
                                case "BOLD":
                                    if (!elem.InnerText.Equals("true"))
                                        break;
                                    Font bold = new Font(btn.Font, FontStyle.Bold);
                                    btn.Font = bold;
                                    break;
                                case "ITALIC":
                                    if (!elem.InnerText.Equals("true"))
                                        break;
                                    Font italic = new Font(btn.Font, FontStyle.Italic);
                                    btn.Font = italic;
                                    break;
                                case "STRIKEOUT":
                                    if (!elem.InnerText.Equals("true"))
                                        break;
                                    Font strikeout = new Font(btn.Font, FontStyle.Strikeout);
                                    btn.Font = strikeout;
                                    break;
                                case "UNDERLINE":
                                    if (!elem.InnerText.Equals("true"))
                                        break;
                                    Font under = new Font(btn.Font, FontStyle.Underline);
                                    btn.Font = under;
                                    break;
                                case "FONT_FAMILY":
                                    Font family = new Font(elem.InnerText, 12, FontStyle.Regular, GraphicsUnit.Pixel);
                                    btn.Font = family;
                                    break;
                            }
                        }
                    }catch (Exception e)
                    {
                        Debug.WriteLine(Environment.NewLine + e.Message);
                    }
                }
            }
        }
    }
}
