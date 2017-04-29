using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpLauncher_Public.utils
{
   public class utils
    {
        public static void set_text(Control ctrl, string text)
        {
            try
            {
                if (ctrl.InvokeRequired)
                {
                    ctrl.BeginInvoke((MethodInvoker)delegate () { ctrl.Text = text; });
                }
                else
                {
                    ctrl.Text = text;
                }
            }catch (Exception e)
            { }
        }

        public static void set_location(Control ctrl, Point p)
        {
            try { 
            if (ctrl.InvokeRequired)
            {
                ctrl.BeginInvoke((MethodInvoker)delegate () { ctrl.Location = p; });
            }
            else
            {
                ctrl.Location = p;
            }
        }catch (Exception e)
            { }
}

        public static void set_width(Control ctrl, int w)
        {
            try { 
                if (ctrl.InvokeRequired)
                {
                    ctrl.BeginInvoke((MethodInvoker)delegate () { ctrl.Size = new Size(w, ctrl.Size.Height); });
                }
                else
                {
                    ctrl.Size = new Size(w, ctrl.Size.Height);
                }
            }
            catch (Exception e)
            { }
        }

        public static void set_enabled(Control ctrl, bool w)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.BeginInvoke((MethodInvoker)delegate () { ctrl.Enabled = w; });
            }
            else
            {
                ctrl.Enabled = w;
            }
        }
        public static void set_visible(Control ctrl, bool w)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.BeginInvoke((MethodInvoker)delegate () { ctrl.Visible = w; });
            }
            else
            {
                ctrl.Visible = w;
            }
        }
        public static void center_text(Control obj, int center)
        {
            set_location(obj, new Point(center / 2 - obj.Size.Width / 2, obj.Location.Y));
        }
    }
}
