using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N64GFXCookie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open CI8 File...";
            openFileDialog1.Filter = "Binary File (*.bin)|*.bin|All files|*.*";
            openFileDialog1.Tag = "ci8";
            openFileDialog1.ShowDialog();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Import PNG";
            openFileDialog1.Filter = "Portable Network Graphics (*.png)|*.png";
            openFileDialog1.Tag = "import";
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if ((string)openFileDialog1.Tag == "ci8")
            {
                //Load CI8
                Program.LoadCI8File(openFileDialog1.OpenFile());
                UpdateImage();
            }
            else
            {
                //Import PNG
                Program.ImportPNG(openFileDialog1.OpenFile());
                UpdateImage();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Export to PNG";
            saveFileDialog1.Filter = "Portable Network Graphics (*.png)|*.png";
            saveFileDialog1.Tag = "to";
            saveFileDialog1.ShowDialog();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save CI8 File...";
            saveFileDialog1.Filter = "Binary File (*.bin)|*.bin|All files|*.*";
            saveFileDialog1.Tag = "ci8";
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if ((string)saveFileDialog1.Tag == "to")
            {
                //Export to PNG
                Program.SavePNGRender(saveFileDialog1.OpenFile(), (int)numericUpDown1.Value, (int)numericUpDown2.Value);
            }
            else
            {
                //Save CI8 binary file
                Program.SaveCI8File(saveFileDialog1.OpenFile());
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Exit
            Application.Exit();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }


        //Other Functions
        private void UpdateImage()
        {
            pictureBox1.Image = Program.RenderGFX((int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value);
            pictureBox1.Width = pictureBox1.Image.Width;
            pictureBox1.Height = pictureBox1.Image.Height;
        }
    }
}
