using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SQLiteProject
{
    public partial class HolidaysForm : Form
    {
        private Form parentForm;

        // Конструктор с родительской формой
        public HolidaysForm(Form parent)
        {
            InitializeComponent();
            parentForm = parent;

            // Делаем такого же размера как родительская форма
            if (parent != null)
            {
                this.Size = new System.Drawing.Size(
                    Math.Min(parent.Width - 50, 600), // Но не больше 600
                    Math.Min(parent.Height - 50, 450) // И не больше 450
                );
            }

            LoadCompactHolidays();
        }

        // Старый конструктор для обратной совместимости
        public HolidaysForm() : this(null)
        {
        }

        private void LoadCompactHolidays()
        {
            List<Holiday> holidays = new List<Holiday>
            {
                new Holiday { Name = "Новый год", Date = "01.01" },
                new Holiday { Name = "Рождество", Date = "07.01" },
                new Holiday { Name = "23 февраля", Date = "23.02" },
                new Holiday { Name = "8 марта", Date = "08.03" },
                new Holiday { Name = "1 мая", Date = "01.05" },
                new Holiday { Name = "9 мая", Date = "09.05" },
                new Holiday { Name = "12 июня", Date = "12.06" },
                new Holiday { Name = "4 ноября", Date = "04.11" }
            };

            listBoxHolidays.Items.Clear();
            listBoxHolidays.Items.Add("ГОСУДАРСТВЕННЫЕ ПРАЗДНИКИ РФ:");
            listBoxHolidays.Items.Add("========================");

            foreach (var holiday in holidays)
            {
                listBoxHolidays.Items.Add($"  {holiday.Date} - {holiday.Name}");
            }

            this.Text = "Праздники РФ";
        }
    }
}