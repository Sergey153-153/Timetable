using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLib;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace mySQLite
{
    public class SQLiteQueries
    {
        public DbFacadeSQLite _sqlt;

        public SQLiteQueries(string dbName)
        {
            _sqlt = new DbFacadeSQLite(dbName);
        }


        #region Создание таблиц в БД
        /// <summary>
        /// Создание таблиц
        /// </summary>
        private void SaveLog(string txt, string fileTo = "testSQL.txt")
        {
            StreamWriter streamWriter = new StreamWriter(@"..\..\..\" + fileTo, false, Encoding.GetEncoding("utf-8"));
            streamWriter.WriteLine(txt);
            streamWriter.Close();
        }

        public void ClearDB()
        {
            DataTable dt = _sqlt.Execute(@"SELECT 'drop table ' || name || ';'    
                                     FROM sqlite_master
                                     WHERE type = 'table';");
            _sqlt.BeginTransaction();
            foreach (DataRow row in dt.Rows)
            {
                _sqlt.ExecuteNonQuery(row[0].ToString());
            }
            _sqlt.CommitTransaction();
        }

        public int CreateTables(string dbName, bool isTransact = true)
        {
            ClearDB();
            string sqlCmd = @"CREATE TABLE country (
                    [id_country] INT NOT NULL,
                    [name_country] TEXT NOT NULL,
                    [sname_country] TEXT NOT NULL,
                    PRIMARY KEY (id_country));";

            sqlCmd += @"CREATE TABLE region (
                    [id_region] INT NOT NULL,
                    [name_region] TEXT NOT NULL,
                    [sname_region] TEXT NOT NULL,
                    PRIMARY KEY (id_region));";

            sqlCmd += @"CREATE TABLE country_region (
                    [id_region] INT NOT NULL,
                    [id_country] INT NOT NULL,
                    FOREIGN KEY(id_region) REFERENCES region(id_region),
                    FOREIGN KEY(id_country) REFERENCES country(id_country)
                    );";

            if (isTransact)
                _sqlt.BeginTransaction();

            ConnectionState previousConnectionState = ConnectionState.Closed;
            try
            {
                previousConnectionState = _sqlt.connect.State;
                if (_sqlt.connect.State == ConnectionState.Closed)
                {
                    _sqlt.connect.Open();
                }
                _sqlt.command = new SQLiteCommand(_sqlt.connect);
                _sqlt.command.CommandText = sqlCmd;
                _sqlt.command.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                _sqlt.SaveLog(1, string.Format("Ошибка при генерации таблиц новой базы данных: {0}!", error.Message));
                if (isTransact)
                    _sqlt.RollBackTransaction();
                return 0;
            }
            finally
            {
                if (previousConnectionState == ConnectionState.Closed)
                {
                    _sqlt.connect.Close();
                }
            }

            if (isTransact)
                _sqlt.CommitTransaction();
            return 1;
        }
        #endregion


        public int getFirstFreeIndexRecord(string dbtable)
        {
            string curResult = string.Empty;
            string curTable = string.Empty;
            switch (dbtable)
            {
                case "city":
                    curResult = "MAX (ci.id_city) maxId";
                    curTable = "city ci";
                    break;
            }
            DataTable dt = _sqlt.FetchByColumn(curTable, curResult, "", "");
            return int.Parse(dt.Rows[0]["maxId"].ToString()) + 1;
        }

        public int getIdRecord(string dbtable, string value, string value2="")
        {
            string curResult = string.Empty;
            string curTable = string.Empty;
            string curCondition = string.Empty;

            switch (dbtable)
            {
                case "region":
                    curResult = "r.id_region idRec";
                    curTable = "region r";
                    curCondition = "r.name_region = " + "'" + value + "'";
                break;
                case "city":
                    curResult = "ci.id_city idRec";
                    curTable = "city ci";
                    curCondition = "ci.name_city = " + "'" + value + "'";
                    if (value2 != string.Empty)
                        curCondition += " AND ci.note_city = " + "'" + value2 + "'";
                break;
            }

            DataTable dt = _sqlt.FetchByColumn(curTable, curResult, curCondition, "");
            return int.Parse(dt.Rows[0]["idRec"].ToString());
        }

        #region Запросы данных о городах

        public int getNumberOfCities(string curCity)
        {
            DataTable dt = _sqlt.FetchByColumn("city ci", "COUNT(*) as cnt", " ci.name_city = '" + curCity + "' ");
            return int.Parse(dt.Rows[0]["cnt"].ToString());
        }

        public Dictionary<string, int> getListCountry()
        {
            Dictionary<string, int> newD = new Dictionary<string, int>();
            DataTable dt = _sqlt.FetchByColumn("country", "id_country, name_country", "", "ORDER BY name_country ASC");
            foreach (DataRow row in dt.Rows)
            {
                newD[row["name_country"].ToString()] = int.Parse(row["id_country"].ToString());
                
            }
            return newD;
        }

        public List<SQLiteProject.Form1.InfoRegion> getListRegion(string curCountry = "")
        {
            Dictionary<string, int> newD = new Dictionary<string, int>();

            string curResult = "r.id_region rid, r.name_region rname";
            string curTable = "region r";
            string curCondition = string.Empty;
            string curOther = "ORDER BY r.name_region ASC";

            if (curCountry != string.Empty)
            {
                curTable = "region r, country c, country_region rc";
                curCondition = " c.name_country = '" + curCountry + "' AND rc.id_country = c.id_country AND r.id_region = rc.id_region ";
            }
            DataTable dt = _sqlt.FetchByColumn(curTable, curResult, curCondition, curOther);

            List<SQLiteProject.Form1.InfoRegion> tmpList = new List<SQLiteProject.Form1.InfoRegion>();

            SQLiteProject.Form1.InfoRegion tmpRec;
            foreach (DataRow row in dt.Rows)
            {
                tmpRec = new SQLiteProject.Form1.InfoRegion();
                tmpRec.region_id = int.Parse(row["rid"].ToString());
                tmpRec.region_name = row["rname"].ToString();
                tmpList.Add(tmpRec);
            }
            return tmpList;
        }
        #endregion
    }
}
