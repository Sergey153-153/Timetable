using mySQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class HolidaysForm : Form
    {
        private Form parentForm;
        private SQLiteQueries sqliteQ;

        public HolidaysForm(Form parent, SQLiteQueries db)
        {
            InitializeComponent();
            parentForm = parent;
            sqliteQ = db;

            LoadHolidaysFromDB();
        }
        
        private void LoadHolidaysFromDB()
        {
            panel1.Controls.Clear();

            var list = sqliteQ.GetAllHolidays();

            if (list == null || list.Count == 0)
            {
                Label lbl = new Label
                {
                    Text = "Праздников нет",
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                panel1.Controls.Add(lbl);
                return;
            }

            var sorted = list.OrderBy(h =>
            {
                if (h.IsAnnual)
                {
                    var parts = h.DateValue.Split('.');
                    return new DateTime(2000, int.Parse(parts[1]), int.Parse(parts[0]));
                }
                else
                {
                    return DateTime.ParseExact(h.DateValue, "yyyy.MM.dd", null);
                }
            }).ToList();


            int y = 10;

            foreach (var h in sorted)
            {
                Button btn = new Button
                {
                    Width = panel1.Width - 30,
                    Height = 40,
                    Location = new Point(10, y),
                    Tag = h,
                    Text = FormatHolidayText(h),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                btn.Click += HolidayButton_Click;

                panel1.Controls.Add(btn);
                y += 45;
            }
        }

        private string FormatHolidayText(Holiday h)
        {
            string date = h.IsAnnual ? h.DateValue : h.DateValue.Replace('.', '-');
            return $"{date}   —   {h.Name}";
        }

        private void HolidayButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Holiday h = (Holiday)btn.Tag;

            ShowEditHolidayForm(h);
        }

        private void ShowEditHolidayForm(Holiday holiday)
        {
            bool isNew = holiday == null;

            if (isNew)
                holiday = new Holiday { Name = "", DateValue = DateTime.Now.ToString("yyyy.MM.dd"), IsAnnual = false, Description = "" };

            Form f = new Form
            {
                Text = isNew ? "Добавить праздник" : "Редактировать праздник",
                Size = new Size(350, 350),
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label lbl1 = new Label { Text = "Название:", Location = new Point(20, 20) };
            TextBox tbName = new TextBox { Location = new Point(20, 45), Width = 280, Text = holiday.Name };

            Label lbl2 = new Label { Text = "Дата:", Location = new Point(20, 80) };
            DateTimePicker dtDate = new DateTimePicker
            {
                Location = new Point(20, 105),
                Width = 280,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = holiday.IsAnnual ? "dd.MM" : "yyyy.MM.dd"
            };

            if (holiday.IsAnnual)
            {
                dtDate.ShowUpDown = true;
                string[] parts = holiday.DateValue.Split('.');
                dtDate.Value = new DateTime(2000, int.Parse(parts[1]), int.Parse(parts[0]));
            }
            else
            {
                dtDate.Value = DateTime.ParseExact(holiday.DateValue, "yyyy.MM.dd", null);
            }

            CheckBox cbAnnual = new CheckBox
            {
                Text = "Ежегодный праздник",
                Location = new Point(20, 135),
                Checked = holiday.IsAnnual
            };
            cbAnnual.CheckedChanged += (s, e) =>
            {
                if (cbAnnual.Checked)
                {
                    dtDate.CustomFormat = "dd.MM";
                    dtDate.ShowUpDown = true;
                    dtDate.Value = new DateTime(2000, dtDate.Value.Month, dtDate.Value.Day);
                }
                else
                {
                    dtDate.CustomFormat = "yyyy.MM.dd";
                    dtDate.ShowUpDown = false;
                    dtDate.Value = new DateTime(dtDate.Value.Year, dtDate.Value.Month, dtDate.Value.Day);
                }
            };

            Label lbl3 = new Label { Text = "Описание:", Location = new Point(20, 160) };
            TextBox tbDesc = new TextBox
            {
                Location = new Point(20, 185),
                Width = 280,
                Height = 60,
                Multiline = true,
                Text = holiday.Description
            };

            Button btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(20, 260),
                Width = 120
            };

            Button btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(160, 260),
                Width = 120,
                Enabled = !isNew
            };

            f.Controls.AddRange(new Control[] { lbl1, tbName, lbl2, dtDate, cbAnnual, lbl3, tbDesc, btnSave, btnDelete });

            btnSave.Click += (s, e) =>
            {
                string dateValue = cbAnnual.Checked
                    ? dtDate.Value.ToString("dd.MM")
                    : dtDate.Value.ToString("yyyy.MM.dd");

                List<string> newData = new List<string>
                {
                    $"{tbName.Text};{dateValue};{(cbAnnual.Checked ? 1 : 0)};{tbDesc.Text}"
                };

                if (isNew)
                {
                    sqliteQ.AddHolidays(newData);
                }
                else
                {
                    sqliteQ.ReplaceHoliday(holiday.HolidayID, newData);
                }

                f.Close();
                LoadHolidaysFromDB();
            };

            btnDelete.Click += (s, e) =>
            {
                if (MessageBox.Show("Удалить праздник?", "Удаление",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    sqliteQ.DeleteHoliday(holiday.HolidayID);
                    f.Close();
                    LoadHolidaysFromDB();
                }
            };

            f.ShowDialog();
        }

        private void back_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void add_schelude_Click(object sender, EventArgs e)
        {
            ShowEditHolidayForm(null);
        }
    }
}