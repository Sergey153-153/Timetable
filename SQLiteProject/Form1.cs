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
    public partial class Form1 : Form
    {
        SQLiteQueries sqliteQ;
        
        private const int COUNT_TABLES_IN_DB = 4; //кол-во таблиц в БД
        public List<string> listSchedules;
        public List<string> listLessons;

        public Form1()
        {
            InitializeComponent();
            panelLessons.AutoScroll = true;

            this.Activated += Form1_Activated;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (sqliteQ == null) return; // ничего не делаем, пока БД не подключена
            List<LessonInfo> todaysLessons = sqliteQ.GetLessonsForDate(DateTime.Today);
            ShowLessonsAsButtons(todaysLessons);
            label1Info();
        }

        public void ShowLessonsAsButtons(List<LessonInfo> lessons)
        {
            panelLessons.Controls.Clear(); // очищаем старые кнопки

            int y = 10; // начальная позиция
            int buttonHeight = 40;
            int buttonWidth = panelLessons.ClientSize.Width - 20; // немного отступа

            foreach (var lesson in lessons)
            {
                Button btn = new Button();
                btn.Text = $"{lesson.Time} - {lesson.Subject}";
                btn.Tag = lesson.LessonID; // сохраняем id урока
                btn.Width = buttonWidth;
                btn.Height = buttonHeight;
                btn.Left = 10;
                btn.Top = y;

                btn.Click += LessonButton_Click;

                panelLessons.Controls.Add(btn);
                y += buttonHeight + 10; // смещение для следующей кнопки
            }
        }

        private void LessonButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            int lessonId = (int)btn.Tag;

            // открываем форму MoreLesson
            MoreLesson f2 = new MoreLesson(this, sqliteQ, lessonId);
            f2.StartPosition = FormStartPosition.CenterParent;
            f2.StartPosition = FormStartPosition.Manual;
            f2.Location = this.Location;

            f2.Show();
            f2.BringToFront();
            f2.Activate();
            this.Hide();
        }

        /*public int GetDayNumber(string day)
        {
            switch (day)
            {
                case "Monday": return 1;
                case "Tuesday": return 2;
                case "Wednesday": return 3;
                case "Thursday": return 4;
                case "Friday": return 5;
                case "Saturday": return 6;
                case "Понедельник": return 1;
                case "Вторник": return 2;
                case "Среда": return 3;
                case "Четверг": return 4;
                case "Пятница": return 5;
                case "Суббота": return 6;
                default: return 0;
            }
        }*/

        private void connectToDB()
        {
            string dbName = "testDB.db";
            int flag = 0;
            if (!File.Exists(dbName))
            {
                DialogResult result = MessageBox.Show("Файл с базой данных не найден. Создать новую базу данных?",
                                                      "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                    Application.Exit();
                flag += 1;
                SQLiteConnection.CreateFile(dbName);
            }
            sqliteQ = new SQLiteQueries(dbName);

            if (sqliteQ._sqlt.GetTables().Count != COUNT_TABLES_IN_DB)
            {
                if (flag == 0)
                {
                    DialogResult result = MessageBox.Show("В файле БД отсутствуют необходимые таблицы. Пересоздать таблицы?",
                                          "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                        Application.Exit();
                }
                sqliteQ.CreateTables(dbName);

                //Если БД была новая, то заполним ее тестовыми данными
                saveDataToDB();
            }
        }

        private void saveDataToDB()
        {
            List<string> listSchedules = new List<string>()
            {
                "1;0000;основное;2",
                "2;1212;Двухнедельное расписание;2",
                "3;1111;Однонедельное расписание;1"
            };

            List<string> listLessons = new List<string>()
            {
                // основное 1
                "1;1;1;1;ТОК;Соловьев В.П.;27б;13:00;14:30",
                "1;2;1;1;История России;Пирогов Д.В.;ПСПШ;17:00;18:30",

                // Расписание 1
                "2;1;1;1;ТОК;Соловьев В.П.;27б;13:00;14:30",
                "2;2;1;1;История России;Пирогов Д.В.;ПСПШ;17:00;18:30",

                // Расписание 2
                "3;0;2;1;ТОК;Соловьев В.П.;27б;13:00;14:30",
                "3;0;2;2;ТОК;Соловьев В.П.;27б;14:40;16:10",
                "3;0;2;3;РЛП;Николаев К.Г.;27б;16:40;18:10",
                "3;0;2;4;Компьютерное моделирование;Соловьев А.В.;27б;18:20;20:35",
                "3;0;3;1;История России;Пирогов Д.В.;ПСПШ;17:00;18:30",
                "3;0;3;2;История России;Пирогов Д.В.;ПСПШ;18:35;20:05",
                "3;0;5;1;Применение IT в гуманитарной сфере;Глинников МВ;27б;10:30;12:00",
                "3;0;5;2;Применение IT в гуманитарной сфере;Глинников МВ;27б;12:10;13:35",
                "3;0;5;3;Экстримальные задачи;Пугачев О.В.;Т1;14:00;15:30",
                "3;0;5;4;Экстримальные задачи;Пугачев О.В.;Т1;15:40;17:10",
                "3;0;5;5;История России;Пирогов Д.В.;Т6;17:20;18:50",
                "3;0;6;1;ТРПО;Пуцко Н.Н.;115;09:00;10:30",
                "3;0;6;2;ТРПО;Пуцко Н.Н.;115;10:40;12:10",
                "3;0;6;3;АИС;Пуцко Н.Н.;115;12:30;14:00",
                "3;0;6;4;АИС;Пуцко Н.Н.;115;14:10;14:55"
            };

            ParametersCollection paramss = new ParametersCollection();

            int errSchedules = sqliteQ.AddSchedules(listSchedules);
            MessageBox.Show($"Обработано расписаний: {listSchedules.Count}, ошибок: {errSchedules}");

            int errLessons = sqliteQ.AddLessons(listLessons);
            MessageBox.Show($"Обработано уроков: {listLessons.Count}, ошибок: {errLessons}");
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            connectToDB();
            List<LessonInfo> todaysLessons = sqliteQ.GetLessonsForDate(DateTime.Today);
            ShowLessonsAsButtons(todaysLessons);
            label1Info();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsForm1 settings = new SettingsForm1(this, sqliteQ);
            settings.ShowDialog();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (sqliteQ == null)
                return;

            DateTime selectedDate = dateTimePicker1.Value.Date;
            List<LessonInfo> lessons = sqliteQ.GetLessonsForDate(selectedDate);
            ShowLessonsAsButtons(lessons);
            label1Info();
        }

        private void label1Info()
        {
            if (sqliteQ == null) return;
            DateTime selectedDate = dateTimePicker1.Value.Date;
            List<LessonInfo> lessons = sqliteQ.GetLessonsForDate(selectedDate);
            LessonInfo lesson = lessons.FirstOrDefault();
            if (lesson == null)
            {
                return;
            }
            else
            {
                if (lesson.WeekNumber == 1)
                    label1.Text = "Нечётная неделя";
                if (lesson.WeekNumber == 2)
                    label1.Text = "Чётная неделя";
            }
        }

        private void buttonTasks_Click(object sender, EventArgs e)
        {
            return;
        }

        private void buttonAddLesson_Click(object sender, EventArgs e)
        {
            return;
        }
    }
}
