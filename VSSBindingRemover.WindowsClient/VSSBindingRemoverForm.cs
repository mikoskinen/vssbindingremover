using System;
using System.Windows.Forms;
using VSSBindingRemover.Library;

namespace VSSBindingRemover.WindowsClient
{
    public partial class VSSBindingRemoverForm : Form
    {
        public VSSBindingRemoverForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK )
            {
                this.textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                VSSBindingRemover.Library.VSSBindingRemover vssRemover = new VSSBindingRemover.Library.VSSBindingRemover(this.textBox1.Text);
                vssRemover.RemoveBindings();

                MessageBox.Show("Processing completed without errors.", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
