using mySQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace SQLiteProject
{
    public partial class Tasks : Form
    {
        private Form1 form1;
        private SQLiteQueries sqliteQ;
        private List<TaskItem> _tasks;

        public Tasks()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Настройка формы
            this.Text = "Задачи";
            this.Size = new Size(375, 700);
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

        private void buttonSchelude_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSetting_Click(object sender, EventArgs e)
        {
            SettingsForm1 settings = new SettingsForm1(form1, sqliteQ);
            this.Hide();
            settings.ShowDialog();
            this.Close();
        }
    }
}
