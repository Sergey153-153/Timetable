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
        public List<string> listCountry;
        public List<string> listRegion;
        public List<string> listCountryRegion;

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
                "1;1212;Двухнедельное расписание;2",
                "2;1111;Однонедельное расписание;1"
            };

            List<string> listLessons = new List<string>()
            {
                // Расписание 1
                "1;1;1;1;ТОК;Соловьев В.П.;27б;13:00;14:30",
                "1;2;1;1;История России;Пирогов Д.В.;ПСПШ;17:00;18:30",

                // Расписание 2
                "2;0;2;1;ТОК;Соловьев В.П.;27б;13:00;14:30",
                "2;0;2;2;ТОК;Соловьев В.П.;27б;14:40;16:10",
                "2;0;2;3;РЛП;Николаев К.Г.;27б;16:40;18:10",
                "2;0;2;4;Компьютерное моделирование;Соловьев А.В.;27б;18:20;20:35",
                "2;0;3;1;История России;Пирогов Д.В.;ПСПШ;17:00;18:30",
                "2;0;3;2;История России;Пирогов Д.В.;ПСПШ;18:35;20:05",
                "2;0;5;1;Применение IT в гуманитарной сфере;Глинников МВ;27б;10:30;12:00",
                "2;0;5;2;Применение IT в гуманитарной сфере;Глинников МВ;27б;12:10;13:35",
                "2;0;5;3;Экстримальные задачи;Пугачев О.В.;Т1;14:00;15:30",
                "2;0;5;4;Экстримальные задачи;Пугачев О.В.;Т1;15:40;17:10",
                "2;0;5;5;История России;Пирогов Д.В.;Т6;17:20;18:50",
                "2;0;6;1;ТРПО;Пуцко Н.Н.;115;09:00;10:30",
                "2;0;6;2;ТРПО;Пуцко Н.Н.;115;10:40;12:10",
                "2;0;6;3;АИС;Пуцко Н.Н.;115;12:30;14:00",
                "2;0;6;4;АИС;Пуцко Н.Н.;115;14:10;14:55"
            };

            ParametersCollection paramss = new ParametersCollection();
            int cntErr = 0;

            for (int i = 0; i < listSchedules.Count; i++)
            {
                string[] arrSchedule = listSchedules[i].Split(';');
                paramss.Clear();
                paramss.Add("ScheduleID", arrSchedule[0], System.Data.DbType.Int32);
                paramss.Add("Code", arrSchedule[1], System.Data.DbType.String);
                paramss.Add("Name", arrSchedule[2], System.Data.DbType.String);
                paramss.Add("Type", arrSchedule[3], System.Data.DbType.Int32);

                if (sqliteQ._sqlt.Insert("Schedules", paramss) == 0)
                    cntErr++;
            }
            MessageBox.Show($"Данные о расписаниях: Обработано записей: {listSchedules.Count}. Ошибок: {cntErr}.");

            cntErr = 0;
            for (int i = 0; i < listLessons.Count; i++)
            {
                string[] arrLesson = listLessons[i].Split(';');
                paramss.Clear();
                paramss.Add("ScheduleID", arrLesson[0], System.Data.DbType.Int32);
                paramss.Add("WeekNumber", arrLesson[1], System.Data.DbType.Int32);
                paramss.Add("DayOfWeek", arrLesson[2], System.Data.DbType.Int32);
                paramss.Add("LessonNumber", arrLesson[3], System.Data.DbType.Int32);
                paramss.Add("Subject", arrLesson[4], System.Data.DbType.String);
                paramss.Add("Teacher", arrLesson[5], System.Data.DbType.String);
                paramss.Add("Location", arrLesson[6], System.Data.DbType.String);
                paramss.Add("StartTime", arrLesson[7], System.Data.DbType.String);
                paramss.Add("EndTime", arrLesson[8], System.Data.DbType.String);

                if (sqliteQ._sqlt.Insert("Lessons", paramss) == 0)
                    cntErr++;
            }
            MessageBox.Show($"Данные об уроках: Обработано записей: {listLessons.Count}. Ошибок: {cntErr}.");
        }


        private void loadFromDBCountry()
        {
            //dictCountry = sqliteQ.getListCountry();
            cbCountry.Items.Clear();
            foreach (KeyValuePair<string, int> pair in dictCountry)
            {
                cbCountry.Items.Add(pair.Key);
            }
            if (cbCountry.Items.Count > 0)
                cbCountry.SelectedIndex = 0;
        }

        private void loadFromDBRegion()
        {
            string curCountry = cbCountry.SelectedItem == null ? "" : cbCountry.SelectedItem.ToString();

            //listInfoRegion = sqliteQ.getListRegion(curCountry);
            dgvRegion.DataSource = listInfoRegion;
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
            MoreLesson f2 = new MoreLesson(this);
            this.Hide();
            f2.Show();
        }
    }
}
