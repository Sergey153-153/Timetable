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
    public partial class Tasks : Form
    {
        private List<TaskItem> _tasks;
        private SQLiteConnection _connection;

        public Tasks()
        {
            InitializeComponent();
            InitializeCustomComponents();
            SetConnection(_connection);
        }

        public void SetConnection(SQLiteConnection connection)
        {
            _connection = connection;
            LoadTasksFromDatabase();
            DisplayTasks();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Задачи";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(380, 650);
        }

        private void LoadTasksFromDatabase()
        {
            if (_connection == null)
            {
                LoadTestData();
                return;
            }

            try
            {
                _tasks = new List<TaskItem>();
                string sql = @"SELECT t.*, s.Name as SubjectName 
                              FROM Tasks t 
                              LEFT JOIN Subjects s ON t.SubjectId = s.Id 
                              WHERE t.IsCompleted = 0 
                              ORDER BY t.Deadline ASC";

                using (var command = new SQLiteCommand(sql, _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _tasks.Add(new TaskItem
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            Deadline = Convert.ToDateTime(reader["Deadline"]),
                            Type = reader["Type"].ToString(),
                            SubjectName = reader["SubjectName"].ToString(),
                            FilePath = reader["FilePath"]?.ToString() ?? ""
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки задач из БД: {ex.Message}");
                LoadTestData();
            }
        }

        private void LoadTestData()
        {
            _tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Математика - упражнения", Description = "Решить задачи №1-10", Deadline = DateTime.Now.AddDays(1), Type = "ДЗ", SubjectName = "Математика" },
                new TaskItem { Id = 2, Title = "Физика - лабораторная", Description = "Оформить отчет", Deadline = DateTime.Now.AddDays(1), Type = "КР", SubjectName = "Физика" }
            };
        }

        private void DisplayTasks()
        {
            content_block.Controls.Clear();

            if (_tasks == null || _tasks.Count == 0)
            {
                Label noTasksLabel = new Label();
                noTasksLabel.Text = "Заданий нет";
                noTasksLabel.Font = new Font("Arial", 12);
                noTasksLabel.TextAlign = ContentAlignment.MiddleCenter;
                noTasksLabel.Dock = DockStyle.Fill;
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
            Panel panel = new Panel();
            panel.Size = new Size(content_block.Width - 30, 40);
            panel.Location = new Point(5, yPosition);

            Label dateLabel = new Label();
            dateLabel.Text = $"{date:dd.MM.yyyy} - {date:dddd}";
            dateLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            dateLabel.Dock = DockStyle.Fill;
            dateLabel.TextAlign = ContentAlignment.MiddleLeft;
            dateLabel.Padding = new Padding(10, 0, 0, 0);

            panel.Controls.Add(dateLabel);
            return panel;
        }

        private Panel CreateTaskPanel(TaskItem task, int yPosition)
        {
            Panel panel = new Panel();
            panel.Size = new Size(content_block.Width - 30, 70);
            panel.Location = new Point(5, yPosition);
            panel.BorderStyle = BorderStyle.FixedSingle;

            // Заголовок задачи
            Label titleLabel = new Label();
            titleLabel.Text = $"{task.Type}: {task.SubjectName}";
            titleLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            titleLabel.Location = new Point(10, 10);
            titleLabel.AutoSize = true;

            // Описание задачи
            Label descLabel = new Label();
            descLabel.Text = task.Description;
            descLabel.Font = new Font("Arial", 9);
            descLabel.Location = new Point(10, 35);
            descLabel.AutoSize = true;

            // флаг если есть прикрепленный файл
            if (!string.IsNullOrEmpty(task.FilePath))
            {
                Label fileIcon = new Label();
                fileIcon.Text = "!!";
                fileIcon.Font = new Font("Arial", 10, FontStyle.Bold);
                fileIcon.Location = new Point(panel.Width - 30, 10);
                fileIcon.AutoSize = true;
                fileIcon.Cursor = Cursors.Hand;
                fileIcon.Tag = task;
                fileIcon.Click += FileIcon_Click;
                panel.Controls.Add(fileIcon);
            }

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(descLabel);

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

                // Основная панель
                Panel mainPanel = new Panel();
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.Padding = new Padding(20);
                mainPanel.AutoScroll = true;

                // Тип и предмет
                Label typeLabel = new Label();
                typeLabel.Text = $"{task.Type}: {task.SubjectName}";
                typeLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                typeLabel.Location = new Point(0, 0);
                typeLabel.AutoSize = true;
                mainPanel.Controls.Add(typeLabel);

                // Дедлайн
                Label deadlineLabel = new Label();
                deadlineLabel.Text = $"Срок сдачи: {task.Deadline:dd.MM.yyyy} ({task.Deadline:dddd})";
                deadlineLabel.Font = new Font("Arial", 11);
                deadlineLabel.Location = new Point(0, 40);
                deadlineLabel.AutoSize = true;
                mainPanel.Controls.Add(deadlineLabel);

                // Описание
                Label descTitleLabel = new Label();
                descTitleLabel.Text = "Описание:";
                descTitleLabel.Font = new Font("Arial", 11, FontStyle.Bold);
                descTitleLabel.Location = new Point(0, 80);
                descTitleLabel.AutoSize = true;
                mainPanel.Controls.Add(descTitleLabel);

                TextBox descriptionBox = new TextBox();
                descriptionBox.Text = task.Description;
                descriptionBox.Multiline = true;
                descriptionBox.ScrollBars = ScrollBars.Vertical;
                descriptionBox.Location = new Point(0, 105);
                descriptionBox.Size = new Size(440, 100);
                descriptionBox.ReadOnly = true;
                descriptionBox.BorderStyle = BorderStyle.FixedSingle;
                descriptionBox.BackColor = SystemColors.Window;
                mainPanel.Controls.Add(descriptionBox);

                // Файл
                int currentY = 220;

                if (!string.IsNullOrEmpty(task.FilePath))
                {
                    Label fileTitleLabel = new Label();
                    fileTitleLabel.Text = "Прикрепленный файл:";
                    fileTitleLabel.Font = new Font("Arial", 11, FontStyle.Bold);
                    fileTitleLabel.Location = new Point(0, currentY);
                    fileTitleLabel.AutoSize = true;
                    mainPanel.Controls.Add(fileTitleLabel);

                    Button downloadButton = new Button();
                    downloadButton.Text = $" Скачать: {Path.GetFileName(task.FilePath)}";
                    downloadButton.Font = new Font("Arial", 10);
                    downloadButton.Location = new Point(0, currentY + 30);
                    downloadButton.Size = new Size(300, 35);
                    downloadButton.Click += (s, e) => DownloadFile(task.FilePath);
                    mainPanel.Controls.Add(downloadButton);

                    currentY += 80;
                }

                // Кнопка закрытия
                Button closeButton = new Button();
                closeButton.Text = "Закрыть";
                closeButton.Font = new Font("Arial", 10);
                closeButton.Location = new Point(0, currentY + 20);
                closeButton.Size = new Size(100, 30);
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

        private void task_settings_Click(object sender, EventArgs e)
        {
            SettingsForm1 settingsForm = new SettingsForm1();
            this.Hide();
            settingsForm.ShowDialog();
            this.Close();
        }

        private void task_add_Click(object sender, EventArgs e)
        {
            Add_task addTaskForm = new Add_task();
            addTaskForm.SetConnection(_connection);
            if (addTaskForm.ShowDialog() == DialogResult.OK)
            {
                LoadTasksFromDatabase();
                DisplayTasks();
            }
        }
    }
}