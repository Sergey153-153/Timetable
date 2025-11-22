using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class DeleteForm : Form
    {
        public string SelectedOption { get; private set; } = "Cancel";

        public DeleteForm()
        {
            InitializeComponent();
        }

        private void btnDeleteDay_Click(object sender, EventArgs e)
        {
            SelectedOption = "Day";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDeleteForever_Click(object sender, EventArgs e)
        {
            SelectedOption = "Forever";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedOption = "Cancel";
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
