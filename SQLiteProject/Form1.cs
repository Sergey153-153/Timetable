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
        private HolidayService holidayService;

        private const int COUNT_TABLES_IN_DB = 6; //кол-во таблиц в БД
        public List<string> listSchedules;
        public List<string> listLessons;

        public Form1()
        {
            InitializeComponent();
            panelLessons.AutoScroll = true;
            holidayService = new HolidayService();

            this.Activated += Form1_Activated;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (sqliteQ == null) return; // ничего не делаем, пока БД не подключена
            DateTime SelectedDate1 = dateTimePicker1.Value.Date;
            if (holidayService.IsHoliday(SelectedDate1))
            {
                ShowHolidayMessage(SelectedDate1);
                label1Info();
                scrollButtonsInfo(SelectedDate1);
                return;
            }
            List<LessonInfo> todaysLessons = sqliteQ.GetLessonsForDate(SelectedDate1);
            ShowLessonsAsButtons(todaysLessons);
            label1Info();
            scrollButtonsInfo(SelectedDate1);
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
            f2.SelectedDate = dateTimePicker1.Value.Date;
            f2.StartPosition = FormStartPosition.CenterParent;
            f2.StartPosition = FormStartPosition.Manual;
            f2.Location = this.Location;

            f2.Show();
            f2.BringToFront();
            f2.Activate();
            this.Hide();
        }

        public string DayOfWeekShortRu(string day)
        {
            switch (day)
            {
                case "Monday": return "Пн";
                case "Tuesday": return "Вт";
                case "Wednesday": return "Ср";
                case "Thursday": return "Чт";
                case "Friday": return "Пт";
                case "Saturday": return "Сб";
                case "Sunday": return "Вс";
                case "Понедельник": return "Пн";
                case "Вторник": return "Вт";
                case "Среда": return "Ср";
                case "Четверг": return "Чт";
                case "Пятница": return "Пт";
                case "Суббота": return "Сб";
                case "Воскресенье": return "Вс";
                default: return "";
            }
        }

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
                sqliteQ.CreateTasksTables(dbName);
                sqliteQ.AddDefaultSubjects();
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
            scrollButtonsInfo(DateTime.Today);
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
            if (holidayService.IsHoliday(selectedDate))
            {
                ShowHolidayMessage(selectedDate);
                label1Info();
                scrollButtonsInfo(selectedDate);
                return;
            }
            List<LessonInfo> lessons = sqliteQ.GetLessonsForDate(selectedDate);
            ShowLessonsAsButtons(lessons);
            label1Info();
            scrollButtonsInfo(selectedDate);
        }

        private void ShowHolidayMessage(DateTime date)
        {
            panelLessons.Controls.Clear(); // очищаем старые кнопки

            string holidayName = holidayService.GetHolidayName(date);

            // Создаем специальную кнопку/лейбл для сообщения о празднике
            Label holidayLabel = new Label();
            holidayLabel.Text = $"ЗАНЯТИЙ НЕТ\nСегодня праздник:\n{holidayName.ToUpper()}";
            holidayLabel.TextAlign = ContentAlignment.MiddleCenter;
            holidayLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            holidayLabel.ForeColor = Color.DarkRed;
            holidayLabel.BackColor = Color.LightYellow;
            holidayLabel.BorderStyle = BorderStyle.FixedSingle;

            // Настраиваем размер и позицию
            holidayLabel.Width = panelLessons.ClientSize.Width - 20;
            holidayLabel.Height = 120;
            holidayLabel.Left = 10;
            holidayLabel.Top = 50;

            // Делаем многострочный текст
            holidayLabel.AutoSize = false;

            panelLessons.Controls.Add(holidayLabel);

            // Можно добавить дополнительную информацию
            Label infoLabel = new Label();
            infoLabel.Text = "В этот день занятия не проводятся";
            infoLabel.Font = new Font("Arial", 10, FontStyle.Italic);
            infoLabel.ForeColor = Color.Gray;
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Width = panelLessons.ClientSize.Width - 20;
            infoLabel.Height = 30;
            infoLabel.Left = 10;
            infoLabel.Top = holidayLabel.Bottom + 10;

            panelLessons.Controls.Add(infoLabel);
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
            if (lesson != null && lesson.WeekNumber == 0)
            {
                label1.Text = " ";
            }
            else
            {
                if (lesson.WeekNumber == 1)
                    label1.Text = "Нечётная неделя";
                if (lesson.WeekNumber == 2)
                    label1.Text = "Чётная неделя";
            }
        }

        private void scrollButtonsInfo(DateTime date)
        {
            button2.BackColor = Color.LightGray;
            button3.BackColor = Color.LightGray;
            button4.BackColor = Color.LightGray;
            button5.BackColor = Color.LightGray;
            button6.BackColor = Color.LightGray;
            button7.BackColor = Color.LightGray;
            DateTime localMonday = date.AddDays((1-(int)date.DayOfWeek));
            button2.Text = DayOfWeekShortRu(localMonday.DayOfWeek.ToString()) + "," + localMonday.Day.ToString();
            button3.Text = DayOfWeekShortRu(localMonday.AddDays(1).DayOfWeek.ToString()) + "," + localMonday.AddDays(1).Day.ToString();
            button4.Text = DayOfWeekShortRu(localMonday.AddDays(2).DayOfWeek.ToString()) + "," + localMonday.AddDays(2).Day.ToString();
            button5.Text = DayOfWeekShortRu(localMonday.AddDays(3).DayOfWeek.ToString()) + "," + localMonday.AddDays(3).Day.ToString();
            button6.Text = DayOfWeekShortRu(localMonday.AddDays(4).DayOfWeek.ToString()) + "," + localMonday.AddDays(4).Day.ToString();
            button7.Text = DayOfWeekShortRu(localMonday.AddDays(5).DayOfWeek.ToString()) + "," + localMonday.AddDays(5).Day.ToString();
            if (sqliteQ.GetLessonsForDate(localMonday).FirstOrDefault() != null)
                button2.BackColor = Color.GreenYellow;
            if (sqliteQ.GetLessonsForDate(localMonday.AddDays(1)).FirstOrDefault() != null)
                button3.BackColor = Color.GreenYellow;
            if (sqliteQ.GetLessonsForDate(localMonday.AddDays(2)).FirstOrDefault() != null)
                button4.BackColor = Color.GreenYellow;
            if (sqliteQ.GetLessonsForDate(localMonday.AddDays(3)).FirstOrDefault() != null)
                button5.BackColor = Color.GreenYellow;
            if (sqliteQ.GetLessonsForDate(localMonday.AddDays(4)).FirstOrDefault() != null)
                button6.BackColor = Color.GreenYellow;
            if (sqliteQ.GetLessonsForDate(localMonday.AddDays(5)).FirstOrDefault() != null)
                button7.BackColor = Color.GreenYellow;
        }

        private void buttonTasks_Click(object sender, EventArgs e)
        {
            Tasks tasksForm = new Tasks(sqliteQ);
            tasksForm.ShowDialog();
        }

        private void buttonAddLesson_Click(object sender, EventArgs e)
        {
            using (Form addForm = BuildAddLessonForm(out Func<List<string>> collectData))
            {
                addForm.StartPosition = FormStartPosition.CenterParent;

                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    List<string> newLessons = collectData(); // получаем список строк

                    int err = sqliteQ.AddLessons(newLessons);
                    if (err == 0)
                        MessageBox.Show("Пара успешно добавлена!");
                    else
                        MessageBox.Show("Ошибка при добавлении пары!");

                }
            }
        }

        private Form BuildAddLessonForm(out Func<List<string>> collectData)
        {
            Form f = new Form();
            f.Text = "Создание новой пары";
            f.Size = new Size(375, 648);
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.MaximizeBox = false;

            // ====== ScheduleID не показываем, он всегда = 1 ======
            int scheduleId = 1;

            // ====== Неделя ======
            Label lblWeek = new Label() { Text = "Неделя (0/1/2):", Location = new Point(20, 20) };
            ComboBox cbWeek = new ComboBox() { Location = new Point(150, 18), Width = 200 };
            cbWeek.Items.AddRange(new[] { "0", "1", "2" });
            cbWeek.SelectedIndex = 0;

            // ====== День недели ======
            Label lblDay = new Label() { Text = "День недели:", Location = new Point(20, 60) };
            ComboBox cbDay = new ComboBox() { Location = new Point(150, 58), Width = 200 };
            cbDay.Items.AddRange(new[]{"Пн","Вт","Ср","Чт","Пт","Сб"});
            cbDay.SelectedIndex = 0;

            // ====== Номер пары ======
            Label lblLesson = new Label() { Text = "Номер пары:", Location = new Point(20, 100) };
            ComboBox cbLesson = new ComboBox() { Location = new Point(150, 98), Width = 200 };
            cbLesson.Items.AddRange(new[] { "1", "2", "3", "4", "5", "6", "7" });
            cbLesson.SelectedIndex = 0;

            // ====== Предмет ======
            Label lblSubject = new Label() { Text = "Предмет:", Location = new Point(20, 140) };
            TextBox txtSubject = new TextBox() { Location = new Point(150, 138), Width = 200 };

            // ====== Преподаватель ======
            Label lblTeacher = new Label() { Text = "Преподаватель:", Location = new Point(20, 180) };
            TextBox txtTeacher = new TextBox() { Location = new Point(150, 178), Width = 200 };

            // ====== Аудитория ======
            Label lblLocation = new Label() { Text = "Аудитория:", Location = new Point(20, 220) };
            TextBox txtLocation = new TextBox() { Location = new Point(150, 218), Width = 200 };

            // ====== Начало ======
            Label lblStart = new Label() { Text = "Начало (HH:MM):", Location = new Point(20, 260) };
            TextBox txtStart = new TextBox() { Location = new Point(150, 258), Width = 200, Text = "08:30" };

            // ====== Конец ======
            Label lblEnd = new Label() { Text = "Конец (HH:MM):", Location = new Point(20, 300) };
            TextBox txtEnd = new TextBox() { Location = new Point(150, 298), Width = 200, Text = "10:05" };

            // ====== Кнопки OK / Cancel ======
            Button btnOk = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(80, 330),
                Width = 100
            };

            Button btnCancel = new Button()
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new Point(200, 330),
                Width = 100
            };

            f.Controls.AddRange(new Control[]
                    {
                lblWeek, cbWeek,
                lblDay, cbDay,
                lblLesson, cbLesson,
                lblSubject, txtSubject,
                lblTeacher, txtTeacher,
                lblLocation, txtLocation,
                lblStart, txtStart,
                lblEnd, txtEnd,
                btnOk, btnCancel
            });

            // ========= ЛОГИКА СОХРАНЕНИЯ ===================================

            collectData = () =>
            {
                List<string> list = new List<string>();

                string line =
                    $"{scheduleId};" +
                    $"{cbWeek.SelectedItem};" +
                    $"{cbDay.SelectedIndex + 1};" +
                    $"{cbLesson.SelectedItem};" +
                    $"{txtSubject.Text};" +
                    $"{txtTeacher.Text};" +
                    $"{txtLocation.Text};" +
                    $"{txtStart.Text};" +
                    $"{txtEnd.Text}";

                list.Add(line);

                return list;
            };

            return f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dateTimePicker1.Value.AddDays(-7);
            dateTimePicker1_ValueChanged(sender, e);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dateTimePicker1.Value.AddDays(7);
            dateTimePicker1_ValueChanged(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime localMonday = dateTimePicker1.Value.AddDays((1 - (int)dateTimePicker1.Value.DayOfWeek));
            dateTimePicker1.Value = localMonday;
            dateTimePicker1_ValueChanged(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime localTuesday = dateTimePicker1.Value.AddDays((2 - (int)dateTimePicker1.Value.DayOfWeek));
            dateTimePicker1.Value = localTuesday;
            dateTimePicker1_ValueChanged(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime localWednesday = dateTimePicker1.Value.AddDays((3 - (int)dateTimePicker1.Value.DayOfWeek));
            dateTimePicker1.Value = localWednesday;
            dateTimePicker1_ValueChanged(sender, e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DateTime localTuesday = dateTimePicker1.Value.AddDays((4 - (int)dateTimePicker1.Value.DayOfWeek));
            dateTimePicker1.Value = localTuesday;
            dateTimePicker1_ValueChanged(sender, e);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DateTime localFriday = dateTimePicker1.Value.AddDays((5 - (int)dateTimePicker1.Value.DayOfWeek));
            dateTimePicker1.Value = localFriday;
            dateTimePicker1_ValueChanged(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DateTime localSaturday = dateTimePicker1.Value.AddDays((6 - (int)dateTimePicker1.Value.DayOfWeek));
            dateTimePicker1.Value = localSaturday;
            dateTimePicker1_ValueChanged(sender, e);
        }
    }
}
