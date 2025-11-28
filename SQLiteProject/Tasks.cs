using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class Tasks : Form
    {
        private List<TaskItem> _tasks;
        private Panel _contentPanel;

        public Tasks()
        {
            InitializeComponent();
            InitializeTasks(); // Загружаем тестовые данные
            InitializeCustomComponents();
            DisplayTasks();
        }
        private void InitializeTasks()
        {
            // Тестовые данные 
            _tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Математика - упражнения", Description = "Решить задачи №1-10", Deadline = DateTime.Now.AddDays(1), Type = "ДЗ" },
                new TaskItem { Id = 2, Title = "Физика - лабораторная", Description = "Оформить отчет", Deadline = DateTime.Now.AddDays(1), Type = "КР" },
                new TaskItem { Id = 3, Title = "История - эссе", Description = "Написать эссе о ВОВ", Deadline = DateTime.Now.AddDays(3), Type = "ДЗ" },
                new TaskItem { Id = 4, Title = "Программирование - проект", Description = "Завершить модуль 4", Deadline = DateTime.Now.AddDays(5), Type = "КР" },
                new TaskItem { Id = 5, Title = "Английский - упражнения", Description = "Упражнения на времена", Deadline = DateTime.Now.AddDays(2), Type = "ДЗ" }
            };
        }

        private void InitializeCustomComponents()
        {
            // Настройка формы
            this.Text = "Задачи";
            this.Size = new Size(400, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        private void DisplayTasks()
        {
            content_block.Controls.Clear();

            // Группируем задачи по дате дедлайна и сортируем по дате
            var groupedTasks = _tasks
                .Where(t => !t.IsCompleted)
                .GroupBy(t => t.Deadline.Date)
                .OrderBy(g => g.Key);

            int currentY = 10;

            foreach (var group in groupedTasks)
            {
                // Создаем панель для даты
                Panel datePanel = CreateDatePanel(group.Key, currentY);
                content_block.Controls.Add(datePanel);
                currentY += datePanel.Height + 5;

                // Создаем плашки для каждой задачи в этой группе
                foreach (var task in group.OrderBy(t => t.Deadline))
                {
                    Panel taskPanel = CreateTaskPanel(task, currentY);
                    content_block.Controls.Add(taskPanel);
                    currentY += taskPanel.Height + 5;
                }

                currentY += 10; // Отступ между группами
            }
        }

        private Panel CreateDatePanel(DateTime date, int yPosition)
        {
            Panel panel = new Panel();
            panel.Size = new Size(content_block.Width - 30, 40);
            panel.Location = new Point(5, yPosition);
            //panel.BorderStyle = BorderStyle.FixedSingle;

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
            //panel.Padding = new Padding(10);

            // Заголовок задачи
            Label titleLabel = new Label();
            titleLabel.Text = $"{task.Type}: {task.Title}";
            titleLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            titleLabel.Location = new Point(10, 10);
            titleLabel.AutoSize = true;

            // Описание задачи
            Label descLabel = new Label();
            descLabel.Text = task.Description;
            descLabel.Font = new Font("Arial", 9);
            descLabel.Location = new Point(10, 35);
            descLabel.AutoSize = true;

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(descLabel);

            return panel;
        }

        private void task_schelude_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void task_settings_Click(object sender, EventArgs e)
        {
            Form2 settingsForm = new Form2();
            settingsForm.Show();

            this.Close();
        }

        private void task_add_Click(object sender, EventArgs e)
        {
            Add_task addTaskForm = new Add_task();
            addTaskForm.ShowDialog();
        }
    }
}