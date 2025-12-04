using mySQLite;
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
    public partial class Tasks : Form
    {
        private Form1 form1;
        private SQLiteQueries sqliteQ;

        public Tasks()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Настройка формы
            this.Text = "Задачи";
            this.Size = new Size(375, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void buttonSchelude_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSetting_Click(object sender, EventArgs e)
        {
            SettingsForm1 settings = new SettingsForm1(form1, sqliteQ);
            settings.ShowDialog();
            this.Close();
        }
    }
}
