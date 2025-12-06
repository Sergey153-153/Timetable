using mySQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class Tasks : Form
    {
        private List<TaskItem> _tasks;
        private SQLiteQueries _db;

        public Tasks(SQLiteQueries db)
        {
            InitializeComponent();
            _db = db;
            LoadTasksFromDatabase();
        }

        private void LoadTasksFromDatabase()
        {
            if (_db == null)
            {
                LoadTestData();
                return;
            }
            try
            {
                // Получаем только невыполненные задачи
                _tasks = _db.GetTasksByFilter(isCompleted: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки задач из БД: {ex.Message}");
                LoadTestData();
            }
            DisplayTasks();
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
                Label noTasksLabel = new Label
                {
                    Text = "Заданий нет",
                    Font = new Font("Arial", 12),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
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
                    using (var editForm = new Add_task(_db, task))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            detailsForm.Close();

                            LoadTasksFromDatabase(); 
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
                        _db.DeleteTask(task.Id);
                        detailsForm.Close();
                        LoadTasksFromDatabase();
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

        private void task_schelude_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void task_settings_Click(object sender, EventArgs e)
        {
            SettingsForm1 settingsForm = new SettingsForm1(_db);
            this.Hide();
            settingsForm.ShowDialog();
            this.Close();
        }

        private void task_add_Click(object sender, EventArgs e)
        {
            Add_task addTaskForm = new Add_task(_db);
            if (addTaskForm.ShowDialog() == DialogResult.OK)
            {
                LoadTasksFromDatabase();
                DisplayTasks();
            }
        }
    }
}
