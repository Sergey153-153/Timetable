using mySQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class Add_task : Form
    {
        private SQLiteQueries _db; 
        private string _selectedFilePath = "";

        public Add_task(SQLiteQueries db)
        {
            InitializeComponent();
            _db = db;
            InitializeForm();
            LoadSubjects();
        }

        private void InitializeForm()
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd.MM.yyyy";
            dateTimePicker1.Value = DateTime.Now.AddDays(1);
            dateTimePicker1.MinDate = DateTime.Now;

            Lesson_type.DropDownStyle = ComboBoxStyle.DropDown;
        }

        private void LoadSubjects()
        {
            Lesson_type.Items.Clear();
            if (_db != null)
            {
                List<SubjectItem> subjects = _db.GetAllSubjects();
                foreach (var s in subjects)
                {
                    Lesson_type.Items.Add(s);
                }
            }
            else
            {
                // дефолтные предметы
                string[] defaultSubjects = { "Физика", "ТРПО", "АИС" };
                foreach (var s in defaultSubjects)
                {
                    Lesson_type.Items.Add(new SubjectItem { Name = s });
                }
            }

            Lesson_type.DisplayMember = "Name";
            Lesson_type.ValueMember = "Name";
        }

        private string GetOrCreateSubjectName()
        {
            if (string.IsNullOrWhiteSpace(Lesson_type.Text))
            {
                MessageBox.Show("Введите название предмета!");
                return null;
            }

            string subjectName = Lesson_type.Text.Trim();

            if (_db != null && !_db.SubjectExists(subjectName))
            {
                _db.AddSubject(subjectName);
                LoadSubjects();
                Lesson_type.Text = subjectName;
                MessageBox.Show($"Предмет '{subjectName}' добавлен в базу данных");
            }

            return subjectName;
        }

        private void SaveTaskToDatabase(string description, string type, string subjectName, DateTime deadline, string filePath)
        {
            if (_db == null)
            {
                MessageBox.Show("Ошибка: подключение к БД не установлено!");
                return;
            }

            string title = $"{type} - {deadline:dd.MM.yyyy}";
            _db.AddSimpleTask(title, description, deadline, type, subjectName, filePath);
        }

        private string GetSelectedTaskType()
        {
            if (HomeTask.Checked) return "ДЗ";
            if (Control.Checked) return "КР";
            if (Test.Checked) return "Зачет";
            if (Exam.Checked) return "Экзамен";
            return "";
        }

        private void back_tasks_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox_description.Text))
                {
                    MessageBox.Show("Введите описание задания!");
                    return;
                }

                string taskType = GetSelectedTaskType();
                if (string.IsNullOrEmpty(taskType))
                {
                    MessageBox.Show("Выберите тип задания!");
                    return;
                }

                string subjectName = GetOrCreateSubjectName();
                if (string.IsNullOrEmpty(subjectName)) return;

                SaveTaskToDatabase(
                    textBox_description.Text,
                    taskType,
                    subjectName,
                    dateTimePicker1.Value,
                    _selectedFilePath
                );

                MessageBox.Show("Задание успешно добавлено!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void Add_files_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Все файлы (*.*)|*.*";
                openFileDialog.Title = "Выберите файл для задания";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _selectedFilePath = openFileDialog.FileName;
                    MessageBox.Show($"Файл выбран: {Path.GetFileName(_selectedFilePath)}");
                }
            }
        }
    }
}
