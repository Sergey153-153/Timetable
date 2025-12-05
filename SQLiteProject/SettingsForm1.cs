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

namespace SQLiteProject
{
    public partial class SettingsForm1 : Form
    {
        private Form1 form1;
        private SQLiteQueries sqliteQ;

        //private int z;

        public SettingsForm1(Form1 parentForm, SQLiteQueries db)
        {
            InitializeComponent();

            form1 = parentForm;
            sqliteQ = db;
        }
        public SettingsForm1()
        {
            InitializeComponent();
        }
        private void DownloadSchedule()
        {
            using (System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовый файл (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.Title = "Экспорт расписания";
                saveFileDialog.FileName = $"расписание_{DateTime.Now:yyyy_dd_MM}.txt";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        var scheduleData = sqliteQ.getOneWeekLessons(1);

                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.UTF8))
                        {
                            writer.WriteLine("ЭКСПОРТ РАСПИСАНИЯ");
                            writer.WriteLine($"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm}");
                            writer.WriteLine();

                            var days = new Dictionary<int, string>
                    {
                        { 1, "ПОНЕДЕЛЬНИК" },
                        { 2, "ВТОРНИК" },
                        { 3, "СРЕДА" },
                        { 4, "ЧЕТВЕРГ" },
                        { 5, "ПЯТНИЦА" },
                        { 6, "СУББОТА" },
                        { 7, "ВОСКРЕСЕНЬЕ" }
                    };

                            foreach (var day in days)
                            {
                                var dayLessons = scheduleData.Where(l => l.DayOfWeek == day.Key)
                                                           .OrderBy(l => l.LessonNumber)
                                                           .ToList();

                                if (dayLessons.Count > 0)
                                {
                                    writer.WriteLine(day.Value);
                                    writer.WriteLine(new string('-', 40));

                                    foreach (var lesson in dayLessons)
                                    {
                                        writer.WriteLine($"Пара {lesson.LessonNumber}: {lesson.Time}");
                                        writer.WriteLine($"  Предмет: {lesson.Subject}");
                                        writer.WriteLine($"  Преподаватель: {lesson.Teacher}");
                                        writer.WriteLine($"  Аудитория: {lesson.Location}");
                                        writer.WriteLine();
                                    }
                                    writer.WriteLine();
                                }
                            }
                        }

                        System.Windows.Forms.MessageBox.Show($"Расписание успешно экспортировано в файл:\n{saveFileDialog.FileName}",
                            "Экспорт завершен",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Ошибка при экспорте: {ex.Message}",
                            "Ошибка",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UploadSchedule()
        {
            using (System.Windows.Forms.Form inputForm = new System.Windows.Forms.Form())
            {
                inputForm.Width = 450;
                inputForm.Height = 350;
                inputForm.Text = "Загрузка расписания";
                inputForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                inputForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;

                System.Windows.Forms.Label lbl = new System.Windows.Forms.Label()
                {
                    Left = 10,
                    Top = 10,
                    Text = "Выберите расписание из списка:",
                    AutoSize = true
                };

                System.Windows.Forms.ListBox listBox = new System.Windows.Forms.ListBox()
                {
                    Left = 10,
                    Top = 35,
                    Width = 410,
                    Height = 200
                };

                System.Windows.Forms.Button btnOk = new System.Windows.Forms.Button()
                {
                    Text = "Загрузить",
                    Left = 240,
                    Width = 80,
                    Top = 250,
                    DialogResult = System.Windows.Forms.DialogResult.OK
                };

                System.Windows.Forms.Button btnCancel = new System.Windows.Forms.Button()
                {
                    Text = "Отмена",
                    Left = 330,
                    Width = 80,
                    Top = 250,
                    DialogResult = System.Windows.Forms.DialogResult.Cancel
                };

                inputForm.Controls.Add(lbl);
                inputForm.Controls.Add(listBox);
                inputForm.Controls.Add(btnOk);
                inputForm.Controls.Add(btnCancel);

                inputForm.AcceptButton = btnOk;
                inputForm.CancelButton = btnCancel;

                // Загружаем список расписаний
                try
                {
                    var schedules = sqliteQ.GetAllSchedules();

                    if (schedules == null || schedules.Count == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("В базе данных нет доступных расписаний!",
                            "Ошибка",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Warning);
                        return;
                    }

                    foreach (var schedule in schedules)
                    {
                        string displayText = $"{schedule.Code} - {schedule.Name}";
                        listBox.Items.Add(new ScheduleListItem
                        {
                            DisplayText = displayText,
                            ScheduleCode = schedule.Code,
                            ScheduleID = schedule.ScheduleID
                        });
                    }

                    // Выделяем первый элемент по умолчанию
                    if (listBox.Items.Count > 0)
                    {
                        listBox.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Ошибка при загрузке списка расписаний: {ex.Message}",
                        "Ошибка",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                if (inputForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                if (listBox.SelectedItem == null)
                {
                    System.Windows.Forms.MessageBox.Show("Выберите расписание из списка!");
                    return;
                }

                ScheduleListItem selectedItem = (ScheduleListItem)listBox.SelectedItem;
                string code = selectedItem.ScheduleCode;

                try
                {
                    var info = sqliteQ.getScheduleByCode(code);

                    if (info == null)
                    {
                        System.Windows.Forms.MessageBox.Show("Расписание с таким кодом не найдено!");
                        return;
                    }

                    var result = System.Windows.Forms.MessageBox.Show(
                        $"Вы уверены, что хотите заменить текущее расписание на \"{info.Name}\" (код: {info.Code})? Это действие нельзя отменить.",
                        "Подтверждение замены",
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question);

                    if (result != System.Windows.Forms.DialogResult.Yes)
                        return;

                    int targetID = 1;

                    if (sqliteQ.DeleteSchedule(targetID) == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Ошибка удаления старого расписания!");
                        return;
                    }

                    int res = sqliteQ.CopySchedule(info.ScheduleID, targetID);

                    if (res == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Ошибка копирования нового расписания!");
                        return;
                    }

                    // Очищаем таблицу LessonOverrides
                    sqliteQ.ClearLessonOverrides();

                    System.Windows.Forms.MessageBox.Show("Расписание успешно загружено!");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Ошибка при загрузке расписания: {ex.Message}");
                }
            }
        }

        // Вспомогательный класс для хранения информации о расписании в ListBox
        private class ScheduleListItem
        {
            public string DisplayText { get; set; }
            public string ScheduleCode { get; set; }
            public int ScheduleID { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DownloadSchedule();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UploadSchedule();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HolidaysForm holidaysForm = new HolidaysForm(this); // Передаем текущую форму как родительскую
            holidaysForm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Tasks tasksForm = new Tasks();
            this.Hide();
            tasksForm.ShowDialog();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
