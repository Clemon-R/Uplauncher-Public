using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UpLauncher_Public.data;

namespace UpLauncher_Public.world
{
    class component : design
    {
        private XmlNodeList list;
        private int id;
        private Control me;

        public component(main main, network client, UpLauncher upl, int id, XmlNodeList list, int type) : base (main, client, upl, false)
        {
            if (config.MODE_DEBUG)
            {
                string name = type == 0 ? "button" : "picture";
                name = type == 2 ? "label" : name;
                Debug.WriteLine(Environment.NewLine + "New "+ name + " (" + id + "):");
            }
            this.list = list;
            this.id = id;
            if (type == 0)
                this.me = new Button();
            else if (type == 1)
                this.me = new PictureBox();
            else
                this.me = new Label();
            me.Name = id + "";
            me.TabIndex = 0;
            load_design(me, list);
            if (this.me is Button)
                ((Button)me).UseVisualStyleBackColor = true;
            else if (this.me is Label)
            {
                ((Label)me).AutoSize = true;
                ((Label)me).BackColor = System.Drawing.Color.Transparent;
            }
            else if (this.me is PictureBox)
            {
                ((PictureBox)me).BackColor = System.Drawing.Color.Transparent;
                ((System.ComponentModel.ISupportInitialize)((PictureBox)me)).BeginInit();
            }
            main.Controls.Add(me);
        }

        public Control get_me()
        {
            return (this.me);
        }

        public int get_id()
        {
            return (this.id);
        }

        public void kill()
        {
            me.Dispose();
            main.Controls.Remove(me);
        }
    }
}
