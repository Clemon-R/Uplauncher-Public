using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UpLauncher_Public.utils
{
    class action
    {
        public static Dictionary<int, List<action>> list = new Dictionary<int, List<action>>();
        public static Dictionary<Control, List<action>> listc = new Dictionary<Control, List<action>>();
        private int time;
        private XmlNodeList actions;
        private Control ctrl;
        private int value;

        public action(int time, Control ctrl, XmlNodeList xml, int value) //Time = attribue
        {
            this.time = time;
            this.actions = xml;
            this.ctrl = ctrl;
            this.value = value;
            if (!list.ContainsKey(time))
                list.Add(time, new List<action>());
            list[time].Add(this);
            if (!listc.ContainsKey(ctrl))
                listc.Add(ctrl, new List<action>());
            listc[ctrl].Add(this);
        }

        public static void load_action(int time)
        {
            if (!list.ContainsKey(time))
                return;
            List<action> list_actions = list[time];
            foreach (action ac in list_actions)
            {
                if (ac.get_time() != time)
                    break;
                if (config.MODE_DEBUG)
                    Debug.WriteLine("Component ("+ac.get_ctrl().Name+") add action: Time " +time);
                switch (time)
                {
                    case 0:
                        ac.get_ctrl().MouseClick += new MouseEventHandler(play_click);
                        break;
                    case 1:
                        ac.get_ctrl().MouseHover += new EventHandler(play_hover);
                        break;
                    case 2:
                        ac.get_ctrl().MouseLeave += new EventHandler(play_leave);
                        break;
                    case 3:
                        ac.get_ctrl().MouseUp += new MouseEventHandler(play_release);
                        break;
                    case 4:
                    case 5:
                    case 6:
                        play_action(ac.get_ctrl(), time);
                        break;
                    case 7:
                        if (!UpLauncher.timers.ContainsKey(ac.get_value()))
                        {
                            Timer timer = new Timer();
                            timer.Interval = ac.get_value();
                            timer.Tick += new EventHandler(play_timer);
                            UpLauncher.timers.Add(ac.get_value(), new List<object>());
                            UpLauncher.timers[ac.get_value()].Add(timer);
                            timer.Start();
                        }
                        UpLauncher.timers[ac.get_value()].Add(ac.get_ctrl());
                        break;
                    default:
                        play_action(ac.get_ctrl(), time);
                        break;
                }
            }
        }

        public static void play_timer(object sender, EventArgs e)
        {
            if (sender is Timer)
            {
                Timer time = sender as Timer;
                foreach (object obj in UpLauncher.timers[time.Interval])
                {
                    if (obj is Control)
                        play_action(obj as Control, 7);
                }
            }
        }

        public static void play_release(object sender, EventArgs e)
        {
            if (sender is Control)
                play_action(sender as Control, 3);
        }

        public static void play_hover(object sender, EventArgs e)
        {
            if (sender is Control)
                play_action(sender as Control, 1);
        }

        public static void play_leave(object sender, EventArgs e)
        {
            if (sender is Control)
                play_action(sender as Control, 2);
        }

        public static void play_click(object sender, EventArgs e)
        {
            if(sender is Control)
                play_action(sender as Control, 0);
        }

        public static void play_action(Control ctrl, int time)
        {
            if (!listc.ContainsKey(ctrl))
                return;
            List<action> xmllist = listc[ctrl];
            if (config.MODE_DEBUG)
                Debug.WriteLine(Environment.NewLine + "Component ("+ctrl.Name+") play action : time " + time);
            foreach (action ac in xmllist)
            {
                if (ac.get_time() == time)
                    ac.launch_actions();
            }
        }

        public void launch_actions()
        {
            foreach (XmlElement elem in this.actions)
            {
                try
                {
                    if (elem.NodeType == XmlNodeType.Element)
                    {
                        if (config.MODE_DEBUG)
                            Debug.WriteLine(Environment.NewLine + "Component (" + ctrl.Name + ") action: " + elem.Name + "=" + elem.InnerText);
                        switch (elem.Name.ToUpper())
                        {
                            case "START":
                                Process.Start(elem.InnerText);
                                break;
                            case "WIN_STATE":
                                int state = Int32.Parse(elem.InnerText);
                                if (state < 3)
                                    UpLauncher.main.WindowState = (FormWindowState)Int32.Parse(elem.InnerText);
                                else
                                    UpLauncher.main.Close();
                                break;
                            case "LOCK":
                                string[] args = elem.InnerText.Split(',');
                                foreach (string arg in args)
                                {
                                    utils.set_enabled(UpLauncher.components[Int32.Parse(arg)].get_me(), false);
                                }
                                break;
                            case "UNLOCK":
                                string[] args1 = elem.InnerText.Split(',');
                                foreach (string arg in args1)
                                {
                                    utils.set_enabled(UpLauncher.components[Int32.Parse(arg)].get_me(), true);
                                }
                                break;
                            case "HIDDEN":
                                string[] args2 = elem.InnerText.Split(',');
                                foreach (string arg in args2)
                                {
                                    utils.set_visible(UpLauncher.components[Int32.Parse(arg)].get_me(), false);
                                }
                                break;
                            case "VISIBLE":
                                string[] args3 = elem.InnerText.Split(',');
                                foreach (string arg in args3)
                                {
                                    utils.set_visible(UpLauncher.components[Int32.Parse(arg)].get_me(), true);
                                }
                                break;
                        }
                    }
                }catch(Exception e)
                {
                    Debug.WriteLine(Environment.NewLine + e.Message);
                }
            }
            UpLauncher.design.load_design(this.ctrl, this.actions);
        }

        public int get_value()
        {
            return (this.value);
        }

        public XmlNodeList get_actions()
        {
            return (this.actions);
        }

        public Control get_ctrl()
        {
            return (this.ctrl);
        }

        public int get_time()
        {
            return (this.time);
        }
    }
}
