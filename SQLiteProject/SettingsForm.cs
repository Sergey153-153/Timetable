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
    public partial class Form2 : Form
    {
        private Form1 mainForm;
        private SQLiteQueries sqliteQ;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(Form1 parentForm, SQLiteQueries db)
        {
            InitializeComponent();
            mainForm = parentForm;
            sqliteQ = db;
            this.StartPosition = FormStartPosition.CenterParent;
        }
    }
}