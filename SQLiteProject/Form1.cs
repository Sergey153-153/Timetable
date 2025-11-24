using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SQLite;
using DatabaseLib;
using mySQLite;

namespace SQLiteProject
{
    public partial class Form1 : Form
    {
        SQLiteQueries sqliteQ;
        
        private const int COUNT_TABLES_IN_DB = 3; //кол-во таблиц в БД
        public List<string> listSchedules;
        public List<string> listLessons;

        public Dictionary<string, int> dictCountry = new Dictionary<string, int>();

        public class InfoRegion
        {
            public int region_id { get; set; }
            public string region_name { get; set; }
        }
        private List<InfoRegion> listInfoRegion;

        public Form1()
        {
            InitializeComponent();
        }


        private void connectToDB()
        {
            string dbName = "testDB.db";
            int flag = 0;
            if (!File.Exists(dbName))
            {
                DialogResult result = MessageBox.Show("Файл с базой данных не найден. Создать новую базу данных?",
                                                      "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                    Application.Exit();
                flag += 1;
                SQLiteConnection.CreateFile(dbName);
            }
            sqliteQ = new SQLiteQueries(dbName);

            if (sqliteQ._sqlt.GetTables().Count != COUNT_TABLES_IN_DB)
            {
                if (flag == 0)
                {
                    DialogResult result = MessageBox.Show("В файле БД отсутствуют необходимые таблицы. Пересоздать таблицы?",
                                          "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                        Application.Exit();
                }
                sqliteQ.CreateTables(dbName);

                //Если БД была новая, то заполним ее тестовыми данными
                saveDataToDB();
            }
        }

        private void saveDataToDB()
        {
            List<string> listSchedules = new List<string>()
            {
                "1;0000;основное;2",
                "2;1212;Двухнедельное расписание;2",
                "3;1111;Однонедельное расписание;1"
            };

            List<string> listLessons = new List<string>()
            {
                // основное 1
                "1;1;1;1;ТОК;Соловьев В.П.;27б;13:00;14:30",
                "1;2;1;1;История России;Пирогов Д.В.;ПСПШ;17:00;18:30",

                // Расписание 1
                "2;1;1;1;ТОК;Соловьев В.П.;27б;13:00;14:30",
                "2;2;1;1;История России;Пирогов Д.В.;ПСПШ;17:00;18:30",

                // Расписание 2
                "3;0;2;1;ТОК;Соловьев В.П.;27б;13:00;14:30",
                "3;0;2;2;ТОК;Соловьев В.П.;27б;14:40;16:10",
                "3;0;2;3;РЛП;Николаев К.Г.;27б;16:40;18:10",
                "3;0;2;4;Компьютерное моделирование;Соловьев А.В.;27б;18:20;20:35",
                "3;0;3;1;История России;Пирогов Д.В.;ПСПШ;17:00;18:30",
                "3;0;3;2;История России;Пирогов Д.В.;ПСПШ;18:35;20:05",
                "3;0;5;1;Применение IT в гуманитарной сфере;Глинников МВ;27б;10:30;12:00",
                "3;0;5;2;Применение IT в гуманитарной сфере;Глинников МВ;27б;12:10;13:35",
                "3;0;5;3;Экстримальные задачи;Пугачев О.В.;Т1;14:00;15:30",
                "3;0;5;4;Экстримальные задачи;Пугачев О.В.;Т1;15:40;17:10",
                "3;0;5;5;История России;Пирогов Д.В.;Т6;17:20;18:50",
                "3;0;6;1;ТРПО;Пуцко Н.Н.;115;09:00;10:30",
                "3;0;6;2;ТРПО;Пуцко Н.Н.;115;10:40;12:10",
                "3;0;6;3;АИС;Пуцко Н.Н.;115;12:30;14:00",
                "3;0;6;4;АИС;Пуцко Н.Н.;115;14:10;14:55"
            };

            ParametersCollection paramss = new ParametersCollection();

            int errSchedules = sqliteQ.AddSchedules(listSchedules);
            MessageBox.Show($"Обработано расписаний: {listSchedules.Count}, ошибок: {errSchedules}");

            int errLessons = sqliteQ.AddLessons(listLessons);
            MessageBox.Show($"Обработано уроков: {listLessons.Count}, ошибок: {errLessons}");

        }

        public void RefreshAllSchedulesData()
        {
            // 1. Обновляем ComboBox с расписаниями
            if (cbCountry != null)
            {
                cbCountry.Items.Clear();
                DataTable dt = sqliteQ._sqlt.FetchByColumn("Schedules", "ScheduleID, Name", "", "ORDER BY ScheduleID");
                foreach (DataRow row in dt.Rows)
                {
                    cbCountry.Items.Add(new KeyValuePair<int, string>(
                        int.Parse(row["ScheduleID"].ToString()),
                        row["Name"].ToString()
                    ));
                }
                if (cbCountry.Items.Count > 0)
                    cbCountry.SelectedIndex = 0;
            }

            // 2. Можно добавить сюда обновление любых других контролов,
            // которые зависят от расписаний
        }

        private void loadFromDBCountry()
        {
            cbCountry.Items.Clear();
            DataTable dt = sqliteQ._sqlt.FetchByColumn("Schedules", "ScheduleID, Name", "", "ORDER BY ScheduleID");

            foreach (DataRow row in dt.Rows)
            {
                cbCountry.Items.Add(new KeyValuePair<int, string>(
                    int.Parse(row["ScheduleID"].ToString()),
                    row["Name"].ToString()
                ));
            }

            if (cbCountry.Items.Count > 0)
                cbCountry.SelectedIndex = 0;
        }

        private void loadFromDBRegion()
        {
            string curCountry = cbCountry.SelectedItem == null ? "" : cbCountry.SelectedItem.ToString();

            //listInfoRegion = sqliteQ.getListRegion(curCountry);
            //dgvRegion.DataSource = listInfoRegion;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            connectToDB();
            //saveDataToDB();
            loadFromDBCountry();
        }

        private void cbCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadFromDBRegion();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MoreLesson f2 = new MoreLesson(this, sqliteQ, 1);
            f2.StartPosition = FormStartPosition.CenterParent;
            f2.StartPosition = FormStartPosition.Manual;  // чтобы позиция была как у Form1
            f2.Location = this.Location;                  // ставим точно поверх Form1

            f2.Show();        // показываем
            f2.BringToFront(); // поднимаем вверх
            f2.Activate();     // делаем активной
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
