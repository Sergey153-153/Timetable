using DatabaseLib;
using Microsoft.VisualBasic;
using mySQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class MoreLesson : Form
    {

        private Form1 form1;
        private int LessonId;
        private SQLiteQueries sqliteQ;

        //private int z;
        //Дима
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

            //Если WeekNumber = 0 → не выводим неделю
            if (lesson.WeekNumber == 0)
                lblTime.Text = $"{dayText}: {lesson.Time}";
            else
                lblTime.Text = $"{lesson.WeekNumber} неделя, {dayText}: {lesson.Time}";
        }

        public string GetDayName(int day)
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
            using (Form pickDateTime = new Form())
            {
                pickDateTime.Width = 350;
                pickDateTime.Height = 200;
                pickDateTime.Text = "Выберите дату и время пары";

                // Метки и контролы
                Label lblStart = new Label() { Text = "Начало:", Left = 10, Top = 10, Width = 80 };
                DateTimePicker dtStart = new DateTimePicker()
                {
                    Left = 100,
                    Top = 10,
                    Width = 220,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "dd.MM.yyyy HH:mm"
                };

                CheckBox chkEnd = new CheckBox() { Text = "Указать время конца", Left = 10, Top = 50, Width = 150 };
                DateTimePicker dtEnd = new DateTimePicker()
                {
                    Left = 160,
                    Top = 50,
                    Width = 160,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "HH:mm",
                    ShowUpDown = true,
                    Enabled = false
                };

                Button btnOk = new Button() { Text = "OK", Left = 100, Width = 80, Top = 100, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Отмена", Left = 190, Width = 80, Top = 100, DialogResult = DialogResult.Cancel };

                pickDateTime.Controls.Add(lblStart);
                pickDateTime.Controls.Add(dtStart);
                pickDateTime.Controls.Add(chkEnd);
                pickDateTime.Controls.Add(dtEnd);
                pickDateTime.Controls.Add(btnOk);
                pickDateTime.Controls.Add(btnCancel);

                pickDateTime.AcceptButton = btnOk;
                pickDateTime.CancelButton = btnCancel;

                // Обработчик галочки
                chkEnd.CheckedChanged += (s, ev) => dtEnd.Enabled = chkEnd.Checked;

                if (pickDateTime.ShowDialog() != DialogResult.OK)
                    return;

                DateTime startTime = dtStart.Value;
                DateTime endTime;

                if (!chkEnd.Checked)
                    endTime = startTime.AddMinutes(90);
                else
                    endTime = new DateTime(startTime.Year, startTime.Month, startTime.Day,
                                           dtEnd.Value.Hour, dtEnd.Value.Minute, 0);

                string overrideDate = startTime.ToString("yyyy-MM-dd");
                string newStartTime = startTime.ToString("HH:mm");
                string newEndTime = endTime.ToString("HH:mm");
                string newLocation = ""; // пусто, но не null

                string line = $"{LessonId};{overrideDate};1;{newStartTime};{newEndTime};{newLocation}";

                List<string> listOverrides = new List<string>() { line };
                int err = sqliteQ.AddLessonOverrides(listOverrides);

                // Отмена пары
                string line1 = $"{LessonId};{DateTime.Today:yyyy-MM-dd};0;;;;";
                List<string> listOverrides1 = new List<string>() { line1 };
                int err1 = sqliteQ.AddLessonOverrides(listOverrides1);

                if (err == 0)
                    MessageBox.Show($"Пара успешно перенесена!\n{startTime:dd.MM.yyyy HH:mm} - {endTime:HH:mm}");
                else
                    MessageBox.Show("Ошибка при переносе пары.");

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (Form deleteForm = new Form())
            {
                deleteForm.Width = 300;
                deleteForm.Height = 200;
                deleteForm.Text = "Удаление пары";

                Button btnDay = new Button() { Text = "Удалить только в этот день", Left = 20, Top = 20, Width = 250, DialogResult = DialogResult.Yes };
                Button btnForever = new Button() { Text = "Удалить навсегда", Left = 20, Top = 60, Width = 250, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Отмена", Left = 20, Top = 100, Width = 250, DialogResult = DialogResult.Cancel };

                deleteForm.Controls.Add(btnDay);
                deleteForm.Controls.Add(btnForever);
                deleteForm.Controls.Add(btnCancel);

                if (deleteForm.ShowDialog() == DialogResult.Cancel)
                    return;

                if (deleteForm.DialogResult == DialogResult.Yes)
                {
                    // Удаление только на сегодня
                    string line = $"{LessonId};{DateTime.Today:yyyy-MM-dd};0;;;;";
                    List<string> listOverrides = new List<string>() { line };
                    int err = sqliteQ.AddLessonOverrides(listOverrides);

                    if (err == 0)
                        MessageBox.Show("Пара удалена только на сегодня!");
                    else
                        MessageBox.Show("Ошибка удаления пары на сегодня.");
                }
                else if (deleteForm.DialogResult == DialogResult.OK)
                {
                    // Удаление навсегда
                    if (sqliteQ.DeleteLessonForever(LessonId) > 0)
                        MessageBox.Show("Пара удалена навсегда!");
                    else
                        MessageBox.Show("Ошибка удаления пары навсегда.");
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
            }
        }
    }
}
