using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INI_Viewer
{
    public partial class Form2 : Form
    {
        public string output;
        public Form2(string initValue)
        {
            InitializeComponent();
            textBox1.Text = initValue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            output = textBox1.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
