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
        private List<TaskItem> _tasks;

        public DateTime SelectedDate { get; set; }

        public MoreLesson(Form1 parentForm, SQLiteQueries db, int lessonId)
        {
            InitializeComponent();

            form1 = parentForm;
            sqliteQ = db;
            LessonId = lessonId;

            LoadTasks();
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

        private void LoadTasks()
        {
            _tasks = sqliteQ.GetTasksForLesson(LessonId);
            DisplayTasks();
        }

        private void DisplayTasks()
        {
            content_block.Controls.Clear();
            
            if (_tasks == null || _tasks.Count == 0)
            {
                Label noTasksLabel = new Label();
                noTasksLabel.Text = "Заданий нет";
                noTasksLabel.Font = new Font("Arial", 14);
                noTasksLabel.AutoSize = false;
                noTasksLabel.Size = new Size(300, 50);
                noTasksLabel.TextAlign = ContentAlignment.MiddleCenter;
                noTasksLabel.Location = new Point(0, 0);

                content_block.Controls.Add(noTasksLabel);
                return;
            }

            var groupedTasks = _tasks
                .Where(t => !t.IsCompleted)
                .GroupBy(t => t.Deadline.Date)
                .OrderBy(g => g.Key);

            int currentY = 10;

            foreach (var group in groupedTasks)
            {
                Panel datePanel = CreateDatePanel(group.Key, currentY);
                content_block.Controls.Add(datePanel);
                currentY += datePanel.Height + 5;

                foreach (var task in group.OrderBy(t => t.Deadline))
                {
                    Panel taskPanel = CreateTaskPanel(task, currentY);
                    content_block.Controls.Add(taskPanel);
                    currentY += taskPanel.Height + 5;
                }

                currentY += 10;
            }
        }

        private Panel CreateDatePanel(DateTime date, int yPosition)
        {
            Panel panel = new Panel
            {
                Size = new Size(content_block.Width - 30, 40),
                Location = new Point(5, yPosition)
            };

            Label dateLabel = new Label
            {
                Text = $"{date:dd.MM.yyyy} - {date:dddd}",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            panel.Controls.Add(dateLabel);
            return panel;
        }

        private Panel CreateTaskPanel(TaskItem task, int yPosition)
        {
            Panel panel = new Panel
            {
                Size = new Size(content_block.Width - 30, 70),
                Location = new Point(5, yPosition),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Заголовок задачи
            Label titleLabel = new Label
            {
                Text = $"{task.Type}: {task.SubjectName}",
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            // Описание задачи
            Label descLabel = new Label
            {
                Text = task.Description,
                Font = new Font("Arial", 9),
                Location = new Point(10, 35),
                AutoSize = true
            };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(descLabel);

            // Файл
            if (!string.IsNullOrEmpty(task.FilePath))
            {
                Label fileIcon = new Label
                {
                    Text = "!!",
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Location = new Point(panel.Width - 30, 10),
                    AutoSize = true,
                    Cursor = Cursors.Hand,
                    Tag = task
                };
                fileIcon.Click += FileIcon_Click;
                panel.Controls.Add(fileIcon);
            }

            // Обработчик клика по задаче
            panel.Click += (s, e) => ShowTaskDetails(task);
            titleLabel.Click += (s, e) => ShowTaskDetails(task);
            descLabel.Click += (s, e) => ShowTaskDetails(task);

            return panel;
        }

        private void FileIcon_Click(object sender, EventArgs e)
        {
            if (sender is Label fileIcon && fileIcon.Tag is TaskItem task)
            {
                OpenFileForTask(task);
            }
        }

        private void ShowTaskDetails(TaskItem task)
        {
            using (var detailsForm = new Form())
            {
                detailsForm.Text = "Детали задания";
                detailsForm.Size = new Size(500, 400);
                detailsForm.StartPosition = FormStartPosition.CenterParent;
                detailsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                detailsForm.MaximizeBox = false;
                detailsForm.MinimizeBox = false;

                Panel mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), AutoScroll = true };

                Label typeLabel = new Label
                {
                    Text = $"{task.Type}: {task.SubjectName}",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    Location = new Point(0, 0),
                    AutoSize = true
                };
                mainPanel.Controls.Add(typeLabel);

                Label deadlineLabel = new Label
                {
                    Text = $"Срок сдачи: {task.Deadline:dd.MM.yyyy} ({task.Deadline:dddd})",
                    Font = new Font("Arial", 11),
                    Location = new Point(0, 40),
                    AutoSize = true
                };
                mainPanel.Controls.Add(deadlineLabel);

                Label descTitleLabel = new Label
                {
                    Text = "Описание:",
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    Location = new Point(0, 80),
                    AutoSize = true
                };
                mainPanel.Controls.Add(descTitleLabel);

                TextBox descriptionBox = new TextBox
                {
                    Text = task.Description,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Location = new Point(0, 105),
                    Size = new Size(440, 100),
                    ReadOnly = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = SystemColors.Window
                };
                mainPanel.Controls.Add(descriptionBox);

                int currentY = 220;

                if (!string.IsNullOrEmpty(task.FilePath))
                {
                    Label fileTitleLabel = new Label
                    {
                        Text = "Прикрепленный файл:",
                        Font = new Font("Arial", 11, FontStyle.Bold),
                        Location = new Point(0, currentY),
                        AutoSize = true
                    };
                    mainPanel.Controls.Add(fileTitleLabel);

                    Button downloadButton = new Button
                    {
                        Text = $" Скачать: {Path.GetFileName(task.FilePath)}",
                        Font = new Font("Arial", 10),
                        Location = new Point(0, currentY + 30),
                        Size = new Size(300, 35)
                    };
                    downloadButton.Click += (s, e) => DownloadFile(task.FilePath);
                    mainPanel.Controls.Add(downloadButton);

                    currentY += 80;
                }

                Button closeButton = new Button
                {
                    Text = "Закрыть",
                    Font = new Font("Arial", 10),
                    Location = new Point(0, currentY + 20),
                    Size = new Size(100, 30)
                };
                closeButton.Click += (s, e) => detailsForm.Close();
                mainPanel.Controls.Add(closeButton);

                detailsForm.Controls.Add(mainPanel);
                detailsForm.ShowDialog();
            }
        }

        private void OpenFileForTask(TaskItem task)
        {
            if (!string.IsNullOrEmpty(task.FilePath) && File.Exists(task.FilePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(task.FilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл не найден или путь к файлу не указан");
            }
        }

        private void DownloadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("Файл не найден");
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.FileName = Path.GetFileName(filePath);
                saveDialog.Filter = "Все файлы (*.*)|*.*";
                saveDialog.Title = "Сохранить файл";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(filePath, saveDialog.FileName, true);
                        MessageBox.Show($"Файл сохранен как: {saveDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
                    }
                }
            }
        }

        private void task_schelude_Click(object sender, EventArgs e)
        {
            this.Close();
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

                pickDateTime.StartPosition = FormStartPosition.CenterParent;
                pickDateTime.FormBorderStyle = FormBorderStyle.FixedDialog;
                pickDateTime.ControlBox = false;

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
                string dateStr = SelectedDate.ToString("yyyy-MM-dd");

                string line1 = $"{LessonId};{dateStr};0;;;;";
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

                deleteForm.StartPosition = FormStartPosition.CenterParent;
                deleteForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                deleteForm.ControlBox = false;

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
                    // Удаление только на выбранный день
                    string dateStr = SelectedDate.ToString("yyyy-MM-dd");

                    string line = $"{LessonId};{dateStr};0;;;;";
                    List<string> listOverrides = new List<string>() { line };
                    int err = sqliteQ.AddLessonOverrides(listOverrides);

                    if (err == 0)
                        MessageBox.Show("Пара удалена только на выбранный день!");
                    else
                        MessageBox.Show("Ошибка удаления пары на выбранный день.");
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

        private void btnAddHomework_Click(object sender, EventArgs e)
        {
            Add_task addTaskForm = new Add_task(sqliteQ);
            addTaskForm.ShowDialog();
            LoadTasks();
        }
    }
}
