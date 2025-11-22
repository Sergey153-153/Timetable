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

using System.Data.SQLite;
using DatabaseLib;
using mySQLite;

namespace SQLiteProject
{
    public partial class MoreLesson : Form
    {

        private Form1 form1;
        private int LessonId;

        public MoreLesson(Form1 parentForm)
        {
            InitializeComponent();
            form1 = parentForm;
        }

        public MoreLesson(Form1 parentForm, int lessonId)
        {
            InitializeComponent();
            form1 = parentForm;
            LessonId = lessonId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form1.Show();
            this.Close();
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            MoveForm mf = new MoveForm(LessonId);
            mf.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (DeleteForm form = new DeleteForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.SelectedOption == "Day")
                        ;
                        //DeleteForThisDay();
                    else if (form.SelectedOption == "Forever")
                        ;
                        //DeleteForever();
                }
            }
        }
    }
}
