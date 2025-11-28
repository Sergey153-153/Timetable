using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class Add_task : Form
    {
        private SQLiteConnection _connection;
        private string _selectedFilePath = "";

        public Add_task()
        {
            InitializeComponent();
            InitializeForm();
        }

        public void SetConnection(SQLiteConnection connection)
        {
            _connection = connection;
        }

        private void InitializeForm()
        {
            LoadSubjects();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd.MM.yyyy";
            dateTimePicker1.Value = DateTime.Now.AddDays(1);
            dateTimePicker1.MinDate = DateTime.Now;
            dateTimePicker1.ShowUpDown = false;

            Lesson_type.DropDownStyle = ComboBoxStyle.DropDown;
        }

        private void LoadSubjects()
        {
            if (_connection == null)
            {
                LoadDefaultSubjects();
                return;
            }

            try
            {
                Lesson_type.Items.Clear();
                string sql = "SELECT Id, Name FROM Subjects ORDER BY Name";
                using (var command = new SQLiteCommand(sql, _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Lesson_type.Items.Add(new SubjectItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }

                Lesson_type.DisplayMember = "Name";
                Lesson_type.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки предметов: {ex.Message}");
                LoadDefaultSubjects();
            }
        }

        private void LoadDefaultSubjects()
        {
            Lesson_type.Items.Clear();
            Lesson_type.Items.Add(new SubjectItem { Id = 1, Name = "Физика" });
            Lesson_type.Items.Add(new SubjectItem { Id = 2, Name = "ТРПО" });
            Lesson_type.Items.Add(new SubjectItem { Id = 3, Name = "АИС" });
        }

        private int GetOrCreateSubjectId()
        {
            if (string.IsNullOrWhiteSpace(Lesson_type.Text))
            {
                MessageBox.Show("Введите название предмета!");
                return 0;
            }

            string subjectName = Lesson_type.Text.Trim();

            if (Lesson_type.SelectedItem != null && Lesson_type.SelectedItem is SubjectItem selectedSubject)
            {
                return selectedSubject.Id;
            }

            return FindOrCreateSubject(subjectName);
        }

        private int FindOrCreateSubject(string subjectName)
        {
            if (_connection == null) return 0;

            try
            {
                // Используем INSERT OR IGNORE чтобы избежать ошибок дубликатов
                string sql = @"INSERT OR IGNORE INTO Subjects (Name) VALUES (@Name);
                      SELECT Id FROM Subjects WHERE Name = @Name;";

                using (var command = new SQLiteCommand(sql, _connection))
                {
                    command.Parameters.AddWithValue("@Name", subjectName);
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        int subjectId = Convert.ToInt32(result);

                        // Если это новый предмет - обновляем список
                        if (IsNewSubject(subjectName))
                        {
                            ForceReloadSubjects();
                            SelectNewSubject(subjectName);
                            MessageBox.Show($"Предмет '{subjectName}' добавлен в базу данных");
                        }

                        return subjectId;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка работы с предметами: {ex.Message}");
                return 0;
            }
        }

        private bool IsNewSubject(string subjectName)
        {
            // Проверяем, есть ли предмет в текущем ComboBox
            foreach (SubjectItem item in Lesson_type.Items)
            {
                if (item.Name == subjectName)
                    return false;
            }
            return true;
        }

        // перезагрузка списка предметов
        private void ForceReloadSubjects()
        {
            try
            {
                string currentText = Lesson_type.Text;

                Lesson_type.Items.Clear();

                string sql = "SELECT Id, Name FROM Subjects ORDER BY Name";
                using (var command = new SQLiteCommand(sql, _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Lesson_type.Items.Add(new SubjectItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }

                Lesson_type.DisplayMember = "Name";
                Lesson_type.ValueMember = "Id";
                Lesson_type.Text = currentText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перезагрузки предметов: {ex.Message}");
            }
        }

        // ВЫБОР НОВОГО ПРЕДМЕТА В COMBOBOX
        private void SelectNewSubject(string subjectName)
        {
            foreach (SubjectItem item in Lesson_type.Items)
            {
                if (item.Name == subjectName)
                {
                    Lesson_type.SelectedItem = item;
                    return;
                }
            }

            Lesson_type.Text = subjectName;
        }

        private void SaveTaskToDatabase(string description, string type, int subjectId, DateTime deadline, string filePath)
        {
            if (_connection == null)
            {
                MessageBox.Show("Ошибка: подключение к БД не установлено!");
                return;
            }

            string sql = @"INSERT INTO Tasks (Title, Description, Deadline, Type, SubjectId, FilePath, IsCompleted) 
                          VALUES (@Title, @Description, @Deadline, @Type, @SubjectId, @FilePath, @IsCompleted)";

            using (var command = new SQLiteCommand(sql, _connection))
            {
                string title = $"{type} - {deadline:dd.MM.yyyy}";
                DateTime dateOnly = deadline.Date;

                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Deadline", dateOnly);
                command.Parameters.AddWithValue("@Type", type);
                command.Parameters.AddWithValue("@SubjectId", subjectId);
                command.Parameters.AddWithValue("@FilePath", filePath ?? "");
                command.Parameters.AddWithValue("@IsCompleted", false);

                command.ExecuteNonQuery();
            }
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

                if (_connection == null)
                {
                    MessageBox.Show("Ошибка: подключение к базе данных не установлено!");
                    return;
                }

                int subjectId = GetOrCreateSubjectId();
                if (subjectId == 0)
                {
                    return;
                }

                SaveTaskToDatabase(
                    textBox_description.Text,
                    taskType,
                    subjectId,
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
                    MessageBox.Show($"Файл выбран: {System.IO.Path.GetFileName(_selectedFilePath)}");
                }
            }
        }
    }
}