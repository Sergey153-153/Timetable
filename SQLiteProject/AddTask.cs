using mySQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class AddTask : Form
    {
        private SQLiteQueries sqliteQ;

        public AddTask()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Настройка формы
            this.Text = "Добавить задание";
            this.Size = new Size(400, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void buttonAddTask_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}