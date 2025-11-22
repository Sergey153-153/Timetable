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
            /*int id1 = 1;
            sqliteQ.AddSchedule(id1, "1212", "Двухнедельное расписание", 2);

            sqliteQ.AddLesson(id1, 1, 1, 1, "ТОК", "Соловьев В.П.", "27б", "13:00", "14:30");
            sqliteQ.AddLesson(id1, 2, 1, 1, "История России", "Пирогов Д.В.", "ПСПШ", "17:00", "18:30");

            int id2 = 2;
            sqliteQ.AddSchedule(id2, "1111", "Однонедельное расписание", 1);

            sqliteQ.AddLesson(id2, 0, 2, 1, "ТОК", "Соловьев В.П.", "27б", "13:00", "14:30");
            sqliteQ.AddLesson(id2, 0, 2, 2, "ТОК", "Соловьев В.П.", "27б", "14:40", "16:10");
            sqliteQ.AddLesson(id2, 0, 2, 3, "РЛП", "Николаев К.Г.", "27б", "16:40", "18:10");
            sqliteQ.AddLesson(id2, 0, 2, 4, "Компьютерное моделирование", "Соловьев А.В.", "27б", "18:20", "20:35");

            sqliteQ.AddLesson(id2, 0, 3, 1, "История России", "Пирогов Д.В.", "ПСПШ", "17:00", "18:30");
            sqliteQ.AddLesson(id2, 0, 3, 2, "История России", "Пирогов Д.В.", "ПСПШ", "18:35", "20:05");

            sqliteQ.AddLesson(id2, 0, 5, 1, "Применение IT в гуманитарной сфере", "Глинников МВ", "27б", "10:30", "12:00");
            sqliteQ.AddLesson(id2, 0, 5, 2, "Применение IT в гуманитарной сфере", "Глинников МВ", "27б", "12:10", "13:35");
            sqliteQ.AddLesson(id2, 0, 5, 3, "Экстримальные задачи", "Пугачев О.В.", "Т1", "14:00", "15:30");
            sqliteQ.AddLesson(id2, 0, 5, 4, "Экстримальные задачи", "Пугачев О.В.", "Т1", "15:40", "17:10");
            sqliteQ.AddLesson(id2, 0, 5, 5, "История России", "Пирогов Д.В.", "Т6", "17:20", "18:50");

            sqliteQ.AddLesson(id2, 0, 6, 1, "ТРПО", "Пуцко Н.Н.", "115", "9:00", "10:30");
            sqliteQ.AddLesson(id2, 0, 6, 2, "ТРПО", "Пуцко Н.Н.", "115", "10:40", "12:10");
            sqliteQ.AddLesson(id2, 0, 6, 3, "АИС", "Пуцко Н.Н.", "115", "12:30", "14:00");
            sqliteQ.AddLesson(id2, 0, 6, 4, "АИС", "Пуцко Н.Н.", "115", "14:10", "14:55"); */
            /*
            listCountry = new List<string>();
            listCountry.Add("1;Россия;-");
            listCountry.Add("2;Беларусь;-");

            listRegion = new List<string>();
            listRegion.Add("1;Москва;-");
            listRegion.Add("2;Санкт-Петербург;-");
            listRegion.Add("3;Минск;-");

            listCountryRegion = new List<string>();
            listCountryRegion.Add("1;1");
            listCountryRegion.Add("2;1");
            listCountryRegion.Add("3;2");

            ParametersCollection paramss = new ParametersCollection();
            int cntErr = 0;

            for (int i = 0; i < listCountry.Count; i++)
            {
                string[] arrCountry = listCountry[i].Split(';');
                paramss.Clear();
                paramss.Add("id_country", arrCountry[0], System.Data.DbType.Int32);
                paramss.Add("name_country", arrCountry[1], System.Data.DbType.String);
                paramss.Add("sname_country", arrCountry[2], System.Data.DbType.String);
                if (sqliteQ._sqlt.Insert("country", paramss) == 0)
                    cntErr++;            
            }
            MessageBox.Show("Данные о странах: Обработано записей: " + listCountry.Count.ToString() + ". Ошибок: " + cntErr.ToString() + ".");

            cntErr = 0;
            for (int i = 0; i < listRegion.Count; i++)
            {
                string[] arrRegion = listRegion[i].Split(';');
                paramss.Clear();
                paramss.Add("id_region", arrRegion[0], System.Data.DbType.Int32);
                paramss.Add("name_region", arrRegion[1], System.Data.DbType.String);
                paramss.Add("sname_region", arrRegion[2], System.Data.DbType.String);
                if (sqliteQ._sqlt.Insert("region", paramss) == 0)
                    cntErr++;
            }
            MessageBox.Show("Данные о регионах: Обработано записей: " + listRegion.Count.ToString() + ". Ошибок: " + cntErr.ToString() + ".");

            cntErr = 0;
            for (int i = 0; i < listCountryRegion.Count; i++)
            {
                string[] arrCountryRegion = listCountryRegion[i].Split(';');
                paramss.Clear();
                paramss.Add("id_region", arrCountryRegion[0], System.Data.DbType.Int32);
                paramss.Add("id_country", arrCountryRegion[1], System.Data.DbType.Int32);
                if (sqliteQ._sqlt.Insert("country_region", paramss) == 0)
                    cntErr++;
            }
            MessageBox.Show("Данные о связях страна-регион: Обработано записей: " + listCountryRegion.Count.ToString() + ". Ошибок: " + cntErr.ToString() + ".");
            */
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
