using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteProject
{
    partial class Form2
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.Button btnDownloadSchedule;
        private System.Windows.Forms.Button btnUploadSchedule;
        private System.Windows.Forms.Button btnHolidays;
        private System.Windows.Forms.Button btnTasks;
        private System.Windows.Forms.Button btnScheduleSettings;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 450);
            this.Text = "Настройки";
            this.BackColor = System.Drawing.Color.White;
            this.Padding = new System.Windows.Forms.Padding(20);

            InitializeControls();
            this.ResumeLayout(false);
        }

        #endregion

        private void InitializeControls()
        {
            mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.RowCount = 5;
            mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));

            btnDownloadSchedule = CreateSettingsButton("Скачать расписание", 0);

            btnUploadSchedule = CreateSettingsButton("Закачать расписание", 1);

            btnHolidays = CreateSettingsButton("Список праздников", 2);

            btnTasks = CreateSettingsButton("Задания", 3);

            btnScheduleSettings = CreateSettingsButton("Расписание", 4);

            mainTableLayout.Controls.Add(btnDownloadSchedule, 0, 0);
            mainTableLayout.Controls.Add(btnUploadSchedule, 0, 1);
            mainTableLayout.Controls.Add(btnHolidays, 0, 2);
            mainTableLayout.Controls.Add(btnTasks, 0, 3);
            mainTableLayout.Controls.Add(btnScheduleSettings, 0, 4);

            this.Controls.Add(mainTableLayout);
        }

        private System.Windows.Forms.Button CreateSettingsButton(string text, int row)
        {
            var button = new System.Windows.Forms.Button();
            button.Dock = System.Windows.Forms.DockStyle.Fill;
            button.Text = text;
            button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            button.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = System.Drawing.Color.White;
            button.ForeColor = System.Drawing.Color.Black;
            button.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            button.Height = 70;
            button.Margin = new System.Windows.Forms.Padding(0);

            button.MouseEnter += (s, e) => {
                button.BackColor = System.Drawing.Color.LightGray;
            };
            button.MouseLeave += (s, e) => {
                button.BackColor = System.Drawing.Color.White;
            };

            button.Click += (s, e) => HandleSettingsClick(text);

            return button;
        }
        /*
        private void HandleSettingsClick(string settingType)
        {
            switch (settingType)
            {
                case "Скачать расписание":
                    DownloadSchedule();
                    break;
                case "Закачать расписание":
                    UploadSchedule();
                    break;
                case "Список праздников":
                    OpenHolidaysSettings();
                    break;
                case "Задания":
                    OpenTaskSettings();
                    break;
                case "Расписание":
                    OpenScheduleSettings();
                    break;
            }
        }*/
        /*
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
        }*/        /*
        private void UploadSchedule()
        {
            using (System.Windows.Forms.Form inputForm = new System.Windows.Forms.Form())
            {
                inputForm.Width = 400;
                inputForm.Height = 180;
                inputForm.Text = "Загрузка расписания";
                inputForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                inputForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;

                System.Windows.Forms.Label lbl = new System.Windows.Forms.Label() { Left = 10, Top = 10, Text = "Введите код расписания:", AutoSize = true };
                System.Windows.Forms.TextBox txt = new System.Windows.Forms.TextBox() { Left = 10, Top = 40, Width = 360 };
                System.Windows.Forms.Button btnOk = new System.Windows.Forms.Button() { Text = "Загрузить", Left = 200, Width = 80, Top = 80, DialogResult = System.Windows.Forms.DialogResult.OK };
                System.Windows.Forms.Button btnCancel = new System.Windows.Forms.Button() { Text = "Отмена", Left = 290, Width = 80, Top = 80, DialogResult = System.Windows.Forms.DialogResult.Cancel };

                inputForm.Controls.Add(lbl);
                inputForm.Controls.Add(txt);
                inputForm.Controls.Add(btnOk);
                inputForm.Controls.Add(btnCancel);

                inputForm.AcceptButton = btnOk;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                string code = txt.Text.Trim();

                if (string.IsNullOrWhiteSpace(code))
                {
                    System.Windows.Forms.MessageBox.Show("Введите код расписания!");
                    return;
                }

                try
                {
                    var info = sqliteQ.getScheduleByCode(code);

                    if (info == null)
                    {
                        System.Windows.Forms.MessageBox.Show("Расписание с таким кодом не найдено!");
                        return;
                    }

                    var result = System.Windows.Forms.MessageBox.Show(
                        "Вы уверены, что хотите заменить текущее расписание? Это действие нельзя отменить.",
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

                    System.Windows.Forms.MessageBox.Show("Расписание успешно загружено!");
                    mainForm.RefreshAllSchedulesData();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Ошибка при загрузке расписания: {ex.Message}");
                }
            }
        }*/

        private void OpenHolidaysSettings()
        {
            System.Windows.Forms.MessageBox.Show("Функция 'Список праздников' в разработке", "Внимание",
                          System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        private void OpenTaskSettings()
        {
            System.Windows.Forms.MessageBox.Show("Функция 'задания' в разработке", "Внимание",
                          System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        private void OpenScheduleSettings()
        {
            System.Windows.Forms.MessageBox.Show("Функция 'расписание' в разработке", "Внимание",
                          System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
    }
}