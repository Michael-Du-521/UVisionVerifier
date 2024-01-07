using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UVisionVerifier
{
    public partial class FormUVisionVerifier : Form
    {
        public FormUVisionVerifier()
        {
            InitializeComponent();
        }

        private void buttonNG_Click(object sender, EventArgs e)
        {
           FormInfo formInfo= new FormInfo();
            formInfo.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            statusStrip1.Items[0].Text = DateTime.Now.ToString();
        }

        private void FormUVisionVerifier_Load(object sender, EventArgs e)
        {
            //show DateTime.Now
            timer1.Start();// timer is is for status bar updating dateTime

        }
    }
}
