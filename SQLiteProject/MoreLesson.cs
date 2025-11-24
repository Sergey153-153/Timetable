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

using Microsoft.VisualBasic;

namespace SQLiteProject
{
    public partial class MoreLesson : Form
    {

        private Form1 form1;
        private int LessonId;
        private SQLiteQueries sqliteQ;

        private int z;

        public MoreLesson(Form1 parentForm)
        {
            InitializeComponent();
            form1 = parentForm;
        }

        public MoreLesson(Form1 parentForm, SQLiteQueries db, int lessonId = -1)
        {
            InitializeComponent();
            form1 = parentForm;
            sqliteQ = db;
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
                        z = 1;
                        //DeleteForThisDay();
                    else if (form.SelectedOption == "Forever")
                        z = 2;
                        //DeleteForever();
                }
            }
        }

        private void btnReplaceSchedule_Click(object sender, EventArgs e)
        {
            string code = Interaction.InputBox("Введите код расписания:", "Замена расписания", "");

            if (string.IsNullOrWhiteSpace(code))
                return;

            var info = sqliteQ.getScheduleByCode(code);

            if (info == null)
            {
                MessageBox.Show("Расписание с таким кодом не найдено!");
                return;
            }

            int targetID = 1;

            // удаляем расписание с ID = 1
            if (sqliteQ.DeleteSchedule(targetID) == 0)
            {
                MessageBox.Show("Ошибка удаления старого расписания!");
                return;
            }

            // копируем в ID = 1
            int res = sqliteQ.CopySchedule(info.ScheduleID, targetID);

            if (res == 0)
            {
                MessageBox.Show("Ошибка копирования нового расписания!");
                return;
            }

            MessageBox.Show("Расписание успешно заменено!");
            form1.RefreshAllSchedulesData();
        }
    }
}
