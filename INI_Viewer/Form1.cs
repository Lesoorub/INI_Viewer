using System;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace INI_Viewer
{
    public partial class Form1 : Form
    {
        INI_file ini;
        public Form1()
        {
            InitializeComponent();
        }

        void LoadINIFile(string[] data)
        {
            treeView1.Nodes.Clear();
            ini = new INI_file(data);
            foreach (var h in ini.headers)
            {
                var node = new TreeNode(
                    h.raw, 
                    h.pairs
                    .Select(x => new TreeNode(x.raw)
                    {
                        Tag = x,
                        ForeColor = x.isCommented ? Color.Green : Color.Black
                    })
                    .ToArray())
                {
                    Tag = h
                };
                treeView1.Nodes.Add(node);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Program.filePath))
            {
                OpenFile();
            }
            else
            {
                LoadINIFile(File.ReadAllLines(Program.filePath));
            }
            Activate();
        }

        void OpenFile()
        {
            openFileDialog1.Filter = "Файлы конфигурации (*.ini)|*.ini|Все файлы (*.*)|*.*";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                LoadINIFile(File.ReadAllLines(Program.filePath = openFileDialog1.FileName));
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Parent == null)//if is header
            {
                using (Form2 editor = new Form2(e.Node.Text))
                {
                    if (editor.ShowDialog(this) == DialogResult.OK)
                    {
                        if (!Regex.IsMatch(editor.output, INI_file.Header.RegexPattern))
                        {
                            MessageBox.Show("Недопустимое имя заголовка");
                            return;
                        }
                        var header = (INI_file.Header)e.Node.Tag;
                        ini.raw[header.index] = editor.output;
                        e.Node.Text = editor.output;
                        File.WriteAllLines(Program.filePath, ini.raw);
                    }
                }
                return;
            }

            using (Form2 editor = new Form2(e.Node.Text))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    if (!Regex.IsMatch(editor.output, INI_file.Pair.RegexPattern))
                    {
                        MessageBox.Show("Недопустимое имя значения");
                        return;
                    }
                    var pair = (INI_file.Pair)e.Node.Tag;
                    ini.raw[pair.index] = editor.output;
                    e.Node.Text = editor.output;
                    e.Node.ForeColor = pair.isCommented ? Color.Green : Color.Black;
                    File.WriteAllLines(Program.filePath, ini.raw);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenWithDefaultProgram(Program.filePath);
        }
        public static void OpenWithDefaultProgram(string path)
        {
            using (Process fileopener = new Process())
            {
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + path + "\"";
                fileopener.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
    }
}
