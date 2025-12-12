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

        private int _editTaskId = -1;

        public Add_task(SQLiteQueries db)
        {
            InitializeComponent();
            _db = db;
            InitializeForm();
            LoadSubjects();
        }

        // Конструктор для редактирования существующего задания
        public Add_task(SQLiteQueries db, TaskItem taskToEdit) : this(db)
        {
            _editTaskId = taskToEdit.Id;

            textBox_description.Text = taskToEdit.Description;

            dateTimePicker1.MinDate = DateTimePicker.MinimumDateTime;
            dateTimePicker1.MaxDate = DateTimePicker.MaximumDateTime;

            dateTimePicker1.Value = taskToEdit.Deadline;

            _selectedFilePath = taskToEdit.FilePath;
            Lesson_type.Text = taskToEdit.SubjectName;

            switch (taskToEdit.Type)
            {
                case "ДЗ": HomeTask.Checked = true; break;
                case "КР": Control.Checked = true; break;
                case "Зачет": Test.Checked = true; break;
                case "Экзамен": Exam.Checked = true; break;
            }

            this.Text = "Редактирование задания";
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

            try
            {
                if (_editTaskId != -1)
                {
                    // Удаляем старое и добавляем новое
                    bool deleted = _db.DeleteTask(_editTaskId);

                    int newId = _db.AddSimpleTask(
                        $"{taskType} - {dateTimePicker1.Value:dd.MM.yyyy}",
                        textBox_description.Text,
                        dateTimePicker1.Value,
                        taskType,
                        subjectName,
                        _selectedFilePath
                    );
                }
                else
                {
                    int newId = _db.AddSimpleTask(
                        $"{taskType} - {dateTimePicker1.Value:dd.MM.yyyy}",
                        textBox_description.Text,
                        dateTimePicker1.Value,
                        taskType,
                        subjectName,
                        _selectedFilePath
                    );
                }

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
