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

        public MoreLesson(Form1 parentForm, SQLiteQueries db, int lessonId)
        {
            InitializeComponent();

            form1 = parentForm;
            sqliteQ = db;
            LessonId = lessonId;

            LoadLessonInfo();

            lblSubject.Left = (this.ClientSize.Width - lblSubject.Width) / 2;
            lblTime.Left = (this.ClientSize.Width - lblTime.Width) / 2;
            lblTeacher.Left = (this.ClientSize.Width - lblTeacher.Width) / 2;

        }

        private void LoadLessonInfo()
        {
            var lesson = sqliteQ.GetLessonById(LessonId);

            if (lesson == null)
            {
                MessageBox.Show("Ошибка: пара не найдена!");
                return;
            }

            lblSubject.Text = lesson.Subject;
            lblTeacher.Text = lesson.Teacher;

            string dayText = GetDayName(lesson.DayOfWeek);

            // 🔥 Если WeekNumber = 0 → не выводим неделю
            if (lesson.WeekNumber == 0)
                lblTime.Text = $"{dayText}: {lesson.Time}";
            else
                lblTime.Text = $"{lesson.WeekNumber} неделя, {dayText}: {lesson.Time}";
        }

        private string GetDayName(int day)
        {
            switch (day)
            {
                case 1: return "Понедельник";
                case 2: return "Вторник";
                case 3: return "Среда";
                case 4: return "Четверг";
                case 5: return "Пятница";
                case 6: return "Суббота";
                default: return "?";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form1.Location = this.Location;
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
            // Создаем форму для ввода кода
            using (Form inputForm = new Form())
            {
                inputForm.Width = 400;
                inputForm.Height = 150;
                inputForm.Text = "Замена расписания";
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;

                Label lbl = new Label() { Left = 10, Top = 10, Text = "Введите код расписания:", AutoSize = true };
                TextBox txt = new TextBox() { Left = 10, Top = 40, Width = 360 };
                Button btnOk = new Button() { Text = "OK", Left = 200, Width = 80, Top = 70, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Отмена", Left = 290, Width = 80, Top = 70, DialogResult = DialogResult.Cancel };

                inputForm.Controls.Add(lbl);
                inputForm.Controls.Add(txt);
                inputForm.Controls.Add(btnOk);
                inputForm.Controls.Add(btnCancel);

                inputForm.AcceptButton = btnOk;
                inputForm.CancelButton = btnCancel;

                // Показываем форму как модальный InputBox
                if (inputForm.ShowDialog() != DialogResult.OK)
                    return;

                string code = txt.Text;

                if (string.IsNullOrWhiteSpace(code))
                    return;

                var info = sqliteQ.getScheduleByCode(code);

                if (info == null)
                {
                    MessageBox.Show("Расписание с таким кодом не найдено!");
                    return;
                }

                int targetID = 1;

                if (sqliteQ.DeleteSchedule(targetID) == 0)
                {
                    MessageBox.Show("Ошибка удаления старого расписания!");
                    return;
                }

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
}
