using DatabaseLib;
using Microsoft.VisualBasic;
using mySQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class Form1 : Form
    {
        SQLiteQueries sqliteQ;

        private const int COUNT_TABLES_IN_DB = 7; //кол-во таблиц в БД
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
            DateTime SelectedDate1 = dateTimePicker1.Value.Date;
            if (sqliteQ.IsHoliday(SelectedDate1))
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

            List<string> listHolidays = new List<string>()
            {
                "Новый год;01.01;1;Ежегодный государственный праздник",
                "Рождество Христово;07.01;1;Ежегодный праздник",
                "День защитника Отечества;23.02;1;Ежегодный праздник",
                "Международный женский день;08.03;1;Ежегодный праздник",
                "Праздник Весны и Труда;01.05;1;Ежегодный праздник",
                "День Победы;09.05;1;Ежегодный праздник",
                "День России;12.06;1;Ежегодный праздник",
                "День народного единства;04.11;1;Ежегодный праздник"
            };

            ParametersCollection paramss = new ParametersCollection();

            int errSchedules = sqliteQ.AddSchedules(listSchedules);
            MessageBox.Show($"Обработано расписаний: {listSchedules.Count}, ошибок: {errSchedules}");

            int errLessons = sqliteQ.AddLessons(listLessons);
            MessageBox.Show($"Обработано уроков: {listLessons.Count}, ошибок: {errLessons}");

            int errHolidays = sqliteQ.AddHolidays(listHolidays);
            MessageBox.Show($"Обработано праздников: {listHolidays.Count}, ошибок: {errHolidays}");
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
            if (sqliteQ.IsHoliday(selectedDate))
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
            panelLessons.Controls.Clear(); // очищаем старые элементы

            Holiday holiday = sqliteQ.GetHolidayForDate(date);
            if (holiday == null)
                return; // на всякий случай

            // Текст заголовка
            string title = $"ЗАНЯТИЙ НЕТ\nСегодня праздник:\n{holiday.Name.ToUpper()}";

            // Основной блок сообщения
            Label holidayLabel = new Label
            {
                Text = title,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle,
                Width = panelLessons.ClientSize.Width - 20,
                Height = 130,
                Left = 10,
                Top = 40,
                AutoSize = false
            };

            panelLessons.Controls.Add(holidayLabel);


            // Дата праздника — красиво форматируем
            string dateText = holiday.IsAnnual
                ? $"Дата ежегодного праздника: {holiday.DateValue}"
                : $"Дата события: {holiday.DateValue.Replace('.', '-')}";


            // Описание (если есть)
            Label infoLabel = new Label
            {
                Text = dateText +
                      (string.IsNullOrWhiteSpace(holiday.Description)
                        ? ""
                        : $"\n\nОписание:\n{holiday.Description}"),
                Font = new Font("Arial", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = panelLessons.ClientSize.Width - 20,
                Height = 100,
                Left = 10,
                Top = holidayLabel.Bottom + 10,
                AutoSize = false
            };

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
            DateTime selectedDate1 = dateTimePicker1.Value.Date;
            using (Form addForm = BuildAddLessonForm(out Func<List<string>> collectData, selectedDate1))
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

        private Form BuildAddLessonForm(out Func<List<string>> collectData, DateTime selectedDate)
        {
            Form f = new Form();
            f.Text = "Создание новой пары";
            f.AutoSize = true;
            f.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.MaximizeBox = false;
            f.StartPosition = FormStartPosition.CenterParent;

            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(15),
                AutoSize = true
            };
            f.Controls.Add(panel);

            int scheduleId = 1;

            // ====== Неделя ======
            panel.Controls.Add(new Label { Text = "Неделя (0/1/2):" });
            ComboBox cbWeek = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cbWeek.Items.AddRange(new[] { "0", "1", "2" });
            cbWeek.SelectedIndex = 0;
            panel.Controls.Add(cbWeek);

            // ====== День недели ======
            panel.Controls.Add(new Label { Text = "День недели:" });
            ComboBox cbDay = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            string[] days = new[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб" };
            cbDay.Items.AddRange(days);

            int dayIndex = 0; // По умолчанию Пн

            switch (selectedDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    dayIndex = 0;
                    break;
                case DayOfWeek.Tuesday:
                    dayIndex = 1;
                    break;
                case DayOfWeek.Wednesday:
                    dayIndex = 2;
                    break;
                case DayOfWeek.Thursday:
                    dayIndex = 3;
                    break;
                case DayOfWeek.Friday:
                    dayIndex = 4;
                    break;
                case DayOfWeek.Saturday:
                    dayIndex = 5;
                    break;

                case DayOfWeek.Sunday:
                    dayIndex = 0; // Если воскресенье — ставим Пн
                    break;
            }
            cbDay.SelectedIndex = dayIndex;

            panel.Controls.Add(cbDay);

            // ====== Предмет ======
            panel.Controls.Add(new Label { Text = "Предмет:" });
            ComboBox cbSubject = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            var subjects = sqliteQ.GetAllSubjects();
            cbSubject.Items.AddRange(subjects.Select(s => s.Name).ToArray());
            cbSubject.Items.Add("Добавить новый...");
            cbSubject.SelectedIndex = 0;
            panel.Controls.Add(cbSubject);

            cbSubject.SelectedIndexChanged += (s, e) =>
            {
                if (cbSubject.SelectedItem.ToString() == "Добавить новый...")
                {
                    string newSubject = Microsoft.VisualBasic.Interaction.InputBox("Введите название нового предмета:", "Новый предмет");
                    if (!string.IsNullOrWhiteSpace(newSubject))
                    {
                        if (sqliteQ.AddSubject(newSubject))
                        {
                            cbSubject.Items.Insert(cbSubject.Items.Count - 1, newSubject);
                            cbSubject.SelectedItem = newSubject;
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при добавлении предмета.");
                            cbSubject.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        cbSubject.SelectedIndex = 0;
                    }
                }
            };

            // ====== Преподаватель ======
            panel.Controls.Add(new Label { Text = "Преподаватель:" });
            TextBox txtTeacher = new TextBox { Width = 200 };
            panel.Controls.Add(txtTeacher);

            // ====== Аудитория ======
            panel.Controls.Add(new Label { Text = "Аудитория:" });
            TextBox txtLocation = new TextBox { Width = 200 };
            panel.Controls.Add(txtLocation);

            // ====== Время начала и конца ======
            panel.Controls.Add(new Label { Text = "Начало:" });
            DateTimePicker dtStart = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown = true,
                Value = DateTime.Today.AddHours(8).AddMinutes(30),
                Width = 200
            };
            panel.Controls.Add(dtStart);

            panel.Controls.Add(new Label { Text = "Конец:" });
            DateTimePicker dtEnd = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                ShowUpDown = true,
                Value = dtStart.Value.AddMinutes(90),
                Width = 200
            };
            panel.Controls.Add(dtEnd);

            bool manualEndChange = false;

            dtStart.ValueChanged += (s, e) =>
            {
                if (!manualEndChange)
                    dtEnd.Value = dtStart.Value.AddMinutes(90);
            };

            dtEnd.ValueChanged += (s, e) => manualEndChange = true;

            // ====== Кнопки ======
            FlowLayoutPanel btnPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            Button btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 100 };
            Button btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 100 };
            btnPanel.Controls.Add(btnOk);
            btnPanel.Controls.Add(btnCancel);
            panel.Controls.Add(btnPanel);

            // ========= Сбор данных ===================================
            collectData = () =>
            {
                string subjectName = cbSubject.SelectedItem.ToString();
                int week = int.Parse(cbWeek.SelectedItem.ToString());
                int day = cbDay.SelectedIndex + 1;
                string teacher = txtTeacher.Text;
                string location = txtLocation.Text;
                string startTime = dtStart.Value.ToString("HH:mm");
                string endTime = dtEnd.Value.ToString("HH:mm");

                return new List<string>
                {
                    $"{scheduleId};{week};{day};1;{subjectName};{teacher};{location};{startTime};{endTime}"
                };
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
