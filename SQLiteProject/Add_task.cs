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
    public partial class Add_task : Form
    {
        public Add_task()
        {
            InitializeComponent();
        }

        private void back_tasks_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
