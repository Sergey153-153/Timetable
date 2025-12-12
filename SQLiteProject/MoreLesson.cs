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
                detailsForm.StartPosition = FormStartPosition.CenterScreen;
                detailsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                detailsForm.MaximizeBox = false;
                detailsForm.MinimizeBox = false;
                detailsForm.AutoSize = false; // мы сами считаем высоту
                detailsForm.Width = 375; // фиксированная ширина
                int maxHeight = 648; // максимум по высоте

                Panel mainPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(15),
                    AutoScroll = true
                };

                int y = 10;

                // ===== Тип и предмет =====
                Label typeLabel = new Label
                {
                    Text = $"{task.Type}: {task.SubjectName}",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    AutoSize = false,
                    Width = 330,
                    Location = new Point(5, y)
                };
                mainPanel.Controls.Add(typeLabel);
                y += 40;

                // ===== Дедлайн =====
                Label deadlineLabel = new Label
                {
                    Text = $"Срок сдачи: {task.Deadline:dd.MM.yyyy} ({task.Deadline:dddd})",
                    Font = new Font("Arial", 11),
                    AutoSize = false,
                    Width = 330,
                    Location = new Point(5, y)
                };
                mainPanel.Controls.Add(deadlineLabel);
                y += 35;

                // ===== Описание =====
                Label descTitleLabel = new Label
                {
                    Text = "Описание:",
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    AutoSize = false,
                    Width = 330,
                    Location = new Point(5, y)
                };
                mainPanel.Controls.Add(descTitleLabel);
                y += 25;

                TextBox descriptionBox = new TextBox
                {
                    Text = task.Description,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = SystemColors.Window,
                    Location = new Point(5, y),
                    Size = new Size(330, 150)
                };
                mainPanel.Controls.Add(descriptionBox);
                y += 160;

                // ===== Файл =====
                if (!string.IsNullOrEmpty(task.FilePath))
                {
                    Label fileLabel = new Label
                    {
                        Text = "Прикрепленный файл:",
                        Font = new Font("Arial", 11, FontStyle.Bold),
                        AutoSize = false,
                        Width = 330,
                        Location = new Point(5, y)
                    };
                    mainPanel.Controls.Add(fileLabel);
                    y += 25;

                    Button downloadButton = new Button
                    {
                        Text = $" Скачать: {Path.GetFileName(task.FilePath)}",
                        Font = new Font("Arial", 10),
                        Location = new Point(5, y),
                        Size = new Size(330, 35)
                    };
                    downloadButton.Click += (s, e) => DownloadFile(task.FilePath);
                    mainPanel.Controls.Add(downloadButton);
                    y += 45;
                }

                // ===== Кнопки =====
                Button editButton = new Button
                {
                    Text = "Редактировать",
                    Font = new Font("Arial", 10),
                    Location = new Point(5, y),
                    Size = new Size(110, 35)
                };
                editButton.Click += (s, e) =>
                {
                    using (var editForm = new Add_task(sqliteQ, task))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            detailsForm.Close();
                            LoadTasks();
                        }
                    }
                };
                mainPanel.Controls.Add(editButton);

                Button doneButton = new Button
                {
                    Text = "Выполнено",
                    Font = new Font("Arial", 10),
                    Location = new Point(125, y),
                    Size = new Size(110, 35)
                };
                doneButton.Click += (s, e) =>
                {
                    if (MessageBox.Show("Удалить задачу?", "Подтвердите", MessageBoxButtons.YesNo)
                        == DialogResult.Yes)
                    {
                        sqliteQ.DeleteTask(task.Id);
                        detailsForm.Close(); 
                        LoadTasks();
                    }
                };
                mainPanel.Controls.Add(doneButton);

                Button closeButton = new Button
                {
                    Text = "Закрыть",
                    Font = new Font("Arial", 10),
                    Location = new Point(245, y),
                    Size = new Size(90, 35)
                };
                closeButton.Click += (s, e) => detailsForm.Close();
                mainPanel.Controls.Add(closeButton);

                detailsForm.Controls.Add(mainPanel);

                // ===== Подбор высоты =====
                detailsForm.Load += (s, e) =>
                {
                    int contentHeight = 0;
                    foreach (Control c in mainPanel.Controls)
                        contentHeight = Math.Max(contentHeight, c.Bottom);

                    int formHeight = contentHeight + 50; // отступы и кнопки

                    // если меньше maxHeight — используем авторазмер, иначе включается скролл
                    detailsForm.Height = Math.Min(formHeight, maxHeight);

                    // центрирование на экране
                    detailsForm.StartPosition = FormStartPosition.CenterScreen;
                };

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

                CheckBox chkEnd = new CheckBox()
                {
                    Text = "Указать время конца",
                    Left = 10,
                    Top = 50,
                    Width = 150
                };

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

                // === ЛОГИКА АВТО-ОБНОВЛЕНИЯ КОНЦА ПАРЫ ===

                // При изменении галочки
                chkEnd.CheckedChanged += (s, ev) =>
                {
                    dtEnd.Enabled = chkEnd.Checked;

                    if (!chkEnd.Checked)
                    {
                        // Пересчитываем сразу
                        dtEnd.Value = dtStart.Value.AddMinutes(90);
                    }
                };

                // При изменении времени начала — автообновление конца
                dtStart.ValueChanged += (s, ev) =>
                {
                    if (!chkEnd.Checked)
                    {
                        dtEnd.Value = dtStart.Value.AddMinutes(90);
                    }
                };

                // Установим начальное значение конца
                dtEnd.Value = dtStart.Value.AddMinutes(90);

                // === ПОЛУЧЕНИЕ РЕЗУЛЬТАТА ===
                if (pickDateTime.ShowDialog() != DialogResult.OK)
                    return;

                DateTime startTime = dtStart.Value;
                DateTime endTime;

                if (!chkEnd.Checked)
                    endTime = startTime.AddMinutes(90);
                else
                {
                    endTime = new DateTime(
                        startTime.Year, startTime.Month, startTime.Day,
                        dtEnd.Value.Hour, dtEnd.Value.Minute, 0);
                }

                string overrideDate = startTime.ToString("yyyy-MM-dd");
                string newStartTime = startTime.ToString("HH:mm");
                string newEndTime = endTime.ToString("HH:mm");
                string newLocation = "";

                string line = $"{LessonId};{overrideDate};1;{newStartTime};{newEndTime};{newLocation}";
                List<string> listOverrides = new List<string>() { line };
                int err = sqliteQ.AddLessonOverrides(listOverrides);

                // Отмена пары в исходную дату
                string dateStr = SelectedDate.ToString("yyyy-MM-dd");
                string line1 = $"{LessonId};{dateStr};0;;;;";
                List<string> listOverrides1 = new List<string>() { line1 };
                int err1 = sqliteQ.AddLessonOverrides(listOverrides1);

                if (err == 0)
                { 
                    MessageBox.Show($"Пара успешно перенесена!\n{startTime:dd.MM.yyyy HH:mm} - {endTime:HH:mm}");
                    button1_Click(sender, e);
                }
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
                    { 
                        MessageBox.Show("Пара удалена только на выбранный день!");
                        button1_Click(sender, e);
                    }
                else
                    MessageBox.Show("Ошибка удаления пары на выбранный день.");
                }

                else if (deleteForm.DialogResult == DialogResult.OK)
                {
                    // Удаление навсегда
                    if (sqliteQ.DeleteLessonForever(LessonId) > 0)
                    {
                        MessageBox.Show("Пара удалена навсегда!");
                        button1_Click(sender, e);
                    }
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
